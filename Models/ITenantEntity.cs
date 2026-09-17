namespace Scholar.Models
{
    /// <summary>
    /// Marks an entity as owned by a single institute (tenant). The DbContext
    /// applies an automatic query filter on <see cref="InstituteId"/> and stamps
    /// it on insert, so tenant isolation doesn't depend on every query
    /// remembering to filter by hand.
    /// </summary>
    public interface ITenantEntity
    {
        int InstituteId { get; set; }
    }
}
