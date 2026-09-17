using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Email;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Data;
using Scholar.Enums;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class FeesService : IFeesService
    {
        private readonly IRepository<FeeCategory> _categories;
        private readonly IRepository<Invoice> _invoices;
        private readonly IRepository<Student> _students;
        private readonly IRepository<Grade> _grades;
        private readonly IRepository<Institute> _institutes;
        private readonly ScholarDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateRenderer _templates;
        private readonly ITenantProvider _tenant;
        private readonly ILogger<FeesService> _logger;

        public FeesService(
            IRepository<FeeCategory> categories,
            IRepository<Invoice> invoices,
            IRepository<Student> students,
            IRepository<Grade> grades,
            IRepository<Institute> institutes,
            ScholarDbContext db,
            IEmailSender emailSender,
            IEmailTemplateRenderer templates,
            ITenantProvider tenant,
            ILogger<FeesService> logger)
        {
            _categories = categories;
            _invoices = invoices;
            _students = students;
            _grades = grades;
            _institutes = institutes;
            _db = db;
            _emailSender = emailSender;
            _templates = templates;
            _tenant = tenant;
            _logger = logger;
        }

        public bool IsSuperAdmin => _tenant.IsSuperAdmin;

        public async Task<Common.Paging.PagedResult<InvoiceRow>> GetInvoicesAsync(PageParameters tableParams, string? status)
        {
            IQueryable<Invoice> invoices = _invoices.Query();

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            invoices = status?.ToLowerInvariant() switch
            {
                "unpaid" => invoices.Where(i => i.Status == InvoiceStatus.Unpaid),
                "partial" => invoices.Where(i => i.Status == InvoiceStatus.Partial),
                "paid" => invoices.Where(i => i.Status == InvoiceStatus.Paid),
                "overdue" => invoices.Where(i => i.Status != InvoiceStatus.Paid && i.DueDate < today),
                _ => invoices
            };

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                invoices = invoices.Where(i =>
                    i.Student.FullName.Contains(term) ||
                    i.Period.Contains(term) ||
                    i.Student.Grade.Name.Contains(term));
            }

            var projected = invoices.Select(i => new
            {
                i.Id,
                Student = i.Student.FullName,
                Class = i.Student.Grade.Name,
                i.Period,
                i.DueDate,
                Total = i.Items.Sum(x => (decimal?)x.Amount) ?? 0m,
                Paid = i.Payments.Sum(p => (decimal?)p.Amount) ?? 0m,
                i.Status,
                Institute = i.Institute.Name
            });

            string orderBy = tableParams.OrderBy;

            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "DueDate desc";
            }
            projected = projected.OrderBy(orderBy);

            var paged = await _invoices.GetPagedAsync(projected, tableParams.Page, tableParams.PageSize);

            List<InvoiceRow> rows = paged.Items.Select(i => new InvoiceRow
            {
                Id = i.Id,
                Student = i.Student,
                Class = i.Class,
                Period = i.Period,
                DueDate = i.DueDate.ToString("dd MMM yyyy"),
                Total = Common.Money.Pkr(i.Total),
                Balance = Common.Money.Pkr(i.Total - i.Paid),
                Status = i.Status == InvoiceStatus.Paid
                    ? "Paid"
                    : (i.DueDate < today ? "Overdue" : i.Status.ToString()),
                Institute = i.Institute
            }).ToList();

            return new Common.Paging.PagedResult<InvoiceRow>
            {
                Items = rows,
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<InvoiceDetailsViewModel?> GetInvoiceDetailsAsync(int id)
        {
            Invoice? invoice = await _invoices.Query()
                .Include(i => i.Student).ThenInclude(s => s.Grade)
                .Include(i => i.Institute)
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice is null)
            {
                return null;
            }

            return new InvoiceDetailsViewModel
            {
                Id = invoice.Id,
                StudentName = invoice.Student.FullName,
                RollNumber = invoice.Student.RollNumber,
                Class = invoice.Student.Grade.Name,
                Section = invoice.Student.Section,
                Institute = invoice.Institute.Name,
                Period = invoice.Period,
                DueDate = invoice.DueDate,
                Status = invoice.Status,
                IsOverdue = invoice.IsOverdue,
                Total = invoice.Total,
                Paid = invoice.Paid,
                Items = invoice.Items.Select(x => new InvoiceItemLine { Description = x.Description, Amount = x.Amount }).ToList(),
                Payments = invoice.Payments.OrderByDescending(p => p.PaidOn)
                    .Select(p => new PaymentLine { Amount = p.Amount, PaidOn = p.PaidOn, Method = p.Method, Note = p.Note }).ToList(),
                PaymentAmount = invoice.Balance > 0 ? invoice.Balance : 0
            };
        }

        public async Task<bool> RecordPaymentAsync(int id, decimal amount, DateOnly paidOn, PaymentMethod method, string? note)
        {
            Invoice? invoice = await _invoices.Query()
                                              .Include(i => i.Items)
                                              .Include(i => i.Payments)
                                              .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice is null)
            {
                return false;
            }

            invoice.Payments.Add(new Payment
            {
                InvoiceId = invoice.Id,
                Amount = amount,
                PaidOn = paidOn == default ? DateOnly.FromDateTime(DateTime.Today) : paidOn,
                Method = method,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
            });

            invoice.Status = ResolveStatus(invoice.Total, invoice.Paid + amount);

            _invoices.Update(invoice);
            await _invoices.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SendReminderAsync(int id)
        {
            Invoice? invoice = await _invoices.Query()
                                              .Include(i => i.Student)
                                              .Include(i => i.Items)
                                              .Include(i => i.Payments)
                                              .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice is null)
            {
                return false;
            }

            string? to = invoice.Student.PhoneNumber; // placeholder: no guardian email field yet
            if (string.IsNullOrWhiteSpace(to) || !to.Contains('@'))
            {
                return false;
            }

            try
            {
                string html = await _templates.RenderAsync("FeeDue.html", new Dictionary<string, string>
                {
                    ["StudentName"] = System.Net.WebUtility.HtmlEncode(invoice.Student.FullName),
                    ["Period"] = System.Net.WebUtility.HtmlEncode(invoice.Period),
                    ["Balance"] = Common.Money.Pkr(invoice.Balance),
                    ["DueDate"] = invoice.DueDate.ToString("dd MMM yyyy")
                });

                await _emailSender.SendAsync(to, "Fee payment reminder", html);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send fee reminder for invoice {InvoiceId}", id);
                return false;
            }
        }

        public async Task<GenerateInvoicesViewModel> BuildGenerateFormAsync(int? instituteId)
        {
            int? targetInstitute = _tenant.IsSuperAdmin ? instituteId : _tenant.InstituteId;

            GenerateInvoicesViewModel model = new()
            {
                ShowInstitute = _tenant.IsSuperAdmin,
                InstituteId = targetInstitute
            };

            await PopulateGenerateOptionsAsync(model);
            return model;
        }

        public async Task<int?> GenerateAsync(GenerateInvoicesViewModel model)
        {
            // Institute + line selection are validated in the controller before we get here.
            int? targetInstitute = _tenant.IsSuperAdmin ? model.InstituteId : _tenant.InstituteId;

            if (targetInstitute is null)
            {
                return null;
            }

            List<GenerateLineViewModel> selected = model.Lines.Where(l => l.Include && l.Amount > 0).ToList();

            // Students in the chosen class (and section, if given) at this institute.
            IQueryable<Student> studentsQuery = _students.Query().Where(s => s.IsActive && s.InstituteId == targetInstitute && s.GradeId == model.GradeId);

            if (!string.IsNullOrWhiteSpace(model.Section))
            {
                string section = model.Section.Trim();
                studentsQuery = studentsQuery.Where(s => s.Section == section);
            }

            List<int> studentIds = await studentsQuery.Select(s => s.Id).ToListAsync();

            if (studentIds.Count == 0)
            {
                return null;
            }

            int created = 0;
            var strategy = _db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _db.Database.BeginTransactionAsync();

                foreach (int studentId in studentIds)
                {
                    // Skip if this student already has an invoice for this exact period.
                    bool exists = await _invoices.Query().AnyAsync(i => i.StudentId == studentId && i.Period == model.Period);

                    if (exists)
                    {
                        continue;
                    }

                    Invoice invoice = new()
                    {
                        InstituteId = targetInstitute!.Value,
                        StudentId = studentId,
                        Period = model.Period.Trim(),
                        DueDate = model.DueDate,
                        Status = InvoiceStatus.Unpaid,
                        Items = selected.Select(l => new InvoiceItem
                        {
                            FeeCategoryId = l.CategoryId,
                            Description = l.Name,
                            Amount = l.Amount
                        }).ToList()
                    };

                    await _invoices.AddAsync(invoice);
                    created++;
                }

                await _invoices.SaveChangesAsync();
                await transaction.CommitAsync();
            });

            return created;
        }

        public async Task<Common.Paging.PagedResult<FeeCategoryRow>> GetCategoriesAsync(PageParameters tableParams)
        {
            IQueryable<FeeCategory> categories = _categories.Query();

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                categories = categories.Where(c => c.Name.Contains(term));
            }

            var projected = categories.Select(c => new
            {
                c.Id,
                c.Name,
                c.DefaultAmount,
                Institute = c.Institute.Name,
                c.IsActive
            });

            string orderBy = tableParams.OrderBy;

            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "Name asc";
            }
            projected = projected.OrderBy(orderBy);

            var paged = await _categories.GetPagedAsync(projected, tableParams.Page, tableParams.PageSize);

            List<FeeCategoryRow> rows = paged.Items.Select(c => new FeeCategoryRow
            {
                Id = c.Id,
                Name = c.Name,
                DefaultAmount = Common.Money.Pkr(c.DefaultAmount),
                Institute = c.Institute,
                IsActive = c.IsActive
            }).ToList();

            return new Common.Paging.PagedResult<FeeCategoryRow>
            {
                Items = rows,
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<FeeCategoryFormViewModel?> BuildCategoryFormAsync(int? id)
        {
            FeeCategoryFormViewModel model = new() { ShowInstitute = _tenant.IsSuperAdmin };

            if (id is not null)
            {
                FeeCategory? category = await _categories.Query().FirstOrDefaultAsync(c => c.Id == id.Value);

                if (category is null)
                {
                    return null;
                }

                model.Id = category.Id;
                model.Name = category.Name;
                model.DefaultAmount = category.DefaultAmount;
                model.InstituteId = category.InstituteId;
            }

            await PopulateCategoryOptionsAsync(model);
            return model;
        }

        public async Task<bool> SaveCategoryAsync(FeeCategoryFormViewModel model)
        {
            // The controller validates that a super admin picked an institute.
            int? targetInstitute = _tenant.IsSuperAdmin ? model.InstituteId : _tenant.InstituteId;

            if (targetInstitute is null)
            {
                return false;
            }

            if (model.Id is null)
            {
                await _categories.AddAsync(new FeeCategory
                {
                    InstituteId = targetInstitute.Value,
                    Name = model.Name.Trim(),
                    DefaultAmount = model.DefaultAmount
                });
                await _categories.SaveChangesAsync();
                return true;
            }

            FeeCategory? category = await _categories.Query().FirstOrDefaultAsync(c => c.Id == model.Id.Value);
            if (category is null)
            {
                return false;
            }

            category.Name = model.Name.Trim();
            category.DefaultAmount = model.DefaultAmount;

            _categories.Update(category);
            await _categories.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            bool categoryExist = await _categories.Query().AnyAsync(c => c.Id == id);

            if (!_tenant.IsSuperAdmin && !categoryExist)
            {
                return false;
            }

            return await _categories.SoftDeleteAsync(id);
        }

        private static InvoiceStatus ResolveStatus(decimal total, decimal paid) =>
            paid <= 0 ? InvoiceStatus.Unpaid : (paid >= total ? InvoiceStatus.Paid : InvoiceStatus.Partial);

        public async Task PopulateCategoryOptionsAsync(FeeCategoryFormViewModel model)
        {
            model.ShowInstitute = _tenant.IsSuperAdmin;

            if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await InstituteOptionsAsync();
            }
        }

        public async Task PopulateGenerateOptionsAsync(GenerateInvoicesViewModel model)
        {
            model.ShowInstitute = _tenant.IsSuperAdmin;

            if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await InstituteOptionsAsync();
            }

            model.GradeOptions = await _grades.Query()
                                              .Where(g => g.IsActive)
                                              .OrderBy(g => g.Name)
                                              .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
                                              .ToListAsync();

            int? instituteId = _tenant.IsSuperAdmin ? model.InstituteId : _tenant.InstituteId;

            List<GenerateLineViewModel> categoryLines = [];

            if (instituteId is not null)
            {
                categoryLines = await _categories.Query()
                                                 .Where(c => c.IsActive && c.InstituteId == instituteId)
                                                 .OrderBy(c => c.Name)
                                                 .Select(c => new GenerateLineViewModel
                                                 {
                                                     CategoryId = c.Id,
                                                     Name = c.Name,
                                                     Amount = c.DefaultAmount,
                                                     Include = true
                                                 })
                                                 .ToListAsync();
            }

            // Preserve any amounts/toggles the user already set on postback.
            if (model.Lines.Count > 0)
            {
                foreach (var line in categoryLines)
                {
                    var existing = model.Lines.FirstOrDefault(l => l.CategoryId == line.CategoryId);
                    if (existing is not null)
                    {
                        line.Amount = existing.Amount;
                        line.Include = existing.Include;
                    }
                }
            }

            model.Lines = categoryLines;
        }

        private async Task<List<SelectListItem>> InstituteOptionsAsync()
            => await _institutes.Query()
                                .Where(i => i.IsActive)
                                .OrderBy(i => i.Name)
                                .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
                                .ToListAsync();
    }
}
