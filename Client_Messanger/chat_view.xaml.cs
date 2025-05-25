using Db_messenger.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for chat_view.xaml
    /// </summary>
    public partial class chat_view : Page
    {
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
            GetUsersAsycn();
            //GetChatAsycn();
        }
        public async void GetChatAsycn()
        {
            List<Chat> chats = await AppData.db.Chats.Include(c => c.Users).ToListAsync();


            var user = await AppData.db.Users
                .FirstOrDefaultAsync(u => u.Email == emailuser);

            if (user == null) return;

            var userChats = chats
                .Where(c => c.Users.Any(u => u.Id == user.Id))
                .ToList();

            ChatListBox.Items.Clear(); 
            foreach (var userChat in userChats)
            {
                ChatListBox.Items.Add(userChat.Chat_Name);
            }
        }
  
        public async void GetUsersAsycn()
        {
            List<Messages> messages = await GetUsers();

            //AppData.db.Users.Add(newUser);
            //await AppData.db.SaveChangesAsync();
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

        }
    
    public async Task<List<Messages>> GetUsers()
    {
        return await AppData.db.Messages.ToListAsync();
    }
    //public chat_view(string chatName, bool isGroup) 
    //        : this()
    //    {
    //        ChatListBox.Items.Add(chatName);
    //        ChatListBox.SelectedItem = chatName;
    //        SetChatTitle(chatName, isGroup ? "груповий" : "особистий");

    //        var participants = isGroup ? new List<string> { "User1", "User2", "User3" } : new List<string> { "User1" };
    //        LoadParticipants(participants);
    //    }

        private void CreateChatBtn(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CreateChatWindow());
        }

        private void ChatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatListBox.SelectedItem != null)
            {
                string chatName = ChatListBox.SelectedItem.ToString();
                bool isGroup = chatName.Contains("[група]");
                SetChatTitle(chatName, isGroup ? "груповий" : "особистий");

                List<string> participants = isGroup ? new List<string> { "User1", "User2", "User3" } : new List<string> { "User1" };
                LoadParticipants(participants);
            }
        }

        private async void SendBtn(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                Messages newmessage = new Messages
                {
                    ChatId = 1,
                    UserId = 9,
                    M_Text = message,
                    Sent_at = DateTime.Now,
                };
                AppData.db.Messages.Add(newmessage);
                await AppData.db.SaveChangesAsync();
                //AddMessage("Ви", message);
                MyMessage myMessage = new MyMessage
                    { Sender = "Ви",
                    Message = message,
                    Time = DateTime.Now.ToShortTimeString() };
                model.AddProcess(myMessage);
                MessageInput.Clear();
                if (ListBoxMessage.Items.Count > 0)
                {
                    ListBoxMessage.ScrollIntoView(ListBoxMessage.Items[ListBoxMessage.Items.Count - 1]);
                }
            }
        }

        private void AddMessage(string senderName, string text)
        {
            StackPanel messagePanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            TextBlock nameText = new TextBlock
            {
                Text = senderName + ": ",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 5, 0)
            };

            TextBlock messageText = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 500
            };

            messagePanel.Children.Add(nameText);
            messagePanel.Children.Add(messageText);
            //MessagesPanel.Children.Add(messagePanel);
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
        
    }
}
