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
            LoadChatList();
        }

        private void LoadChatList()
        {
            // приклад
            ChatListBox.Items.Add("Чат з Олею");
            ChatListBox.Items.Add("Група класу");
            ChatListBox.Items.Add("Бот-помічник");
        }

        private void ChatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatListBox.SelectedItem != null)
            {
                string selectedChat = ChatListBox.SelectedItem.ToString();
                ChatTitle.Text = selectedChat;

                MessagesPanel.Children.Clear();

                MessagesPanel.Children.Add(CreateMessageBubble("Привіт!", true));
                MessagesPanel.Children.Add(CreateMessageBubble("Привіт, як справи?", false));
            }
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                MessagesPanel.Children.Add(CreateMessageBubble(message, true));
                MessageInput.Clear();

 
                MessageInput.Focus();
            }
        }

        private Border CreateMessageBubble(string message, bool isOwn)
        {
            var bubble = new Border
            {
                Background = isOwn ? System.Windows.Media.Brushes.LightBlue : System.Windows.Media.Brushes.LightGray,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                HorizontalAlignment = isOwn ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Child = new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap
                }
            };
            return bubble;
        }
    }
}
