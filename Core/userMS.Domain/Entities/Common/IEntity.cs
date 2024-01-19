namespace userMS.Domain.Entities.Common
{
    public interface IEntity<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }

        public DateTime CreatedAt { get; }

        public TPrimaryKey CreatedBy { get; }
    }
}
