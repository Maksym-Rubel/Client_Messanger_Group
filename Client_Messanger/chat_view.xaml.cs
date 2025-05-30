using Db_messenger.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;


namespace Client_Messanger
{
    /// <summary>
    /// Interaction logic for chat_view.xaml
    /// </summary>
    public partial class chat_view : Page
    {
        public int chatid;
        int userid;
        TcpChatClient tcpChatClient = new TcpChatClient();
        ViewModel model;

        string nickname = "";
        string emailuser = "";


        public chat_view(string NickName, string Email)
        {
            InitializeComponent();
            model = new ViewModel();

            this.DataContext = model;


            nickname = NickName;
            emailuser = Email;

            SetChatTitle("Оберіть чат", "особистий");
            GetChatAsync();
            myImage.Source = new BitmapImage(new Uri("pack://application:,,,/images/buttonIcon.png"));
            this.Loaded += ChatView_Loaded;
            Directory.CreateDirectory("ReceivedFile");


        }

        private async void ChatView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await tcpChatClient.ConnectAsync("37.54.60.152", 5000, nickname, emailuser, model);
                await tcpChatClient.StartListeningAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося підключитися до сервера: " + ex.Message);
            }
        }
        public async Task<List<Chat>> GetChats()
        {
            return await AppData.db.Chats.Include(w => w.Users).ToListAsync();
        }
        public async void GetChatAsync()
        {
            try
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
                model.ClearProcessChat();
                foreach (var userChat in userChats)
                {
                    Messages messages = AppData.db.Messages
                    .Where(m => m.ChatId == userChat.Id)
                    .OrderByDescending(m => m.Id)
                    .FirstOrDefault()!;

                    string lastMessage = messages != null ? messages.M_Text : "Немає повідомленнь";
                    string lastTime = messages != null ? messages.Sent_at.ToShortTimeString() : "";
                    if (lastMessage.Length > 25)
                    {
                        lastMessage = lastMessage.Substring(0, 25) + "…";

                    }
                    MyChats myChat = new MyChats { ChatId = chatid, ChatName = userChat.Chat_Name, LastMessage = lastMessage, LastTime = lastTime };
                    model.AddProcessChat(myChat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            NavigationService?.Navigate(new CreateChatWindow(nickname, emailuser));
        }

        private async void ChatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ChatListBox.SelectedItem != null)
                {
                    var chat1 = ChatListBox.SelectedItem as MyChats;
                    string chatName = chat1.ChatName;
                    bool isGroup = chatName.Contains("[група]");
                    SetChatTitle(chatName, isGroup ? "груповий" : "особистий");




                    Chat chat = AppData.db.Chats.FirstOrDefault(w => w.Chat_Name == chatName);

                    chatid = chat.Id;
                    tcpChatClient.SetChatId(chatid);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                try
                {

                    await tcpChatClient.SendMessageAsync(message, chatid);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при відправці: {ex.Message}");
                }

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

            //ListBoxMessage.SelectedIndex = -1;
        }

        private void MyListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            //e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = ChatListBox.SelectedItem.ToString();
                NavigationService?.Navigate(new AddNewUserInChat(name));
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }

        }

        private async void SelectFIle(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                await tcpChatClient.SendFileAsync(dialog.FileName, chatid);
                Messages newmessage = new Messages
                {
                    ChatId = chatid,
                    UserId = userid,
                    M_Text = Path.GetFileName(dialog.FileName),
                    Sent_at = DateTime.Now,
                };
                AppData.db.Messages.Add(newmessage);
                await AppData.db.SaveChangesAsync();
            }
        }

        private void ListBoxMessage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var selectedMessage = ListBoxMessage.SelectedItem as MyMessage;
            if (selectedMessage != null)
            {
                MessageBox.Show(selectedMessage.Message);

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string directory = Path.Combine(documentsPath, "ReceivedFile");

                string path = Path.Combine(directory, selectedMessage.Message);

                Process.Start("explorer.exe", $"/select,\"{path}\"");

            }
            else
            {
                MessageBox.Show("Немає вибраного повідомлення.");
            }


        }
        public void ScrollDown()
        {
            if (ListBoxMessage.Items.Count > 0)
            {
                ListBoxMessage.ScrollIntoView(ListBoxMessage.Items[ListBoxMessage.Items.Count - 1]);
            }
        }
    }

    public class TcpChatClient
    {
        private TcpClient client;
        private StreamWriter writer;
        private StreamReader reader;
        private string nickname;
        private string email;
        private ViewModel model;


        public event Action<string> MessageReceived;
        public async Task ConnectAsync(string host, int port, string nickname, string email, ViewModel model)
        {
            this.nickname = nickname;
            this.email = email;
            this.model = model;

            client = new TcpClient();
            await client.ConnectAsync(host, port);
            var stream = client.GetStream();
            writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            reader = new StreamReader(stream, Encoding.UTF8);
        }
        private int chatId;

        public void SetChatId(int id)
        {
            chatId = id;
        }
        public async Task SendMessageAsync(string text, int chatId1)
        {
            if (writer == null)
                throw new InvalidOperationException("TcpChatClient не підключений. Викличте ConnectAsync перед відправкою повідомлень.");
            var chatMessage = new ChatMessages
            {
                Type = "Message",
                From = nickname,
                Email = email,
                Content = text,
                DateTime = DateTime.Now.ToShortTimeString(),
                chatid = chatId1
            };

            string json = JsonSerializer.Serialize(chatMessage);
            await writer.WriteLineAsync(json);
        }
        public async Task SendFileAsync(string filePath, int chatId1)
        {
            //MessageBox.Show(filePath);
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);
            var chatMessage = new ChatMessages
            {
                Type = "File",
                From = nickname,
                Email = email,
                DateTime = DateTime.Now.ToShortTimeString(),
                chatid = chatId1,
                FileBytes = Convert.ToBase64String(fileBytes),
                FileName = fileName


            };
            Application.Current.Dispatcher.Invoke(() =>
            {
                MyMessage myMessage = new MyMessage
                {
                    Sender = nickname,
                    Message = $"File:{fileName}",
                    Time = DateTime.Now.ToShortTimeString(),

                };
                model.AddProcess(myMessage);
            });
            string json = JsonSerializer.Serialize(chatMessage);
            await writer.WriteLineAsync(json);
        }
        public async Task StartListeningAsync()
        {
            try
            {
                while (true)
                {
                    string line = await reader.ReadLineAsync();
                    if (line == null) break;
                    MessageReceived?.Invoke(line);
                    ChatMessages message = JsonSerializer.Deserialize<ChatMessages>(line);
                    //MessageBox.Show(line);
                    //MessageBox.Show($"{message.Email}{email}{message}{line}");

                    if (message.Email != email && message.chatid == chatId)
                    {
                        if (message.Type == "Message")
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MyMessage myMessage = new MyMessage
                                {
                                    Sender = message.From,
                                    Message = message.Content,
                                    Time = message.DateTime,
                                };
                                model.AddProcess(myMessage);
                                model.ChangeProcessChat(message.Content, chatId);
                            });
                        }
                        else if (message.Type == "File")
                        {
                            byte[] filebytes = Convert.FromBase64String(message.FileBytes);
                            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            string directory = Path.Combine(documentsPath, "ReceivedFile");
                            Directory.CreateDirectory(directory);
                            string safeFileName = Path.GetFileName(message.FileName);
                            string path = Path.Combine(directory, safeFileName);
                            File.WriteAllBytes(path, filebytes);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MyMessage myMessage = new MyMessage
                                {
                                    Sender = message.From,
                                    Message = message.FileName,
                                    Time = message.DateTime,
                                };
                                model.AddProcess(myMessage);
                            });
                        }

                    }

                }
            }
            catch { }
        }



        public void Disconnect()
        {
            client.Close();
        }
    }
    public class MyChats
    {
        public int ChatId { get; set; }

        public string ChatName { get; set; }
        public string ChatNameFirstLetter => ChatName[0].ToString().ToUpper();

        public string LastMessage { get; set; }
        public string LastTime { get; set; }
    }

    public class MyMessage
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
    }
    public class ViewModel
    {
        private ObservableCollection<MyChats> chats;
        private ObservableCollection<MyMessage> files;
        public ViewModel()
        {
            files = new ObservableCollection<MyMessage>();
            chats = new ObservableCollection<MyChats>();

        }
        public IEnumerable<MyMessage> Files => files;
        public IEnumerable<MyChats> Chats => chats;

        public void AddProcess(MyMessage info)
        {
            files.Add(info);
        }
        public void ClearProcess()
        {
            files.Clear();
        }
        public void ChangeProcessChat(string newmes, int chatid)
        {
            var chat = chats.Where(m => m.ChatId == chatid).FirstOrDefault();
            if (chat != null)
            {
                chat.LastMessage = newmes;
            }

        }
        public void AddProcessChat(MyChats info)
        {
            chats.Add(info);
        }
        public void ClearProcessChat()
        {
            chats.Clear();
        }

    }
}
