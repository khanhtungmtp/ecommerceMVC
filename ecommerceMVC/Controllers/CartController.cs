using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using ECommerceMVC.Helpers.Paypal;
namespace ECommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context _context;
        private readonly PaypalClient _paypalClient;

        public CartController(Hshop2023Context context, PaypalClient paypalClient)
        {
            _context = context;
            _paypalClient = paypalClient;
        }
        #region gio hang
        public List<CartItemVM> Cart
        {
            get { return HttpContext.Session.Get<List<CartItemVM>>(SystemContains.CART_KEY) ?? []; }
        }

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hangHoa = _context.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
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

        [Authorize]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
            ViewBag.PaymentClientId = _paypalClient.ClientId;
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM vM)
        {
            if (ModelState.IsValid)
            {
                var custommerID = HttpContext.User.Claims.SingleOrDefault(p => p.Type == SystemContains.CUSTOMMER_ID)?.Value;
                if (string.IsNullOrEmpty(custommerID))
                {
                    return View(Cart);
                }
                var khachHang = new KhachHang();
                if (vM.GiongKhachHang)
                {
                    khachHang = _context.KhachHangs.SingleOrDefault(x => x.MaKh == custommerID);
                }
                else
                {
                    if (string.IsNullOrEmpty(vM.DiaChi))
                    {
                        TempData["ErrorMessage"] = "Địa chỉ giao hàng không được bỏ trống";
                        return View(Cart);
                    }
                }

                if (khachHang is null)
                {
                    TempData["ErrorMessage"] = "Khách hàng không tồn tại";
                    return View(Cart);
                }

                var hoaDon = new HoaDon
                {
                    MaKh = custommerID,
                    HoTen = vM.HoTen ?? khachHang.HoTen,
                    DiaChi = vM.DiaChi ?? khachHang.DiaChi ?? "",
                    SoDienThoai = vM.DienThoai ?? khachHang?.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "Bưu điện",
                    GhiChu = vM.GhiChu
                };
                _context.Database.BeginTransaction();
                try
                {

                    _context.Add(hoaDon);
                    _context.SaveChanges();
                    var cthds = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoaDon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }
                    _context.AddRange(cthds);
                    _context.SaveChanges();
                    _context.Database.CommitTransaction();
                    // set session empty
                    HttpContext.Session.Set<List<CartItemVM>>(SystemContains.CART_KEY, []);
                    return View("Success");
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }
            }
            return View(Cart);
        }
        #endregion

        #region paypal payment
        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            // info send to paypal page
            string tongTien = Cart.Sum(p => p.ThanhTien).ToString();
            string donViTienTe = "USD";
            string maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();
            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new
                {
                    ex
                .GetBaseException().Message
                };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderId,CancellationToken cancellationToken)
        {
            try
            {
                CaptureOrderResponse response = await _paypalClient.CaptureOrder(orderId);
                // Lưu database đơn hàng của mình o buoc nay
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new
                {
                    ex
                .GetBaseException().Message
                };
                return BadRequest(error);
            }
        }

        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }
        #endregion
    }
}
