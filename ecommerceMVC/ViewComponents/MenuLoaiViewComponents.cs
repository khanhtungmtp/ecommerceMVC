using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.ViewComponents
{
    public class MenuLoaiViewComponent :ViewComponent
    {
        private readonly Hshop2023Context _context;

        public MenuLoaiViewComponent(Hshop2023Context context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var data = _context.Loais.Select(l => new MenuLoaiVM
            {
               MaLoai = l.MaLoai,
              TenLoai =  l.TenLoai,
              SoLuong =  l.HangHoas.Count
            }).OrderBy(p=>p.TenLoai);
            return View(data); // default.cshtml
        }
    }
}
