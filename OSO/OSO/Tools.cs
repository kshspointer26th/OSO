using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OSO
{
    public static class Tools
    {
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

        public async static Task<string> getOnlineVersion()
        {
            string html = string.Empty;
            string url = latestUpdate;

            await Task.Factory.StartNew(() =>
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.UserAgent = "OSO";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
            });

            return html.Split(new string[] { "\"tag_name\":\"" }, StringSplitOptions.None)[1].Split(new string[] { "\"," }, StringSplitOptions.None)[0];
        }

        public static string getLocalVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async static Task UpdateProcess(string version)
        {
            await Task.Factory.StartNew(() =>
            {
                string fullpath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string dirpath = System.IO.Path.GetDirectoryName(fullpath);
                string filename = System.IO.Path.GetFileName(fullpath);
                string filename_next = System.IO.Path.GetFileNameWithoutExtension(fullpath);
                string dpath = $"https://github.com/kshspointer26th/OSO/releases/download/{version}/OSO.exe";

                string originalpath = System.IO.Path.Combine(dirpath, filename);
                string newpath = System.IO.Path.Combine(dirpath, $"{filename_next}_{RandomString(8)}.exe");

                while (File.Exists(newpath))
                {
                    newpath = System.IO.Path.Combine(dirpath, $"{filename_next}{RandomString(8)}.exe");
                }

                System.IO.File.Move(originalpath, newpath);

                System.IO.File.Copy(newpath, originalpath);

                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        wc.DownloadFile(new Uri(dpath), originalpath);
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }
                System.Windows.MessageBox.Show("업데이트를 완료했습니다. 프로그램을 재시작합니다.");
                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = originalpath;
                p.StartInfo.Arguments = $"-oldfile {newpath}";
                p.Start();
                Environment.Exit(0);
            });
        }
    }
}
