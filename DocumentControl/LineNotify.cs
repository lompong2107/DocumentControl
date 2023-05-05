using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;

namespace DocumentControl
{
    public class LineNotify
    {
        string lineToken = "jKpbqTO9GZ01E1yOw9AvxbULLn2WbXtLacotAoQPv0F";   // กล่ม Document Action Request
        string lineTokenDocumentControl = "xLRqFLQbhsRduD3fPhsbgM3JeC6cUigwD1zWjGdQSbJ";   // กล่ม แจกจ่ายเอกสารควบคุม
        public void Notify(String message)
        {
            message = System.Web.HttpUtility.UrlEncode(message, Encoding.UTF8);
            var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
            var postData = string.Format("message={0}", message);
            var data = Encoding.UTF8.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Headers.Add("Authorization", "Bearer " + lineToken);
            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public void NotifyDocumentControl(String message)
        {
            message = System.Web.HttpUtility.UrlEncode(message, Encoding.UTF8);
            var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
            var postData = string.Format("message={0}", message);
            var data = Encoding.UTF8.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Headers.Add("Authorization", "Bearer " + lineTokenDocumentControl);
            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
    }
}