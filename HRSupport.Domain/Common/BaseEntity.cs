namespace HRSupport.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public bool Isactive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }

}