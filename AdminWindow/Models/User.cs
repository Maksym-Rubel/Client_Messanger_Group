using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace AdminWindow.Models
{
    public class User : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }

        public string Login { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? BlockedUntil { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
