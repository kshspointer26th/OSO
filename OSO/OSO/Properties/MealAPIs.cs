using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Web;


//Source: https://github.com/asm6788/SchoolLunchAPI
public class meal
{
    public int day_of = 0;
    public string menu = "";


    public meal(int day_of, string menu)
    {
        this.day_of = day_of;
        this.menu = menu;
    }
    public meal() { }
}

public class mealAPI
{
    private static string getURL(int year, int month)
    {

        string url = string.Empty;

        StringBuilder targetUrl = new StringBuilder("http://" + "stu.kwe.go.kr" + "/" + "sts_sci_md00_001.do");
        targetUrl.Append("?");
        targetUrl.Append("schulCode=" + "K100000365" + "&");
        targetUrl.Append("schulCrseScCode=4&");
        targetUrl.Append("schulKndScCode=04&");
        targetUrl.Append("schYm=" + year + string.Format("{0:00}", month) + "&");

        url = targetUrl.ToString();
        return url;

        //https://stu.kwe.go.kr/sts_sci_md00_001.do?schulCode=K100000365&schulCrseScCode=4&schulKndScCode=04&schYm=2019.1&
        //2019년 1월
        //조회버튼 눌러야되는데
    }

    private static string getMealInfo(int y, int n)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getURL(y, n));
        request.Method = "GET";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stReadData = response.GetResponseStream();
        StreamReader srReadData = new StreamReader(stReadData, Encoding.UTF8);
        string strResult = srReadData.ReadToEnd();

        return strResult;
    }

    private static int CheckDigit(string input)
    {
        bool first = false;
        bool twice = false;
        if (Regex.IsMatch(input.Substring(input.Length - 1), @"^\d+$"))
        {
            first = true;

        }
        if (input.Length >= 2)
        {
            if (Regex.IsMatch(input.Substring(input.Length - 2), @"^\d+$"))
            {
                twice = true;

            }
        }
        if (first && twice)
        {
            return 2;
        }
        else if (first)
        {
            return 1;
        }
        return 0;

    }

    private static bool CheckNumber(string letter)
    {
        bool IsCheck = true;

        Regex numRegex = new Regex(@"[0-9]");
        Boolean ismatch = numRegex.IsMatch(letter);

        if (!ismatch)
        {
            IsCheck = false;
        }

        return IsCheck;
    }

    private static List<meal> test(string input)
    {

        List<meal> result = new List<meal>();

        string htmlCode = input;


        htmlCode = htmlCode.Remove(0, htmlCode.IndexOf("tbody"));
        //  Console.WriteLine(htmlCode);
        htmlCode = htmlCode.Remove(htmlCode.IndexOf("/tbody"));
        htmlCode = htmlCode.Replace("\t", "");
        htmlCode = htmlCode.Replace("\r\n", "");
        htmlCode = htmlCode.Replace("<td><div>", ":");
        // htmlCode = htmlCode.Replace("<br />", "");
        htmlCode = htmlCode.Replace("</div></td>", "");
        htmlCode = htmlCode.Replace(@"<td class=""last""><div>", "");
        htmlCode = htmlCode.Replace("t", "");
        htmlCode = htmlCode.Replace("ody", "");

        int day_of = 0;
        List<meal> NaeYong = new List<meal>();

        string[] arr = htmlCode.Split("<br />".ToCharArray()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

        for (int i = 1; i < arr.Length; i++)
        {
            if (CheckDigit(arr[i - 1]) != 0)
            {
                if (arr[i - 1].Remove(0, arr[i - 1].Length - CheckDigit(arr[i - 1])) != "")
                {
                    day_of = Convert.ToInt32(arr[i - 1].Remove(0, arr[i - 1].Length - CheckDigit(arr[i - 1])));
                    arr[i - 1] = arr[i - 1].Remove(arr[i - 1].Length - CheckDigit(arr[i - 1]), CheckDigit(arr[i - 1]));
                }
                NaeYong.Add(new meal(day_of, arr[i - 1]));
            }
            else
            {
                NaeYong.Add(new meal(day_of, arr[i - 1]));
            }
        }

        meal temp = new meal();
        string temp2 = "";

        for (int i = 0; i < NaeYong.Count; i++)
        {
            if (i == 0)
            {
                temp = NaeYong[i];
            }
            if (NaeYong[i].day_of == temp.day_of)
            {
                temp2 = temp2 + "\r\n" + NaeYong[i].menu;
            }
            else
            {
                if (NaeYong[i].menu.Length > 0 && temp2.Length - 1 > 0 && temp2.Length - 2 > 0)
                {
                    if (NaeYong[i].menu[0] != ':')
                    {
                        temp2 = temp2 + "\r\n" + NaeYong[i].menu.Remove(NaeYong[i].menu.Length - 1, 1);
                    }
                    if (CheckNumber(temp2[temp2.Length - 1].ToString()) && CheckNumber(temp2[temp2.Length - 2].ToString()))
                    {
                        temp2 = temp2.Remove(temp2.Length - 2, 2);
                    }
                    else if (CheckNumber(temp2[temp2.Length - 1].ToString()))
                    {
                        temp2 = temp2.Remove(temp2.Length - 1, 1);
                    }
                }
                result.Add(new meal(temp.day_of, temp2.Replace(":", "")));
                temp2 = "";

                temp = NaeYong[i];
            }
        }
        result.Add(new meal(temp.day_of, temp2));
        result = result.Where(s => !string.IsNullOrWhiteSpace(s.menu)).Distinct().ToList();
        result.RemoveAll(x => x.day_of < 1);
        result.RemoveAll(x => x.day_of > 31);

        return result;


    }

    /**
       찾는데 실패시 String.Empty 반환한다
    */
    public static string getMealOfDay(int year, int month, int day)
    {
        List<meal> list = test(getMealInfo(year, month));
        foreach (meal r in list)
        {
            if (r.day_of == day)
            {
                return r.menu;
            }
        }
        return String.Empty;
    }

    public static string getMealOfMonth(int year, int month)
    {
        List<meal> list = test(getMealInfo(year, month));
        StringBuilder strb = new StringBuilder(String.Empty);
        foreach (meal r in list)
        {
            strb.Append(year + "." + month + "." + r.day_of);
            strb.Append("\n");
            strb.Append(r.menu);
        }
        return strb.ToString();
    }

}

