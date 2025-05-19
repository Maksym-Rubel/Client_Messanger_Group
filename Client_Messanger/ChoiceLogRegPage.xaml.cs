using Db_messenger.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
        
        List<User> users;
        public ChoiceLogRegPage()
        {
            InitializeComponent();
            // Add image
            myImage.Source = new BitmapImage(new Uri("pack://application:,,,/images/messenger.png"));
            
            
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
      
        // Lost and Got Focus for text box
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
                foreach(var item in users)
                {
                    if(item.Email == EmailTxt.Text && item.Password == GetHash(PassTxr.Text))
                    {
                        logining = true;
                        NavigationService.Navigate(new chat_view());
                    }
                    

                }
                if(logining != true)
                {
                    MessageBox.Show("Неправильний пароль або пошта");
                }

            }
        }
    }
}
