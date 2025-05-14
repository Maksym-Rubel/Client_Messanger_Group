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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_Messanger
{
    /// <summary>
    /// Interaction logic for ChoiceLogRegPage.xaml
    /// </summary>
    public partial class ChoiceLogRegPage : Page
    {
        public ChoiceLogRegPage()
        {
            InitializeComponent();
            myImage.Source = new BitmapImage(new Uri("pack://application:,,,/images/telegram.png"));
        }

        private void TxtBox_Lost(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Enter password";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void TxtBox_Got(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Enter password")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }
        private void TxtBox_Lost_Second(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Enter your mail address";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void TxtBox_Got_Second(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Enter your mail address")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }
    }
}
