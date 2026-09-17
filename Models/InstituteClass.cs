namespace Scholar.Models
{
    /// <summary>
    /// A class defined by an institute, belonging to a <see cref="Section"/>.
    /// Institute-owned (tenant) data; its <see cref="InstituteId"/> is derived from
    /// the parent section so the two always agree. Stored in the "Class" table.
    /// </summary>
    public class InstituteClass : IAuditableEntity, ITenantEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        public int SectionId { get; set; }

        public Section Section { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
