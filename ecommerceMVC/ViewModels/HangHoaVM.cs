namespace ECommerceMVC.ViewModels
{
    public class HangHoaVM
    {
        public int MaHh { get; set; }
        public string TenHH { get; set; } = string.Empty;
        public string Hinh { get; set; } = string.Empty;
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; } = string.Empty;
        public string TenLoai { get; set; } = string.Empty;
    }

    public class ChiTietHangHoaVM
    {
        public int MaHh { get; set; }
        public string TenHH { get; set; } = string.Empty;
        public string Hinh { get; set; } = string.Empty;
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; } = string.Empty;
        public string TenLoai { get; set; } = string.Empty;
        public string ChiTiet { get; set; } = string.Empty;
        public int DiemDanhGia  { get; set; }
        public int SoLuongTon  { get; set; }
    }
}
