using Domain.Abstract;

namespace Domain.Abstract
{
    public enum Status
    {
        Active = 1,
        Passive = 2
    }
    public abstract class BaseEntity : IBaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? CreatedUsername { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUsername { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedUsername { get; set; }
        public Status Status { get; set; } = Status.Active;
    }
}