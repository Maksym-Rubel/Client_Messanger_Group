using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client_Messanger
{
    /// <summary>
    /// Interaction logic for AddNewUserInChat.xaml
    /// </summary>
    public partial class AddNewUserInChat : Page
    {
        string nickname = "Maksum";
        string emailuser = "rubelmaksum2404@gmail.com";
        string chatnames = "";
        public AddNewUserInChat(string chatname)
        {
            InitializeComponent();
            chatnames = chatname;
        }
        private void TxtBox_Lost_Second(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Введіть пошту";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void TxtBox_Got_Second(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Введіть пошту")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private async void MyBtn_Click(object sender, RoutedEventArgs e)
        {

            var user = AppData.db.Users.Include(u => u.Chats).FirstOrDefault(u => u.Email == ChatNameBox.Text);
            if (user == null)
            {
                MessageBox.Show("Користувача не знайдено.");
                return;
            }
            var chat = AppData.db.Chats.Include(u => u.Users).FirstOrDefault(u => u.Chat_Name == chatnames);
            if (chat == null)
            {
                MessageBox.Show("Користувача не знайдено.");
                return;
            }

            if (!user.Chats.Any(c => c.Id == chat.Id))
            {
                user.Chats.Add(chat);
                await AppData.db.SaveChangesAsync();
            }
            else
            {
                MessageBox.Show("Користувач уже є в чаті.");
            }

            NavigationService?.Navigate(new chat_view(nickname, emailuser));



        }
    }
}
