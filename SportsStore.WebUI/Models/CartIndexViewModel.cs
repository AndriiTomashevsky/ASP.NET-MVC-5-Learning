using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Models
{
    public class CartIndxeViewModel
    {
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; }
    }
}