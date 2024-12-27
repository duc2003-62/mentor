using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace mentor.Utilities
{
    public class Functions
    {
        public static int _MaNguoiDung = 0;
        public static string _TenTaiKhoan = string.Empty;
        public static string _Email = string.Empty;
        public static string _SoDienThoai = string.Empty;
        public static string _Message =  string.Empty;
        public static string TitleSlugGeneration(string type, string? title, long id)
        {
            return type + "-" + SlugGenerator.SlugGenerator.GenerateSlug(title) + "-" + id.ToString() + ".html";
        }
        public static string getCurrentDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string MD5Hash (string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for(int i=0;i<result.Length; i++)
               strBuilder.Append(result[i].ToString("x2"));
            return strBuilder.ToString();
        }
        public static string MD5Password(string? text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text), "Mật khẩu không được để trống.");

            string str = MD5Hash(text);
            for (int i = 0; i <= 5; i++)
                str = MD5Hash(str + str);
            return str;
        }

    }
}