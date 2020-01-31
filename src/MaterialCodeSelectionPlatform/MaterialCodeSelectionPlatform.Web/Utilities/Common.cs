using System;
using System.Security.Cryptography;
using System.Text;

namespace MaterialCodeSelectionPlatform.Web.Utilities
{
    public class CommonHelper
    {
        public static string ToMD5(string str)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}