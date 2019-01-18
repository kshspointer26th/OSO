using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OSO.Properties
{
    public static class Tools {
        private static readonly string latestUpdate = "https://api.github.com/repos/kshspointer26th/OSO/releases/latest";
        public static DateTime ChangeTime(DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
             dateTime.Year,
             dateTime.Month,
             dateTime.Day,
             hours,
             minutes,
             seconds,
             milliseconds,
             dateTime.Kind);
        }

        public static string[] RemoveIndex(string[] aa, int index)
        {
            List<string> tmp = new List<string>(aa);
            tmp.RemoveAt(index);
            return tmp.ToArray();
        }

        public static string getOnlineVersion()
        {
            string html = string.Empty;
            string url = latestUpdate;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.UserAgent = "OSO";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            return html.Split(new string[] {"\"tag_name\":\""}, StringSplitOptions.None)[1].Split(new string[] { "\"," }, StringSplitOptions.None)[0];
        }

        public static string getLocalVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
