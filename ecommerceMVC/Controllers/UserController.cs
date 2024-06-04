using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly Hshop2023Context _context;
        private readonly IMapper _imapper;

        public UserController(Hshop2023Context context, IMapper mapper)
        {
            _context = context;
            _imapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM vM, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var user = _imapper.Map<KhachHang>(vM);
                user.RandomKey = MyUtil.GenereteRandomKey();
                user.MatKhau = user.MatKhau?.ToMd5Hash(user.RandomKey);
                user.HieuLuc = true; // temp
                user.VaiTro = 0;
                if (image != null)
                {
                    user.Hinh = MyUtil.UploadImage(image, "KhachHang");
                }
                _context.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index", "HangHoa");
            }
            return View();
        }

        #region Login
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var user = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == vm.UserName);
                if (user is null)
                {
                    ModelState.AddModelError("Error", "Sai thông tin đăng nhập");
                    return View();
                }

                if (!user.HieuLuc)
                {
                    ModelState.AddModelError("Error", "Tài khoản bị khoá, Vui lòng liên hệ admin");
                    return View();
                }

                if(user.MatKhau != vm.Password.ToMd5Hash(user.RandomKey))
                {
                    ModelState.AddModelError("Error", "Sai thông tin đăng nhập");
                    return View();
                }

                var clainms = new List<Claim>
                {
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Name, user.HoTen),
                    new("CustomerId", user.MaKh),
                    // claims -role động
                    new(ClaimTypes.Role, "Customer")
                };

                var claimsIdentity = new ClaimsIdentity(clainms, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }else {
                    return Redirect("/");
                }

            }
            return View();
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }
        
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [HttpPost]
        public IActionResult Remember()
        {
            return View();
        }
    }
}
