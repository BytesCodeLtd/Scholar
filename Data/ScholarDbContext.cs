using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Models;

namespace Scholar.Data
{
    public class ScholarDbContext(DbContextOptions<ScholarDbContext> options, ITenantProvider? tenant = null) : IdentityDbContext<ApplicationUser>(options)
    {
        private readonly ITenantProvider? _tenant = tenant;

        private int? _tenantId => _tenant?.InstituteId;
        private bool _bypassTenantFilter => _tenant?.BypassFilter ?? true;

        public DbSet<Institute> Institutes => Set<Institute>();
        public DbSet<Board> Boards => Set<Board>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Chapter> Chapters => Set<Chapter>();
        public DbSet<Topic> Topics => Set<Topic>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<McqOption> McqOptions => Set<McqOption>();
        public DbSet<Test> Test => Set<Test>();
        public DbSet<TestQuestion> TestQuestions => Set<TestQuestion>();
        public DbSet<Teacher> Teacher => Set<Teacher>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<InstituteClass> Classes => Set<InstituteClass>();
        public DbSet<InstituteSubject> InstituteSubjects => Set<InstituteSubject>();
        public DbSet<SubjectGroup> SubjectGroups => Set<SubjectGroup>();
        public DbSet<TestSettings> TestSettings => Set<TestSettings>();
        public DbSet<PastPaper> PastPapers => Set<PastPaper>();
        public DbSet<TestSection> TestSections => Set<TestSection>();
        public DbSet<FeeCategory> FeeCategories => Set<FeeCategory>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // A user belongs to at most one institute; removing an institute
            // detaches its users (their InstituteId is set to NULL) rather than
            // deleting the accounts.
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Institute).WithMany(i => i.Users)
                .HasForeignKey(u => u.InstituteId).OnDelete(DeleteBehavior.SetNull);

