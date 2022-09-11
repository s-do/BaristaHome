namespace BaristaHome.Models
{
    public class DrinkTag
    {
        public int DrinkId { get; set; }
        public int TagId { get; set; }

        // Relationships
        public virtual Drink Drink { get; set; }
        public virtual Tag Tag{ get; set; }
    }
}
