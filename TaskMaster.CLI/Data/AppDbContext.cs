using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskMaster.CLI.Models;

namespace TaskMaster.CLI.Data
{
    public class AppDbContext : DbContext
    {
        //This poperty represents the table Tasks in the database Postgress
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Usamos 'localhost' porque estás ejecutando el comando desde Windows
            // Los datos coinciden exactamente con tu archivo YAML
            var connectionString = "Host=localhost;Port=5433;Database=taskmaster_db;Username=alvin_user;Password=password123";

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
