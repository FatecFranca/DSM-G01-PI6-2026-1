namespace Sleep.Domain.Entities
{
    public abstract class EntityBase
    {
        public long Id { get; set; }
        public DateTime Created_On {  get; set; } = DateTime.UtcNow;
        public DateTime Last_Updated {  get; set; }

        public void MarkAsUpdated()
        {
            Last_Updated = DateTime.UtcNow;
        }
    }
}
