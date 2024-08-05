namespace gateway.api.Persistence.Entities
{
    public class Staff
    {
        public string Id { get; set; }
        public Guid AppUserId { get; set; }
        public string TenantId { get; set; }
    }
}
