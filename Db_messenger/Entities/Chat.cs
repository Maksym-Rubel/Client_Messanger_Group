using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db_messenger.Entities
{
    public class Chat
    {
        public int Id { get; set; } 
        public string Chat_Name { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Messages> Messages { get; set; }

    }
}
