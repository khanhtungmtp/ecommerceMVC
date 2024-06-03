using ecommerceMVC.Data;
using ecommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ecommerceMVC.Helpers;
namespace ecommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context _context;

        public CartController(Hshop2023Context context)
        {
            _context = context;
        }

        public List<CartItemVM> Cart
        {
            get { return HttpContext.Session.Get<List<CartItemVM>>(SystemContains.CART_KEY) ?? []; }
        }

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id,int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p=>p.MaHh ==id);
            if (item == null)
            {
                var hangHoa = _context.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if(hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hoá có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItemVM
                {
                    MaHh = hangHoa.MaHh,
                    TenHH = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? "",
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(SystemContains.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item is not null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(SystemContains.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }
    }
}
