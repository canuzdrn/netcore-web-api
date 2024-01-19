namespace userMS.Domain.Entities.Common
{
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual TPrimaryKey CreatedBy { get; set; }
    }
}
