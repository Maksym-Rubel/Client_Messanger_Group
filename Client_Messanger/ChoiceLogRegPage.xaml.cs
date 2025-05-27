using Db_messenger.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Client_Messanger
{
    /// <summary>
    /// Interaction logic for ChoiceLogRegPage.xaml
    /// </summary>
    public partial class ChoiceLogRegPage : Page
    {

        List<User> users;
        public ChoiceLogRegPage()
        {
            InitializeComponent();
            // Add image
            myImage.Source = new BitmapImage(new Uri("pack://application:,,,/images/messenger.png"));

            try
            {
                GetUsersAsycn();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public async void GetUsersAsycn()
        {

            try
            {
                users = await GetUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public async Task<List<User>> GetUsers()
        {

            if (AppData.db == null)
            {
                MessageBox.Show("База даних не ініціалізована!");
                return new List<User>();
            }
            return await AppData.db.Users.ToListAsync();




        }

        // Lost and Got Focus for text box
        private void TxtBox_Lost(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Введи пароль";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void TxtBox_Got(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Введи пароль")
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
                tb.Text = "Введи свою електронну адресу";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void TxtBox_Got_Second(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Введи свою електронну адресу")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void Regestration(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage(this));
        }

        // Хешування пароля
        public static string GetHash(string password)
        {
            byte[] data = Encoding.Unicode.GetBytes(password);
            byte[] hash = SHA1.HashData(data);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
        private void LogInBtn(object sender, RoutedEventArgs e)
        {
            bool logining = false;
            if (PassTxr.Text != "" && EmailTxt.Text != "")
            {
                foreach (var item in users)
                {
                    if (item.Email == EmailTxt.Text && item.Password == GetHash(PassTxr.Text))
                    {
                        logining = true;
                        NavigationService.Navigate(new chat_view(item.Nickname, EmailTxt.Text));

                    }


                }
                if (logining != true)
                {
                    MessageBox.Show("Неправильний пароль або пошта");
                }

            }
        }
    }
}
