using Db_messenger.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Client_Messanger
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {


        List<User> users;
        //Зберігаю всі вкладення при вході
        ChoiceLogRegPage logpage;
        public RegisterPage(ChoiceLogRegPage log)
        {
            InitializeComponent();
            myImage.Source = new BitmapImage(new Uri("pack://application:,,,/images/register.png"));

            logpage = log;

            GetUsersAsycn();
        }
        public async void GetUsersAsycn()
        {
            users = await GetUsers();
        }
        public async Task<List<User>> GetUsers()
        {
            return await AppData.db.Users.ToListAsync();
        }


        private void NameTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Введи своє ім'я";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void NameTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Введи своє ім'я")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }
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



        private void BackToLogin(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(logpage);
        }

        private void myImage_Loaded(object sender, RoutedEventArgs e)
        {
            myImage.RenderTransform = new TranslateTransform();
            DoubleAnimation buttonAnimation = new DoubleAnimation();
            buttonAnimation.From = -myImage.ActualHeight;
            buttonAnimation.To = 0;
            buttonAnimation.Duration = TimeSpan.FromSeconds(0.3);


            ((TranslateTransform)myImage.RenderTransform).BeginAnimation(TranslateTransform.YProperty, buttonAnimation);
        }

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

        private async void RedinBtn(object sender, RoutedEventArgs e)
        {

            bool isUnique = true;
            if (PassTxr.Text != "" && EmailTxt.Text != "" && NameTxt.Text != "")
            {
                foreach (var item in users)
                {
                    if (item.Email == EmailTxt.Text)
                    {
                        MessageBox.Show("You already have account");
                        isUnique = false;
                        break;
                    }


                }

                if (PassTxr.Text.Length >= 8 && PassTxr.Text.Count(char.IsDigit) >= 2 && EmailTxt.Text.Length >= 10 && char.IsUpper(PassTxr.Text[0]) && isUnique == true)
                {
                    User newUser = new User
                    {
                        Nickname = NameTxt.Text,
                        Email = EmailTxt.Text,
                        Password = GetHash(PassTxr.Text)
                    };
                    AppData.db.Users.Add(newUser);
                    await AppData.db.SaveChangesAsync();
                    NavigationService.Navigate(new chat_view(NameTxt.Text, EmailTxt.Text));
                }
            }
        }


    }
}
