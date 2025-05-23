using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
