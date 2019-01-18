using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OSO
{
    /// <summary>
    /// TransparencyAdjustDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TransparencyAdjustDialog : Window
    {
        double originalTransparency;
        bool change = false;

        private static MainWindow mainWindow;

        public TransparencyAdjustDialog()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                mainWindow = this.Owner as MainWindow;
                originalTransparency = mainWindow.wrapper.Opacity;
                this.transparencySlider.Value = mainWindow.wrapper.Opacity * 100;
                this.setTransparencyButton.Content = String.Format("투명도 {0}(으)로 설정", (int)transparencySlider.Value);
            };
        }

        private void setTransparency_Click(object sender, RoutedEventArgs e)
        {
            change = true;
            mainWindow.wrapper.Opacity = this.transparencySlider.Value/100;
            this.Close();
        }

        private void transparencySlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            this.setTransparencyButton.Content = String.Format("투명도 {0}(으)로 설정", (int)transparencySlider.Value);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!change) mainWindow.wrapper.Opacity = originalTransparency;
        }

        private void transparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.setTransparencyButton.Content = String.Format("투명도 {0}(으)로 설정", (int)transparencySlider.Value);
            mainWindow.wrapper.Opacity = this.transparencySlider.Value / 100;
        }
    }
}
