using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaristaHome.Models
{
    public class Drink
    {
        public int DrinkId { get; set; }

        [StringLength(32), Display(Name = "Drink Name")]
        public string DrinkName { get; set; }

        [StringLength(1024), Display(Name = "Enter your instructions here")]
        public string Instructions { get; set; }

        [StringLength(512), Display(Name = "Description")]
        public string Description { get; set; }

        public string? DrinkVideo { get; set; }

        // Look into storing images with EF here: http://www.binaryintellect.net/articles/2f55345c-1fcb-4262-89f4-c4319f95c5bd.aspx
        public byte[]? DrinkImageData { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        // Relationships 
        public int StoreId { get; set; }
        public virtual Store? Store { get; set; }
        public virtual ICollection<DrinkIngredient>? DrinkIngredients { get; set; }
        public virtual ICollection<DrinkTag>? DrinkTags { get; set; }
        public virtual ICollection<Sale>? Sales { get; set; }



    }
}
