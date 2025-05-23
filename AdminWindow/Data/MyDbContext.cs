using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminWindow.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminWindow.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext() : base() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Локальна база SQLite
            optionsBuilder.UseSqlite("Data Source=users.db");
        }
    }
}
