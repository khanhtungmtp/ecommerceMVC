using ecommerceMVC.Helpers;
using ecommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceMVC.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItemVM>>(SystemContains.CART_KEY) ?? [];
            return View("CartPanel", new CartVM
            {
                Quantity = cart.Sum(p => p.SoLuong),
                Total = cart.Sum(p => p.ThanhTien),
            });
        }
    }
}
