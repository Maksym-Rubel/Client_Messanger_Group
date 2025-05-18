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
    /// Interaction logic for chat_view.xaml
    /// </summary>
    public partial class chat_view : Page
    {
        public chat_view()
        {
            InitializeComponent();
            LoadParticipants(new List<string> { "User1" });
            SetChatTitle("Оберіть чат", "особистий");
        }

        private void CreateChatBtn(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функція створення чату ще не реалізована");
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

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                AddMessage("Ви", message);
                MessageInput.Clear();
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
            MessagesPanel.Children.Add(messagePanel);
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
    }
}
