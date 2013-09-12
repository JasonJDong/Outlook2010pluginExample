using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class Utils
    {
        /// <summary>
        /// 有关HTTP请求的辅助类
        /// </summary>
        private const string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private const String KEY_64 = "Newegg.D";
        private const String IV_64 = "Newegg.J";

        public static String Object2Json(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static T Json2Object<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, String session)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            var request = WebRequest.Create(url) as HttpWebRequest;
            if (request != null)
            {
                request.Method = "GET";
                request.UserAgent = DefaultUserAgent;
                if (!string.IsNullOrEmpty(userAgent))
                {
                    request.UserAgent = userAgent;
                }
                if (timeout.HasValue)
                {
                    request.Timeout = timeout.Value;
                }
                if (!String.IsNullOrWhiteSpace(session))
                {
                    request.Headers.Add("Cookie", session);
                }
                return request.GetResponse() as HttpWebResponse;
            }
            return null;
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, object postData, int? timeout, string userAgent, Encoding requestEncoding, String session)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求
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
            request.Method = "POST";
            request.ContentType = "application/json";

            request.UserAgent = !string.IsNullOrEmpty(userAgent) ? userAgent : DefaultUserAgent;

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (!String.IsNullOrWhiteSpace(session))
            {
                request.Headers.Add("Cookie", session);
            }
            //如果需要POST数据
            if (postData != null)
            {
                var postJson = Object2Json(postData);
                var data = requestEncoding.GetBytes(postJson);
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

            }
            return request.GetResponse() as HttpWebResponse;
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, String postData, int? timeout, string userAgent, Encoding requestEncoding, String session)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求
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
            request.Method = "POST";
            request.ContentType = "application/json";

            request.UserAgent = !string.IsNullOrEmpty(userAgent) ? userAgent : DefaultUserAgent;

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (!String.IsNullOrWhiteSpace(session))
            {
                request.Headers.Add("Cookie", session);
            }
            //如果需要POST数据
            if (postData != null)
            {
                var data = requestEncoding.GetBytes(postData);
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

            }
            return request.GetResponse() as HttpWebResponse;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }

        public static String Encrypt(String source)
        {
            var byteKey = Encoding.ASCII.GetBytes(KEY_64);
            var byteIV = Encoding.ASCII.GetBytes(IV_64);

            var provider = new DESCryptoServiceProvider();
            using (var ms = new MemoryStream())
            {
                var cst = new CryptoStream(ms, provider.CreateEncryptor(byteKey, byteIV), CryptoStreamMode.Write);
                using (var sw = new StreamWriter(cst))
                {
                    sw.Write(source);
                    sw.Flush();
                    cst.FlushFinalBlock();
                    sw.Flush();
                    return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
                }
            }
        }

        public static String Decrypt(String encrypt)
        {
            var byteKey = Encoding.ASCII.GetBytes(KEY_64);
            var byteIV = Encoding.ASCII.GetBytes(IV_64);

            byte[] byteEncrypt = null;
            try
            {
                byteEncrypt = Convert.FromBase64String(encrypt);
            }
            catch (Exception)
            {
                return null;
            }

            var provider = new DESCryptoServiceProvider();
            using (var ms = new MemoryStream(byteEncrypt))
            {
                using (var cst = new CryptoStream(ms, provider.CreateDecryptor(byteKey, byteIV), CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cst))
                    {
                        return sr.ReadLine();
                    }
                }
            }
        }

        public static String GetCurrentUserSignature(bool html)
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var signatureFolderPath = Path.Combine(userProfile, "Microsoft\\Signatures");
            var signatureFolderInfo = new DirectoryInfo(signatureFolderPath);
            if (signatureFolderInfo.Exists)
            {
                var sg = signatureFolderInfo.GetFiles(html ? "*.htm" : "*.txt", SearchOption.TopDirectoryOnly);
                if (sg.Length > 0)
                {
                    var signature = sg[0];
                    using (var fs = signature.Open(FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(fs, Encoding.Default))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            return String.Empty;
        }
    }
}
