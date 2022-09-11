namespace BaristaHome.Models
{
    public class DrinkIngredient
    {
        public int DrinkId { get; set; }
        public int IngredientId { get; set; }

        // Relationships
        public virtual Drink Drink { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
