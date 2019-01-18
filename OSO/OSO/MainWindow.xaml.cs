using OSO.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private static DateTime stDate = DateTime.Now;

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

            day = stDate.Day;

            try
            {
                getMeal(stDate);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("프로그램에 일시적 오류가 발생하였습니다. 문제가 반복되면 보고하십시오.");
            }
        }

        private async Task getMeal(DateTime dateTime)
        {
            int y = dateTime.Year;
            int m = dateTime.Month;
            int d = dateTime.Day;
            string[] aa = mealAPI.getMealOfDay(y, m, d).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            bool xx = false;
            aa = Tools.RemoveIndex(aa, 0);
            aa = Tools.RemoveIndex(aa, 1);
            aa = Tools.RemoveIndex(aa, 2);
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

            this.dateText.Content = String.Format("[{0}년 {1}월 {2}일 급식표 표시중]", y, m, d);
        }

        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (day != DateTime.Now.Day)
            {
                stDate = DateTime.Now;
                try
                {
                    await getMeal(stDate);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("프로그램에 일시적 오류가 발생하였습니다. 문제가 반복되면 보고하십시오.");
                }
            }
            setTimeText();
        }

        private void setTimeText()
        {
            var now = DateTime.Now;
            DateTime end = DateTime.Now;

            var breakfast = Tools.ChangeTime(now, 7, 0, 0, 0);
            var lunch = Tools.ChangeTime(now, 12, 20, 0, 0);
            var dinner = Tools.ChangeTime(now, 18, 0, 0, 0);

            var breakfast_af = Tools.ChangeTime(now, 7, 20, 0, 0);
            var lunch_af = Tools.ChangeTime(now, 12, 40, 0, 0);
            var dinner_af = Tools.ChangeTime(now, 18, 20, 0, 0);

            if ((now >= breakfast && now <= breakfast_af) || (now >= lunch && now <= lunch_af) || (now >= dinner && now <= dinner_af))
            {
                this.timeText.Content = "배식 시간 입니다";
            }
            else
            {
                if (now < breakfast)
                {
                    end = Tools.ChangeTime(now, 7, 0, 0, 0);
                }
                else if (now < lunch)
                {
                    end = Tools.ChangeTime(now, 12, 20, 0, 0);
                }
                else if (now < dinner)
                {
                    end = Tools.ChangeTime(now, 18, 0, 0, 0);
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

        private async void todayButton_Click(object sender, RoutedEventArgs e)
        {
            stDate = DateTime.Now;
            try
            {
                await getMeal(stDate);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("프로그램에 일시적 오류가 발생하였습니다. 문제가 반복되면 보고하십시오.");
            };
        }

        private void programInformation_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("26기 남정연, 이융희, 이현겸, 신원영");
        }

        private void checkForUpdates_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void nextDay_Click(object sender, RoutedEventArgs e)
        {
            stDate = stDate.AddDays(1);
            try
            {
                await getMeal(stDate);
            } catch (Exception ex)
            {
                System.Windows.MessageBox.Show("프로그램에 일시적 오류가 발생하였습니다. 문제가 반복되면 보고하십시오.");
            }
            
        }

        private async void beforeDay_Click(object sender, RoutedEventArgs e)
        {
            stDate = stDate.AddDays(-1);
            try
            {
                await getMeal(stDate);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("프로그램에 일시적 오류가 발생하였습니다. 문제가 반복되면 보고하십시오.");
            }
        }

        private void adjustTransparency_Click(object sender, RoutedEventArgs e)
        {
            var transparencyAdjustDialog = new TransparencyAdjustDialog();
            transparencyAdjustDialog.Owner = Window.GetWindow(this);
            transparencyAdjustDialog.Show();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("종료하시겠습니까?", "OSO", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
