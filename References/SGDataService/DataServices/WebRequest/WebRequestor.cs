using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SGDataService.DataServices.WebRequest
{
    public class WebRequestor
    {
        public enum RequestMethod
        {
            get, post
        }
        public static string Send(string url, IDictionary<string, string> parameters, RequestMethod method, Encoding encoding = null)
        {
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;
            System.IO.Stream reqStream = null;
            string response = null;

            switch (method)
            {
                case RequestMethod.post:
                    req = (HttpWebRequest)System.Net.WebRequest.Create(url);
                    req.Method = RequestMethod.post.ToString();
                    req.KeepAlive = false;
                    req.ProtocolVersion = HttpVersion.Version10;
                    req.Timeout = 5000;
                    req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));

                    using (reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(postData, 0, postData.Length);

                        using (rsp = (HttpWebResponse)req.GetResponse())
                        {
                            response = GetResponseAsString(rsp, encoding == null ? Encoding.GetEncoding(rsp.CharacterSet) : encoding);
                        }
                    }
                    break;
                case RequestMethod.get:
                    req = (HttpWebRequest)System.Net.WebRequest.Create(url + "?" + BuildQuery(parameters, "utf8"));
                    req.Method = RequestMethod.get.ToString();
                    req.ReadWriteTimeout = 5000;
                    req.ContentType = "text/html;charset=UTF-8";
                    using (rsp = (HttpWebResponse)req.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(rsp.GetResponseStream(), encoding == null ? Encoding.GetEncoding("utf-8") : encoding))
                        {
                            response = sr.ReadToEnd();
                        }
                    }
                    break;
            }
            return response;
        }
        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        private static string BuildQuery(IDictionary<string, string> parameters, string encode)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;
            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name))//&& !string.IsNullOrEmpty(value)
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }
                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(value);
                    //if (encode == "gb2312")
                    //{
                    //    postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                    //}
                    //else if (encode == "utf8")
                    //{
                    //    postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    //}
                    //else
                    //{
                    //    postData.Append(value);
                    //}
                    hasParam = true;
                }
            }
            return postData.ToString();
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            string result = null;
            using (Stream stream = rsp.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

    }
}
