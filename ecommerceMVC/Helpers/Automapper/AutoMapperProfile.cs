using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;

namespace ECommerceMVC.Helpers.Automapper
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile()
        {
           CreateMap<RegisterVM, KhachHang>();
        }
    }
}
