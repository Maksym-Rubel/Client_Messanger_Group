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
using System.Windows.Shapes;

namespace AdminWindow
{ 
/// <summary>
/// Interaction logic for AdminWindow.xaml
/// </summary>

{
    public partial class AdminWindow : Window
    {
        public ObservableCollection<User> Users { get; set; }

        public AdminWindow()
        {
            InitializeComponent();

            // Початкове наповнення колекції користувачів
            Users = new ObservableCollection<User>
            {
                new User { Login = "admin", Status = "Активний", Role = "Адміністратор", BlockedUntil = null },
                new User { Login = "user1", Status = "Активний", Role = "Користувач", BlockedUntil = null },
                new User { Login = "user2", Status = "Заблокований", Role = "Користувач", BlockedUntil = DateTime.Now.AddDays(3) }
            };

            UsersListView.ItemsSource = Users;
        }

        // Видалення користувача
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is User selectedUser)
            {
                var result = MessageBox.Show($"Видалити користувача '{selectedUser.Login}'?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Users.Remove(selectedUser);
                    MessageBox.Show("Користувача успішно видалено.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Оберіть користувача для видалення.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Блокування користувача
        private void Block_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is not User selectedUser)
            {
                MessageBox.Show("Оберіть користувача для блокування.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DurationComboBox.SelectedItem is not ComboBoxItem selectedItem ||
                !int.TryParse(selectedItem.Tag.ToString(), out int days))
            {
                MessageBox.Show("Оберіть тривалість блокування.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string reason = ReasonBox.Text.Trim();
            if (string.IsNullOrEmpty(reason) || reason == "Причина блокування")
            {
                MessageBox.Show("Введіть причину блокування.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedUser.BlockedUntil = DateTime.Now.AddDays(days);
            selectedUser.Status = "Заблокований";

            MessageBox.Show($"Користувача '{selectedUser.Login}' заблоковано до {selectedUser.BlockedUntil?.ToShortDateString()}.\nПричина: {reason}",
                            "Блокування виконано", MessageBoxButton.OK, MessageBoxImage.Information);

            UsersListView.Items.Refresh();
        }

        private void ReasonBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ReasonBox.Text == "Причина блокування")
            {
                ReasonBox.Text = "";
                ReasonBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
    }

    // Модель користувача
    public class User : INotifyPropertyChanged
    {
        private string login;
        private string status;
        private string role;
        private DateTime? blockedUntil;

        public string Login
        {
            get => login;
            set { login = value; OnPropertyChanged(nameof(Login)); }
        }

        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        public string Role
        {
            get => role;
            set { role = value; OnPropertyChanged(nameof(Role)); }
        }

        public DateTime? BlockedUntil
        {
            get => blockedUntil;
            set { blockedUntil = value; OnPropertyChanged(nameof(BlockedUntil)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly AppDbContext _context = new();

        public AdminWindow()
        {
            InitializeComponent();
            _context.Database.Migrate(); // або EnsureCreated() для SQLite
            DbInitializer.Seed(_context);

            Users = new ObservableCollection<User>(_context.Users.ToList());
            UsersListView.ItemsSource = Users;
        }

        // Видалення з бази
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem is User selectedUser)
            {
                _context.Users.Remove(selectedUser);
                _context.SaveChanges();
                Users.Remove(selectedUser);
                MessageBox.Show("Користувача видалено.");
            }
        }

        // Блокування
        private void Block_Click(object sender, RoutedEventArgs e)
        { 
            selectedUser.BlockedUntil = DateTime.Now.AddDays(days);
            selectedUser.Status = "Заблокований";
            _context.SaveChanges();
            UsersListView.Items.Refresh();
        }
    }
}
