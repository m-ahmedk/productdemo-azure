namespace ProductDemo.Models
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        // public string? CreatedBy { get; set; }
        // public string? LastModifiedBy { get; set; }
    }
}
