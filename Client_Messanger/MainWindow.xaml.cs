using System.Windows;

namespace Client_Messanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new ChoiceLogRegPage();
            //MainFrame.Content = new chat_view("Maksum", "rubelmaksum2404@gmail.com");

        }







    }
}