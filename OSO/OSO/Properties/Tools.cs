using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSO.Properties
{
    public static class Tools {
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
    }
}
