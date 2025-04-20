using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bulky.Models.ViewModels
{
    public class ShoppingCartVm
    {
        public IEnumerable<ShopingCart> ShoppingCartList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    
    }
}
