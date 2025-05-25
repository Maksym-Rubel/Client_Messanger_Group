using Db_messenger.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Client_Messanger
{
    /// <summary>
    /// Interaction logic for chat_view.xaml
    /// </summary>
    public partial class chat_view : Page
    {
        int chatid;
        int userid;

        ViewModel model;
        string nickname = "Maksum";
        string emailuser = "rubelmaksum2404@gmail.com";

        public chat_view(string NickName, string Email)
        {
            InitializeComponent();
            model = new ViewModel();
            this.DataContext = model;
            LoadParticipants(new List<string> { "User1" });
            SetChatTitle("Оберіть чат", "особистий");
            GetChatAsync();


        }
        public async Task<List<Chat>> GetChats()
        {
            return await AppData.db.Chats.Include(w => w.Users).ToListAsync();
        }
        public async void GetChatAsync()
        {
            List<Chat> chats = await GetChats();


            var user = await AppData.db.Users
                .FirstOrDefaultAsync(u => u.Email == emailuser);
            userid = user.Id;
            if (user == null) return;

            var userChats = await AppData.db.Chats
            .Include(c => c.Users)
            .Where(c => c.Users.Any(u => u.Email == emailuser))
            .ToListAsync();
            ChatListBox.Items.Clear();
            foreach (var userChat in userChats)
            {
                ChatListBox.Items.Add(userChat.Chat_Name);
            }
        }

        private void TxtBox_Lost_Second(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Написати повідомлення...";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void TxtBox_Got_Second(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Написати повідомлення...")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }




        public async Task<List<Messages>> GetUsers(int chatid)
        {
            return await AppData.db.Messages.Where(w => w.ChatId == chatid).ToListAsync();
        }


        private void CreateChatBtn(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CreateChatWindow());
        }

        private async void ChatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatListBox.SelectedItem != null)
            {
                string chatName = ChatListBox.SelectedItem.ToString();
                bool isGroup = chatName.Contains("[група]");
                SetChatTitle(chatName, isGroup ? "груповий" : "особистий");


                string name = ChatListBox.SelectedItem.ToString();

                Chat chat = AppData.db.Chats.FirstOrDefault(w => w.Chat_Name == name);

                chatid = chat.Id;
                List<Messages> messages = await GetUsers(chat.Id);
                model.ClearProcess();

                foreach (var item in messages)
                {
                    var user = AppData.db.Users.FirstOrDefault(u => u.Id == item.UserId);
                    string senderName = user != null ? user.Nickname : "Невідомий користувач";
                    string email = user != null ? user.Email : "Невідомий користувач";

                    if (email == emailuser)
                    {
                        senderName = "Ви";
                    }
                    MyMessage myMessage = new MyMessage
                    {
                        Sender = senderName,
                        Message = item.M_Text,
                        Time = item.Sent_at.ToShortTimeString()
                    };
                    model.AddProcess(myMessage);
                };
                if (ListBoxMessage.Items.Count > 0)
                {
                    ListBoxMessage.ScrollIntoView(ListBoxMessage.Items[ListBoxMessage.Items.Count - 1]);
                }
                ParticipantsListBox.Items.Clear();

                var userChats = await AppData.db.Users.Include(c => c.Chats).Where(c => c.Chats.Any(u => u.Id == chatid)).ToListAsync();
                foreach (var item in userChats)
                {
                    ParticipantsListBox.Items.Add(item.Nickname);
                }


            }
        }

        private async void SendBtn(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                Messages newmessage = new Messages
                {
                    ChatId = chatid,
                    UserId = userid,
                    M_Text = message,
                    Sent_at = DateTime.Now,
                };
                AppData.db.Messages.Add(newmessage);
                await AppData.db.SaveChangesAsync();
                MyMessage myMessage = new MyMessage
                {
                    Sender = "Ви",
                    Message = message,
                    Time = DateTime.Now.ToShortTimeString()
                };
                model.AddProcess(myMessage);
                MessageInput.Clear();
                if (ListBoxMessage.Items.Count > 0)
                {
                    ListBoxMessage.ScrollIntoView(ListBoxMessage.Items[ListBoxMessage.Items.Count - 1]);
                }
                MessageInput.Text = "Написати повідомлення...";
                MessageInput.Foreground = Brushes.Gray;
            }
        }



        private void LoadParticipants(List<string> users)
        {
            ParticipantsListBox.Items.Clear();

            foreach (string user in users)
            {
                StackPanel userPanel = new StackPanel { Orientation = Orientation.Horizontal };

                Ellipse status = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Green,
                    Margin = new Thickness(0, 0, 5, 0)
                };

                TextBlock userName = new TextBlock
                {
                    Text = user
                };

                userPanel.Children.Add(status);
                userPanel.Children.Add(userName);

                ParticipantsListBox.Items.Add(userPanel);
            }
        }

        private void SetChatTitle(string name, string type)
        {
            ChatTitle.Text = name;
            ChatType.Text = type == "груповий" ? "[Груповий чат]" : "[Особистий чат]";
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.IsKeyDown(Key.LeftShift)))
            {
                int caretIndex = MessageInput.CaretIndex;
                MessageInput.Text = MessageInput.Text.Insert(caretIndex, "\n");
                MessageInput.CaretIndex = caretIndex + 1;

            }
            else if (e.Key == Key.Enter)
            {
                SendBtn(sender, e);
            }

        }
        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ListBoxMessage.SelectedIndex = -1;
        }

        private void MyListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }


    public class MyMessage()
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
    }
    public class ViewModel
    {
        private ObservableCollection<MyMessage> files;
        public ViewModel()
        {
            files = new ObservableCollection<MyMessage>();


        }
        public IEnumerable<MyMessage> Files => files;


        public void AddProcess(MyMessage info)
        {
            files.Add(info);
        }
        public void ClearProcess()
        {
            files.Clear();
        }

    }
}
