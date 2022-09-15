using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        [StringLength(32)]
        public string IngredientName { get; set; }

        // Relationships
        public virtual ICollection<DrinkIngredient>? DrinkIngredients { get; set; }
    }
}
