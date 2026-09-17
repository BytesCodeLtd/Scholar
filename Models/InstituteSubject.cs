using Scholar.Enums;

namespace Scholar.Models
{
    public class InstituteSubject : IAuditableEntity, ITenantEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public SubjectType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
