using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    }
}
