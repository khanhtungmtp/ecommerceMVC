using System.Text;

namespace ECommerceMVC.Helpers
{
    public class MyUtil
    {
        public static string GenereteRandomKey(int length = 5)
        {
            var pattern = @"dfdffewrhgfjghkjrtgfFDSHAQQSFHFTYUUI!";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }
        public static string UploadImage(IFormFile hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, hinh.FileName);
                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    hinh.CopyTo(myfile);
                }
                return hinh.FileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

    }

    
}
