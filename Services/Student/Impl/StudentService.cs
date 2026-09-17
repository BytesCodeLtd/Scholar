using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Common.Storage;
using Scholar.Constants;
using Scholar.Enums;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class StudentService(
        IRepository<Student> students,
        IRepository<InstituteClass> classes,
        IRepository<Institute> institutes,
        IRepository<Attendance> attendance,
        IFileStorage fileStorage,
        ITenantProvider tenant) : IStudentService
    {
        private readonly IRepository<Student> _students = students;
        private readonly IRepository<InstituteClass> _classes = classes;
        private readonly IRepository<Institute> _institutes = institutes;
        private readonly IRepository<Attendance> _attendance = attendance;
        private readonly IFileStorage _fileStorage = fileStorage;
        private readonly ITenantProvider _tenant = tenant;

        public bool CanOpenAdmissionForm => _tenant.IsSuperAdmin || _tenant.InstituteId is not null;

        public async Task<Common.Paging.PagedResult<StudentRow>> GetRosterAsync(PageParameters tableParams)
        {
            IQueryable<Student> students = _students.Query();

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                students = students.Where(s =>
                    s.FullName.Contains(term) ||
                    (s.RollNumber != null && s.RollNumber.Contains(term)) ||
                    (s.Class != null && s.Class.Name.Contains(term)) ||
                    (s.Section != null && s.Section.Contains(term)));
            }

            IQueryable<StudentRow> rows = students.Select(s => new StudentRow
            {
                Id = s.Id,
                Name = s.FullName,
                RollNumber = s.RollNumber,
                Class = s.Class != null ? s.Class.Name : null,
                Section = s.Section,
                Guardian = s.GuardianName,
                Phone = s.PhoneNumber,
                Institute = s.Institute.Name,
                IsActive = s.IsActive
            });

            // Id isn't a display column, so the default/unset sort falls back to Name.
            string orderBy = tableParams.OrderBy;

            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "Name asc";
            }
            rows = rows.OrderBy(orderBy);

            return await _students.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);
        }

        public async Task<StudentDetailsViewModel?> GetDetailsAsync(int id)
        {
            StudentDetailsViewModel? model = await _students.Query()
                .Where(s => s.Id == id)
                .Select(s => new StudentDetailsViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    RollNumber = s.RollNumber,
                    Class = s.Class != null ? s.Class.Name : string.Empty,
                    Section = s.Section,
                    Gender = s.Gender,
                    DateOfBirth = s.DateOfBirth,
                    GuardianName = s.GuardianName,
                    PhoneNumber = s.PhoneNumber,
                    Institute = s.Institute.Name,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (model is null)
            {
                return null;
            }

            // All-time status counts for the summary tiles.
            var counts = await _attendance.Query()
                                          .Where(a => a.StudentId == id)
                                          .GroupBy(a => a.Status)
                                          .Select(g => new { Status = g.Key, Count = g.Count() })
                                          .ToListAsync();

            model.PresentCount = counts.FirstOrDefault(c => c.Status == AttendanceStatus.Present)?.Count ?? 0;
            model.AbsentCount = counts.FirstOrDefault(c => c.Status == AttendanceStatus.Absent)?.Count ?? 0;
            model.LateCount = counts.FirstOrDefault(c => c.Status == AttendanceStatus.Late)?.Count ?? 0;
            model.LeaveCount = counts.FirstOrDefault(c => c.Status == AttendanceStatus.Leave)?.Count ?? 0;

            // Most recent marks for the history list.
            model.AttendanceHistory = await _attendance.Query()
                                                       .Where(a => a.StudentId == id)
                                                       .OrderByDescending(a => a.Date)
                                                       .Take(30)
                                                       .Select(a => new AttendanceHistoryItem { Date = a.Date, Status = a.Status })
                                                       .ToListAsync();

            return model;
        }

        public async Task PopulateCreateOptionsAsync(CreateStudentViewModel model)
        {
            model.GradeOptions = await GradeOptionsAsync();
            model.ShowInstitute = _tenant.IsSuperAdmin;

            if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await InstituteOptionsAsync();
            }
        }

        public async Task<bool> CreateAsync(CreateStudentViewModel model)
        {
            int? instituteId = ResolveInstitute(model.InstituteId);
            if (instituteId is null || !await ClassBelongsToInstituteAsync(model.GradeId, instituteId.Value))
            {
                return false;
            }

            Student student = new()
            {
                InstituteId = instituteId.Value,
                ClassId = model.GradeId,
                FullName = model.FullName.Trim(),
                RollNumber = model.RollNumber?.Trim(),
                Section = model.Section?.Trim(),
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                GuardianName = model.GuardianName?.Trim(),
                PhoneNumber = model.PhoneNumber?.Trim()
            };

            await _students.AddAsync(student);
            await _students.SaveChangesAsync();

            return true;
        }

        public async Task PopulateRegisterOptionsAsync(RegisterStudentViewModel model)
        {
            model.ClassOptions = await ClassOptionsWithSectionsAsync();
            model.ShowInstitute = _tenant.IsSuperAdmin;

            if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await InstituteOptionsAsync();
            }

            // "Last #" hints come from the most recently added student in scope
            // (the query filter already limits this to the current institute).
            var last = await _students.Query()
                                      .OrderByDescending(s => s.Id)
                                      .Select(s => new { s.AdmissionNumber, s.RollNumber })
                                      .FirstOrDefaultAsync();

            model.LastAdmissionNumber = last?.AdmissionNumber;
            model.LastRollNumber = last?.RollNumber;
        }

        public async Task<bool> RegisterAsync(RegisterStudentViewModel model)
        {
            int? instituteId = ResolveInstitute(model.InstituteId);
            if (instituteId is null || !await ClassBelongsToInstituteAsync(model.GradeId, instituteId.Value))
            {
                return false;
            }

            string fullName = string.Join(' ',
                new[] { model.FirstName?.Trim(), model.LastName?.Trim() }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

            Student student = new()
            {
                InstituteId = instituteId.Value,
                ClassId = model.GradeId,
                AdmissionNumber = model.AdmissionNumber?.Trim(),
                AdmissionSession = model.AdmissionSession?.Trim(),
                AdmissionDate = model.AdmissionDate,
                FullName = fullName,
                FirstName = model.FirstName?.Trim(),
                LastName = model.LastName?.Trim(),
                RollNumber = model.RollNumber?.Trim(),
                Section = model.Section?.Trim(),
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Category = model.Category?.Trim(),
                Religion = model.Religion?.Trim(),
                PhoneNumber = model.MobileNumber?.Trim(),
                Email = model.Email?.Trim(),
                Caste = model.Caste?.Trim(),
                BloodGroup = model.BloodGroup?.Trim(),
                Height = model.Height?.Trim(),
                Weight = model.Weight?.Trim(),
                FamilyNo = model.FamilyNo?.Trim(),
                IsFamilyHead = model.IsFamilyHead,
                AsOnDate = model.AsOnDate,
                BFormCnic = model.BFormCnic?.Trim(),
                WhatsappNumber = model.WhatsappNumber?.Trim(),
                CardId = model.CardId?.Trim(),
                RefBy = model.RefBy?.Trim(),
                LastClassAttended = model.LastClassAttended?.Trim(),
                PreviousSchoolDetails = model.PreviousSchoolDetails?.Trim(),
                FatherName = model.FatherName?.Trim(),
                FatherCnic = model.FatherCnic?.Trim(),
                FatherPhone = model.FatherPhone?.Trim(),
                FatherOccupation = model.FatherOccupation?.Trim(),
                MotherName = model.MotherName?.Trim(),
                MotherCnic = model.MotherCnic?.Trim(),
                MotherPhone = model.MotherPhone?.Trim(),
                MotherOccupation = model.MotherOccupation?.Trim(),
                GuardianType = model.GuardianType,
                GuardianName = model.GuardianName?.Trim(),
                GuardianRelation = model.GuardianRelation?.Trim(),
                GuardianEmail = model.GuardianEmail?.Trim(),
                GuardianPhone = model.GuardianPhone?.Trim(),
                GuardianOccupation = model.GuardianOccupation?.Trim(),
                GuardianAddress = model.GuardianAddress?.Trim(),
                ConcessionType = model.ConcessionType?.Trim(),
                ConcessionValidTill = model.ConcessionValidTill,
                TuitionFeeHead = model.TuitionFeeHead?.Trim(),
                AdmissionHead = model.AdmissionHead?.Trim(),
                AdmissionDiscount = model.AdmissionDiscount,
                TermsAndConditions = model.TermsAndConditions?.Trim()
            };

            // Upload any provided photos and store their URLs.
            student.PhotoUrl = await UploadPhotoAsync(model.StudentPhoto);
            student.FatherPhotoUrl = await UploadPhotoAsync(model.FatherPhoto);
            student.MotherPhotoUrl = await UploadPhotoAsync(model.MotherPhoto);
            student.GuardianPhotoUrl = await UploadPhotoAsync(model.GuardianPhoto);

            await _students.AddAsync(student);
            await _students.SaveChangesAsync();

            return true;
        }

        public Task<bool> DeactivateAsync(int id) => SetActiveAsync(id, active: false);

        public Task<bool> ActivateAsync(int id) => SetActiveAsync(id, active: true);

        private int? ResolveInstitute(int? pickedBySuperAdmin)
            => _tenant.IsSuperAdmin ? pickedBySuperAdmin : _tenant.InstituteId;

        private async Task<bool> SetActiveAsync(int id, bool active)
        {
            if (!_tenant.IsSuperAdmin && !await _students.Query().AnyAsync(s => s.Id == id))
            {
                return false;
            }

            return active ? await _students.ActivateAsync(id) : await _students.SoftDeleteAsync(id);
        }

        private async Task<string?> UploadPhotoAsync(IFormFile? file)
            => file is { Length: > 0 } ? await _fileStorage.UploadAsync(file, "students") : null;

        private async Task<List<SelectListItem>> GradeOptionsAsync()
            => await _classes.Query()
                             .Where(c => c.IsActive)
                             .OrderBy(c => c.Name)
                             .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                             .ToListAsync();

        private Task<bool> ClassBelongsToInstituteAsync(int classId, int instituteId)
            => _classes.Query().AnyAsync(c => c.Id == classId && c.InstituteId == instituteId && c.IsActive);

        private async Task<List<StudentClassOption>> ClassOptionsWithSectionsAsync()
            => await _classes.Query()
                             .Where(c => c.IsActive)
                             .OrderBy(c => c.Name)
                             .Select(c => new StudentClassOption
                             {
                                 Id = c.Id,
                                 Name = c.Name,
                                 Sections = c.Sections.OrderBy(s => s.Name).Select(s => s.Name).ToList()
                             })
                             .ToListAsync();

        private async Task<List<SelectListItem>> InstituteOptionsAsync()
            => await _institutes.Query()
                                .Where(i => i.IsActive)
                                .OrderBy(i => i.Name)
                                .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
                                .ToListAsync();
    }
}
