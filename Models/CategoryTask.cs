namespace BaristaHome.Models
{
    public class CategoryTask
    {
        public int CategoryId { get; set; }
        public int StoreTaskId { get; set; }
        public bool IsFinished { get; set; }

        // Relationships
        public virtual Category Category { get; set; }
        public virtual StoreTask StoreTask { get; set; }
    }
}
