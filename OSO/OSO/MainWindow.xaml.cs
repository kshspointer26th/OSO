using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OSO
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        int day;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public MainWindow()
        {
            InitializeComponent();
            SendWpfWindowBack(this);
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            setTimeText();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            getMeal();
        }

        private string[] removeIndex(string[] aa, int index)
        {
            List<string> tmp = new List<string>(aa);
            tmp.RemoveAt(index);
            return tmp.ToArray();
        }

        private async Task<int> getMeal(int y, int m, int d)
        {
            try
            {
                string[] aa = mealAPI.getMealOfDay(y, m, d).Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                bool xx = false;

                aa = removeIndex(aa, 1);
                aa = removeIndex(aa, 2);
                this.mealList.Items.Clear();
                foreach (string a in aa)
                {
                    if (a.Contains("[") && xx)
                    {
                        mealList.Items.Add("");
                    }
                    if (xx == false) xx = true;

                    this.mealList.Items.Add(a);
                }

                day = getDay();
            }
            catch (Exception e)
            {
                return -1;
            }
            return 0;
        }

        private async Task<int> getMeal()
        {
            try
            {
                string[] aa = mealAPI.getMealOfDay(getYear(), getMonth(), getDay()).Split(new[] {
     Environment.NewLine
    }, StringSplitOptions.None);
                bool xx = false;

                aa = removeIndex(aa, 1);
                aa = removeIndex(aa, 2);
                this.mealList.Items.Clear();
                foreach (string a in aa)
                {
                    if (a.Contains("[") && xx)
                    {
                        mealList.Items.Add("");
                    }
                    if (xx == false) xx = true;

                    this.mealList.Items.Add(a);
                }

                day = getDay();
            }
            catch (Exception e)
            {
                return -1;
            }
            return 0;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (day != getDay()) getMeal();
            setTimeText();
        }

        public DateTime changeTime(DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
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

        private void setTimeText()
        {
            var now = DateTime.Now;
            DateTime end = DateTime.Now;

            var breakfast = changeTime(now, 7, 0, 0, 0);
            var lunch = changeTime(now, 12, 20, 0, 0);
            var dinner = changeTime(now, 18, 0, 0, 0);

            var breakfast_af = changeTime(now, 7, 20, 0, 0);
            var lunch_af = changeTime(now, 12, 40, 0, 0);
            var dinner_af = changeTime(now, 18, 20, 0, 0);

            if ((now >= breakfast && now <= breakfast_af) || (now >= lunch && now <= lunch_af) || (now >= dinner && now <= dinner_af))
            {
                this.timeText.Content = "배식시간임;;; 빨리 먹으러 가셈;;;";
            }
            else
            {
                if (now < breakfast)
                {
                    end = changeTime(now, 7, 0, 0, 0);
                }
                else if (now < lunch)
                {
                    end = changeTime(now, 12, 20, 0, 0);
                }
                else if (now < dinner)
                {
                    end = changeTime(now, 18, 0, 0, 0);
                }
                else
                {
                    end = new DateTime(
                     end.Year,
                     end.Month,
                     end.Day + 1,
                     7,
                     0,
                     0,
                     0,
                     end.Kind);

                }
                var result = end.Subtract(now).TotalSeconds;

                int h = (int)result / 60 / 60;
                int m = (int)result / 60 % 60;
                int s = (int)result % 60;

                if (h == 0)
                {
                    this.timeText.Content = "배식까지 남은 시간 : " + m + "분 " + s + "초";
                }
                else
                {
                    this.timeText.Content = "배식까지 남은 시간 : " + h + "시간 " + m + "분 " + s + "초";
                }
            }



        }

        private int getYear()
        {
            return Convert.ToInt32(DateTime.Now.ToString("yyyy"));
        }

        private int getMonth()
        {
            return Convert.ToInt32(DateTime.Now.ToString("MM"));
        }

        private int getDay()
        {
            return Convert.ToInt32(DateTime.Now.ToString("dd"));
        }

        private string getDate()
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /*
         * 프로그램 Draggable 가능하게 만듦 
         */

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();

            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
                this.Top = 0;
            }
        }

        /*
         * 바탕화면 고정 코드
         */

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(
         IntPtr hWnd,
         IntPtr hWndInsertAfter,
         int X,
         int Y,
         int cx,
         int cy,
         uint uFlags);

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            SendWpfWindowBack(Application.Current.MainWindow);
        }

        static void SendWpfWindowBack(Window window)
        {
            var hWnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
        }


        private void Button_Click_end(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("들어오는건 니맘이지만 나가는건 아니라네.. 껄껄껄");
        }

        private void Button_Click_f(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("허허.. 젊은이, 그런게 있다고 생각했단 말인가?");
        }

        private async void Button_Click_refresh(object sender, RoutedEventArgs e)
        {
            var t = getMeal();
            t.Wait();
            int x = t.Result;
            if (x == 0) System.Windows.MessageBox.Show("급식충;;;");
            else System.Windows.MessageBox.Show("프로그램 맛감");
        }

        private async void Button_Click_tommorow(object sender, RoutedEventArgs e)
        {
            var d = DateTime.Now;
            d = d.AddDays(1);
            var t = getMeal(d.Year, d.Month, d.Day);
            t.Wait();
            int x = t.Result;
            if (x == 0) System.Windows.MessageBox.Show("내일 급식인데? 오늘 못먹는데?");
            else System.Windows.MessageBox.Show("모르는게 약이여~ ㅉㅉ");
        }

        private void Button_Click_maker(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("26기 남정연, 이융희, 이현겸, 신원영");
        }
    }
}
