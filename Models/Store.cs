using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BaristaHome.Models
{
    public class Store
    {
        public int StoreId { get; set; }

        [StringLength(64), Display(Name = "Store Name")]
        public string StoreName { get; set; }

        [StringLength(5), Display(Name = "Store Invitation Code")]
        public string StoreInviteCode { get; set; }
    }
}
