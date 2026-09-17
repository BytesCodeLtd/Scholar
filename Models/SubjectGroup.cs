namespace Scholar.Models
{
    public class SubjectGroup : IAuditableEntity, ITenantEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int ClassId { get; set; }

        public InstituteClass Class { get; set; } = null!;

        public ICollection<Section> Sections { get; set; } = new List<Section>();

        public ICollection<InstituteSubject> Subjects { get; set; } = new List<InstituteSubject>();

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
