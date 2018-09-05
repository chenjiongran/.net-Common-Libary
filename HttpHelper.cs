using System;
using System.Net;
using System.Collections;
using System.IO;
using System.Text;

namespace CommonLayer
{
    public class HttpHelper
    {

        /// <summary>
        /// 通过get方式请求页面，传递一个实例化的cookieContainer
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="Referer"></param>
        /// <returns></returns>
        public static ArrayList GetHtmlData(string postUrl, string referer)
        {
            CookieContainer cookie = new CookieContainer();
            return GetHtmlData(postUrl, referer, cookie);
        }
        /// <summary>
        /// 通过get方式请求页面，传递一个实例化的cookieContainer
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="Referer"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ArrayList GetHtmlData(string url, string referer, CookieContainer cookie)
        {
            return GetHtmlData(url, referer, cookie, null);
        }
        /// <summary>
        /// 通过get方式请求页面，传递一个实例化的cookieContainer
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="Referer"></param>
        /// <param name="cookie"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>

        public static ArrayList GetHtmlData(string url, string referer, CookieContainer cookie, WebProxy proxy)
        {
            return GetHtmlData(url, referer, cookie, proxy, Encoding.Default);
        }
        /// <summary>
        /// 通过get方式请求页面，传递一个实例化的cookieContainer
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ArrayList GetHtmlData(string url, string referer, CookieContainer cookie, WebProxy proxy, Encoding encoding)
        {
            HttpWebRequestPara reqPara = new HttpWebRequestPara();
            reqPara.postUrl = url;
            reqPara.referer = referer;
            reqPara.cookie = cookie;
            reqPara.proxy = proxy;
            reqPara.encoding = encoding;
            return GetHtmlData(reqPara);
        }

        public static ArrayList GetHtmlData(HttpWebRequestPara reqPara)
        {
            reqPara.Method = "GET";
            return HttpData(reqPara);
           
        }
        /// <summary>
        /// 发送POST请求 进行登录操作 并保存cookie
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="postUrl"></param>
        /// <param name="Referer"></param>
        /// <returns></returns>
        public static ArrayList PostData(string postData, string postUrl, string referer)
        {
            CookieContainer cookie = (CookieContainer)System.Web.HttpContext.Current.Session["CookieContainer"];
            return PostData(postData, postUrl, referer, cookie);
        }
        /// <summary>
        /// 发送POST请求 进行登录操作 并保存cookie
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="postUrl"></param>
        /// <param name="Referer"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ArrayList PostData(string postData, string postUrl, string referer, CookieContainer cookie)
        {
            return PostData(postData, postUrl, referer, cookie, null, Encoding.UTF8);
        }

        public static ArrayList PostData(string postData, string postUrl, string referer, CookieContainer cookie, WebProxy proxy, Encoding encoding)
        {
            HttpWebRequestPara p = new HttpWebRequestPara();
            p.postData = postData;
            p.postUrl = postUrl;
            p.referer = referer;
            p.ContentType = null;
            p.cookie = cookie;
            p.proxy = proxy;
            p.encoding = encoding;

            return PostData(p);
        }

        /// <summary>
        /// 发送POST请求 进行登录操作 并保存cookie
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="postUrl"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ArrayList PostData(HttpWebRequestPara reqPara)
        {
            reqPara.Method = "POST";
            return HttpData(reqPara);
        }

        public static ArrayList HttpData(HttpWebRequestPara reqPara)
        {
            Encoding encoding = reqPara.encoding ?? Encoding.UTF8;
            ArrayList list = new ArrayList();
            HttpWebRequest request;
            HttpWebResponse response;
            request = WebRequest.Create(reqPara.postUrl) as HttpWebRequest;
            request.Accept = reqPara.Accept ?? "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/QVOD, application/QVOD, */*";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0)";
            request.ContentType = reqPara.ContentType ?? "application/x-www-form-urlencoded";
            if (reqPara.referer != "")
                request.Referer = reqPara.referer;
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            if (!String.IsNullOrWhiteSpace(reqPara.Origin))
                request.Headers.Add("Origin", reqPara.Origin);
            if (!String.IsNullOrWhiteSpace(reqPara.Host))
                request.Host = reqPara.Host;
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.Method = reqPara.Method == null ? "POST" : reqPara.Method.ToUpper();
            request.CookieContainer = reqPara.cookie;
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (reqPara.proxy != null)
            {
                request.Proxy = reqPara.proxy;
            }
            if (reqPara.postData != null)
            {
                byte[] b = encoding.GetBytes(reqPara.postData);
                request.ContentLength = b.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(b, 0, b.Length);
                }
            }
            try
            {
                //获取服务器返回的资源
                using (response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                    {
                        if (response.Cookies.Count > 0)
                            reqPara.cookie.Add(response.Cookies);

                        list.Add(reader.ReadToEnd());
                        list.Add(reqPara.cookie);
                    }
                }
            }
            catch (Exception wex)
            {
                list.Add("发生异常/n/r" + wex.Message);

            }
            finally
            {
                request.Abort();
            }
            return list;
        }
        public class HttpWebRequestPara:ICloneable
        {

            public string postData;
            public string postUrl;
            /// <summary>
            /// 可为null
            /// </summary>
            public string referer;
            /// <summary>
            /// 可为null
            /// </summary>
            public string ContentType;
            public CookieContainer cookie;
            /// <summary>
            /// 可为null
            /// </summary>
            public WebProxy proxy;
            /// <summary>
            /// 可为null
            /// </summary>
            public Encoding encoding;
            /// <summary>
            /// 可为null
            /// </summary>
            public string Accept;
            /// <summary>
            /// 可为null
            /// </summary>
            public string Method;
            /// <summary>
            /// 可为null
            /// </summary>
            public string Origin;

            /// <summary>
            /// 可为null
            /// </summary>
            public string Host;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }
    }
}