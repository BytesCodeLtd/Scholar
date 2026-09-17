namespace Scholar.Models
{
    /// <summary>
    /// A class section defined by an institute (e.g. "A", "Blue", "Morning").
    /// Institute-owned (tenant) data, so it carries an <see cref="InstituteId"/>
    /// and is isolated by the DbContext's tenant query filter.
    /// </summary>
    public class Section : IAuditableEntity, ITenantEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
