using Db_messenger.Entities;
using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for CreateChatWindow.xaml
    /// </summary>
    public partial class CreateChatWindow : Page
    {
        string nickname = "Maksum";
        string emailuser = "rubelmaksum2404@gmail.com";
        public CreateChatWindow()
        {
            InitializeComponent();
        }
        private void TxtBox_Lost_Second(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Введіть назву";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void TxtBox_Got_Second(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Введіть назву")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }
        public async Task<List<User>> GetUsers()
        {
            return await AppData.db.Users.ToListAsync();
        }
        public async Task<List<Chat>> GetChats()
        {
            return await AppData.db.Chats.ToListAsync();
        }
        private async void CreateChatBtn(object sender, RoutedEventArgs e)
        {
           try
           {
                string name = ChatNameBox.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Введіть назву чату.");
                    return;
                }

                //string type = ((ComboBoxItem)ChatTypeComboBox.SelectedItem).Content.ToString();
                //bool isGroup = type == "Груповий";

                //string fullChatName = isGroup ? $"[група] {name}" : name;
                List<User> users = await GetUsers();
                List<Chat> chats = await GetChats();

                var user = AppData.db.Users.Include(u => u.Chats).FirstOrDefault(u => u.Email == emailuser);
                if (user == null)
                {
                    MessageBox.Show("Користувача не знайдено.");
                    return;
                }
                Chat newChat = new Chat
                {
                   
                    Chat_Name = ChatNameBox.Text
                };
                AppData.db.Chats.Add(newChat);
                await AppData.db.SaveChangesAsync();

               
                user.Chats.Add(newChat);
                await AppData.db.SaveChangesAsync();
                NavigationService?.Navigate(new chat_view(nickname, emailuser));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
            }
            
        }
    }
}
