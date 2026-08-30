using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Scholar.Models;

namespace Scholar.Data
{
    public class ScholarDbContext : IdentityDbContext<ApplicationUser>
    {
        public ScholarDbContext(DbContextOptions<ScholarDbContext> options)
            : base(options)
        {
        }

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
        public DbSet<TestSettings> TestSettings => Set<TestSettings>();
        public DbSet<PastPaper> PastPapers => Set<PastPaper>();
        public DbSet<TestSection> TestSections => Set<TestSection>();

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
                .HasIndex(s => s.InstituteId);

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
        }
    }
}
