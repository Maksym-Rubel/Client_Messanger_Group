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
        public CreateChatWindow()
        {
            InitializeComponent();
        }
        private void CreateChatBtn(object sender, RoutedEventArgs e)
        {
            string name = ChatNameBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введіть назву чату.");
                return;
            }

            string type = ((ComboBoxItem)ChatTypeComboBox.SelectedItem).Content.ToString();
            bool isGroup = type == "Груповий";

            string fullChatName = isGroup ? $"[група] {name}" : name;

            NavigationService?.Navigate(new chat_view(fullChatName, isGroup));
        }
    }
}