public class mealCropper
{


    static string[] arr = {
  "[조식]",
  "[중식]",
  "[석식]"
 };
    static bool debug = false;

    private static int foobar(string str)
    {

        if (debug) Console.WriteLine("------" + str);

        for (int i = 1; i <= 3; i++)
        {
            if (str.Trim(null).Equals(arr[i - 1]))
            {
                if (debug) Console.WriteLine("--" + i);
                return i;
            }
        }
        return 0;
    }

    /**
       typehelp
       0 = all
       1 = 조식
       2 = 중식
       3 = 석식
    */


    public static List<string> getMeals(string daydata, int type)
    {

        string[] temp = daydata.Split(new char[] {
   '\n'
  });
        int[] pp = new int[4] {
   0,
   0,
   0,
   0
  };
        pp[3] = temp.Length;
        for (int i = 0; i < temp.Length; i++)
        {
            int foo = foobar(temp[i]);
            if (foo != 0) pp[foo - 1] = i;
        }

        List<string> list = new List<string>();

        if (debug)
        {
            foreach (int i in pp)
            {
                Console.WriteLine(i);
            }
        }

        switch (type)
        {
            case 0:
                if (debug) Console.WriteLine($"{pp[0]}, {pp[3]}");
                for (int i = pp[0]; i < pp[3]; i++)
                {
                    list.Add(temp[i]);
                }
                break;

            default:
                if (debug) Console.WriteLine($"{pp[type - 1] + 1}, {pp[type]}");
                for (int i = pp[type - 1] + 1; i < pp[type]; i++)
                {
                    list.Add(temp[i]);
                }
                break;
        }
        return list;




    }

    private static string getRidOfNums(string str)
    {
        StringBuilder strb = new StringBuilder(String.Empty);
        char[] arr = str.ToCharArray();

        for (int i = 0; i < arr.Length; i++)
        {
            if ('0' <= arr[i] && arr[i] <= '9') continue;
            if (arr[i] == '.') continue;
            strb.Append(new char[] {
    arr[i]
   });

        }

        return strb.ToString();
    }


    public static List<string> getMeals_T(string daydata, int type)
    {
        List<string> list = getMeals(daydata, type);
        List<string> temp = new List<string>();

        foreach (string str in list)
        {
            temp.Add(getRidOfNums(str));
        }
        return temp;
    }

}