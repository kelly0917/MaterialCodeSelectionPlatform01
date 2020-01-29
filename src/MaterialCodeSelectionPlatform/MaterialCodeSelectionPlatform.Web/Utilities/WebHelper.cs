using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CommodityCodeSelectionPlatform.Web.Common
{
    public class WebHelper
    {
        /// <summary>
        /// 根据url获取数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string RequestUrl(string url)
        {
            HttpWebRequest request = CreateWebRequest(url);

            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            string retString = string.Empty;//定义返回内容
            request.Method = "GET";
            request.Timeout = 600000;

            //request.ContentType = "application/x-www-form-urlencoded";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream myResponseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8))
                {
                    retString = myStreamReader.ReadToEnd();
                }
            }
            return retString;
        }


        /// <summary>
        /// 根据url获取数据，带Token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string RequestUrl(string url, string token)
        {
            HttpWebRequest request = CreateWebRequest(url);

            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            string retString = string.Empty;//定义返回内容
            request.Method = "GET";
            request.Timeout = 600000;
            request.Headers.Add("Authorization", "Bearer "+ token);
            //request.ContentType = "application/x-www-form-urlencoded";
            Encoding encoding = Encoding.UTF8;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream myResponseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8))
                {
                    retString = myStreamReader.ReadToEnd();
                }
            }
            return retString;
        }

        /// <summary>
        /// 创建请求对象
        /// </summary>
        /// <param name="url">请求url</param>
        /// <returns></returns>
        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest request = null;// (HttpWebRequest)WebRequest.Create(url);
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            return request;
        }

        /// <summary>
        /// https证书验证回调 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受 
        }

    }
}