            // Teaching assignments (user teaches a subject in a grade). GradeId is
            // NoAction to avoid multiple cascade paths (Grade already cascades via
            // Subject). Unique triple prevents duplicate assignments.
            builder.Entity<Teacher>()
                .HasOne(t => t.User).WithMany()
                .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Teacher>()
                .HasOne(t => t.Subject).WithMany()
                .HasForeignKey(t => t.SubjectId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Teacher>()
                .HasOne(t => t.Grade).WithMany()
                .HasForeignKey(t => t.GradeId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Teacher>()
                .HasIndex(t => new { t.UserId, t.SubjectId, t.GradeId }).IsUnique();

            // Students belong to an institute (removing an institute removes its
            // students). GradeId is NoAction to avoid a second cascade path.
            // Gender is stored as a readable string rather than an int.
            builder.Entity<Student>()
                .HasOne(s => s.Institute).WithMany()
                .HasForeignKey(s => s.InstituteId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Student>()
                .HasOne(s => s.Grade).WithMany()
                .HasForeignKey(s => s.GradeId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Student>()
                .Property(s => s.Gender).HasConversion<string>().HasMaxLength(20);
            builder.Entity<Student>()
                .Property(s => s.GuardianType).HasConversion<string>().HasMaxLength(20);
            builder.Entity<Student>()
                .Property(s => s.AdmissionDiscount).HasPrecision(18, 2);
            builder.Entity<Student>()
                .HasIndex(s => s.InstituteId);

            // Daily attendance. Deleting a student removes their attendance
            // (cascade); the Institute FK is NoAction to avoid a second cascade
            // path (attendance already cascades via Student -> Institute). Status
            // is stored as a readable string. At most one row per student per day.
            builder.Entity<Attendance>()
                .HasOne(a => a.Student).WithMany()
                .HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Attendance>()
                .HasOne(a => a.Institute).WithMany()
                .HasForeignKey(a => a.InstituteId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Attendance>()
                .Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
            builder.Entity<Attendance>()
                .Property(a => a.Date).HasColumnType("date");
            builder.Entity<Attendance>()
                .HasIndex(a => new { a.StudentId, a.Date }).IsUnique();
            builder.Entity<Attendance>()
                .HasIndex(a => new { a.InstituteId, a.Date });

            // Curriculum tree: Boards and Grades are global lookups. A Subject ties a
            // grade to a board (both FKs live on Subject). Deleting a grade cascades to
            // its subjects; a board is Restrict (global/stable, not deleted casually).
            builder.Entity<Subject>()
                .HasOne(s => s.Board).WithMany()
                .HasForeignKey(s => s.BoardId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Subject>()
                .HasOne(s => s.Grade).WithMany(g => g.Subjects)
                .HasForeignKey(s => s.GradeId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Chapter>()
                .HasOne(c => c.Subject).WithMany(s => s.Chapters)
                .HasForeignKey(c => c.SubjectId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Topic>()
                .HasOne(t => t.Chapter).WithMany(c => c.Topics)
                .HasForeignKey(t => t.ChapterId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Question>()
                .HasOne(q => q.Topic).WithMany(t => t.Questions)
                .HasForeignKey(q => q.TopicId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<McqOption>()
                .HasOne(o => o.Question).WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId).OnDelete(DeleteBehavior.Cascade);

            // A question may be privately owned by a teacher; keep the question if the
            // owner is removed by leaving it orphaned as a shared/no-owner question.
            builder.Entity<Question>()
                .HasOne(q => q.Owner).WithMany(u => u.Questions)
                .HasForeignKey(q => q.OwnerId).OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Question>().HasIndex(q => new { q.TopicId, q.Type });

            // One test-settings row per institute; removing an institute
            // removes its settings.
            builder.Entity<TestSettings>()
                .HasOne(p => p.Institute).WithOne()
                .HasForeignKey<TestSettings>(p => p.InstituteId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<TestSettings>()
                .HasIndex(p => p.InstituteId).IsUnique();
            builder.Entity<TestSettings>()
                .Property(p => p.LineHeight).HasColumnType("decimal(3,1)");
            builder.Entity<TestSettings>()
                .Property(p => p.PicWatermarkOpacity).HasColumnType("decimal(3,1)");

            // Past papers hang off a subject; removing a subject removes its
            // past papers.
            builder.Entity<PastPaper>()
                .HasOne(p => p.Subject).WithMany()
                .HasForeignKey(p => p.SubjectId).OnDelete(DeleteBehavior.Cascade);

            // Tests belong to an institute; removing an institute removes its tests.
            builder.Entity<Test>()
                .HasOne(t => t.Institute).WithMany()
                .HasForeignKey(t => t.InstituteId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Test>()
                .HasOne(t => t.Subject).WithMany()
                .HasForeignKey(t => t.SubjectId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Test>().HasIndex(t => new { t.InstituteId, t.IsActive });

            builder.Entity<TestQuestion>()
                .HasOne(tq => tq.Test).WithMany(t => t.Questions)
                .HasForeignKey(tq => tq.TestId).OnDelete(DeleteBehavior.Cascade);

            // Sections belong to a test (cascade). Questions point at their
            // section with NoAction to avoid a second cascade path into
            // TestQuestions (they already cascade via Test).
            builder.Entity<TestSection>()
                .HasOne(s => s.Test).WithMany(t => t.Sections)
                .HasForeignKey(s => s.TestId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<TestQuestion>()
                .HasOne(tq => tq.TestSection).WithMany(s => s.Questions)
                .HasForeignKey(tq => tq.TestSectionId).OnDelete(DeleteBehavior.NoAction);

            // If a question is deleted, don't wipe the test rows referencing it.
            builder.Entity<TestQuestion>()
                .HasOne(tq => tq.Question).WithMany()
                .HasForeignKey(tq => tq.QuestionId).OnDelete(DeleteBehavior.Restrict);

            // Sections are institute-owned; removed with their institute.
            builder.Entity<Section>()
                .HasOne(s => s.Institute).WithMany()
                .HasForeignKey(s => s.InstituteId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Section>()
                .HasIndex(s => s.InstituteId);
            builder.Entity<Section>()
                .Property(s => s.Name).IsRequired().HasMaxLength(50);

            // Classes are institute-owned and belong to a section. Institute link
            // cascades; the section link is Restrict to avoid a second cascade path.
            // Stored in the singular "Class" table.
            builder.Entity<InstituteClass>().ToTable("Class");
            builder.Entity<InstituteClass>()
                .HasOne(c => c.Institute).WithMany()
                .HasForeignKey(c => c.InstituteId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<InstituteClass>()
                .HasOne(c => c.Section).WithMany()
                .HasForeignKey(c => c.SectionId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<InstituteClass>()
                .HasIndex(c => c.InstituteId);
            builder.Entity<InstituteClass>()
                .Property(c => c.Name).IsRequired().HasMaxLength(50);

            // Subjects are institute-owned; removed with their institute. Type is stored
            // as a readable string rather than an int.
            builder.Entity<InstituteSubject>()
                .HasOne(s => s.Institute).WithMany()
                .HasForeignKey(s => s.InstituteId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<InstituteSubject>()
                .HasIndex(s => s.InstituteId);
            builder.Entity<InstituteSubject>()
                .Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.Entity<InstituteSubject>()
                .Property(s => s.Code).IsRequired().HasMaxLength(30);
            builder.Entity<InstituteSubject>()
                .Property(s => s.Type).HasConversion<string>().HasMaxLength(20);

            // Subject groups are institute-owned and belong to one class. Institute link
            // cascades; the class link is Restrict to avoid a second cascade path.
            builder.Entity<SubjectGroup>()
                .HasOne(g => g.Institute).WithMany()
                .HasForeignKey(g => g.InstituteId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<SubjectGroup>()
                .HasOne(g => g.Class).WithMany()
                .HasForeignKey(g => g.ClassId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<SubjectGroup>()
                .HasIndex(g => g.InstituteId);
            builder.Entity<SubjectGroup>()
                .Property(g => g.Name).IsRequired().HasMaxLength(100);
            builder.Entity<SubjectGroup>()
                .Property(g => g.Description).HasMaxLength(500);

            // Many-to-many joins. The group side cascades (deleting a group clears its
            // join rows); the other side is Restrict to avoid multiple cascade paths
            // back to Institute (which already cascades through the group).
            builder.Entity<SubjectGroup>()
                .HasMany(g => g.Sections).WithMany()
                .UsingEntity(
                    "SubjectGroupSection",
                    r => r.HasOne(typeof(Section)).WithMany().HasForeignKey("SectionId").OnDelete(DeleteBehavior.Restrict),
                    l => l.HasOne(typeof(SubjectGroup)).WithMany().HasForeignKey("SubjectGroupId").OnDelete(DeleteBehavior.Cascade));

            builder.Entity<SubjectGroup>()
                .HasMany(g => g.Subjects).WithMany()
                .UsingEntity(
                    "SubjectGroupSubject",
                    r => r.HasOne(typeof(InstituteSubject)).WithMany().HasForeignKey("InstituteSubjectId").OnDelete(DeleteBehavior.Restrict),
                    l => l.HasOne(typeof(SubjectGroup)).WithMany().HasForeignKey("SubjectGroupId").OnDelete(DeleteBehavior.Cascade));

            builder.Entity<FeeCategory>()
                .HasOne(c => c.Institute).WithMany()
                .HasForeignKey(c => c.InstituteId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<FeeCategory>()
                .HasIndex(c => c.InstituteId);
            builder.Entity<FeeCategory>()
                .Property(c => c.DefaultAmount).HasPrecision(18, 2);

            // Invoices belong to an institute; a student's invoices are removed with the
            // institute. Student link is Restrict to avoid a second cascade path.
            builder.Entity<Invoice>()
                .HasOne(i => i.Institute).WithMany()
                .HasForeignKey(i => i.InstituteId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Invoice>()
                .HasOne(i => i.Student).WithMany()
                .HasForeignKey(i => i.StudentId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Invoice>()
                .HasIndex(i => new { i.InstituteId, i.Status });
            builder.Entity<Invoice>()
                .HasIndex(i => i.StudentId);

            // Items cascade with their invoice; keep item history if a category is removed.
            builder.Entity<InvoiceItem>()
                .HasOne(it => it.Invoice).WithMany(i => i.Items)
                .HasForeignKey(it => it.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<InvoiceItem>()
                .HasOne(it => it.FeeCategory).WithMany()
                .HasForeignKey(it => it.FeeCategoryId).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<InvoiceItem>()
                .Property(it => it.Amount).HasPrecision(18, 2);

            // Payments cascade with their invoice.
            builder.Entity<Payment>()
                .HasOne(p => p.Invoice).WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Payment>()
                .Property(p => p.Amount).HasPrecision(18, 2);

            // Give audit columns a DB default so direct SQL inserts (e.g. the seed
            // script) don't have to supply them. EF still stamps its own values on save.
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (!typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
                    continue;

                builder.Entity(entityType.ClrType)
                    .Property(nameof(IAuditableEntity.CreatedAt))
                    .HasDefaultValueSql("SYSUTCDATETIME()");
                builder.Entity(entityType.ClrType)
                    .Property(nameof(IAuditableEntity.UpdatedAt))
                    .HasDefaultValueSql("SYSUTCDATETIME()");
                builder.Entity(entityType.ClrType)
                    .Property(nameof(IAuditableEntity.IsActive))
                    .HasDefaultValue(true);
            }

            ApplyTenantFilter<Student>(builder);
            ApplyTenantFilter<Attendance>(builder);
            ApplyTenantFilter<Section>(builder);
            ApplyTenantFilter<InstituteClass>(builder);
            ApplyTenantFilter<InstituteSubject>(builder);
            ApplyTenantFilter<SubjectGroup>(builder);
            ApplyTenantFilter<Test>(builder);
            ApplyTenantFilter<TestSettings>(builder);
            ApplyTenantFilter<FeeCategory>(builder);
            ApplyTenantFilter<Invoice>(builder);

            // Invoice children have no InstituteId of their own; scope them through
            // their parent invoice so their filters stay consistent with Invoice's.
            builder.Entity<InvoiceItem>()
                .HasQueryFilter(it => _bypassTenantFilter || it.Invoice.InstituteId == _tenantId);
            builder.Entity<Payment>()
                .HasQueryFilter(p => _bypassTenantFilter || p.Invoice.InstituteId == _tenantId);
        }

        /// <summary>Applies the standard tenant query filter to an institute-owned entity.</summary>
        private void ApplyTenantFilter<T>(ModelBuilder builder) where T : class, ITenantEntity
            => builder.Entity<T>()
                .HasQueryFilter(e => _bypassTenantFilter || e.InstituteId == _tenantId);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            StampTenant();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            StampTenant();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Stamps the current institute onto new tenant entities that don't already
        /// carry one, so inserts can't silently land in the wrong (or no) tenant.
        /// Super-admin/no-request contexts must set InstituteId explicitly.
        /// </summary>
        private void StampTenant()
        {
            if (_bypassTenantFilter || _tenantId is not int tenantId)
            {
                return;
            }

            foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added && entry.Entity.InstituteId == 0)
                {
                    entry.Entity.InstituteId = tenantId;
                }
            }
        }
    }
}
