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
        // 1. CONSTRUCTOR PARA LA API: Permite que la API inyecte la configuración externa
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 2. CONSTRUCTOR PARA LA CLI: Permite que la consola siga funcionando sin cambios
        public AppDbContext()
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 3. SOLO CONFIGURAMOS SI NO VIENE YA CONFIGURADO DESDE LA API
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Host=localhost;Port=5433;Database=taskmaster_db;Username=alvin_user;Password=password123";
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        //New User table
        public DbSet<User> Users { get; set; }
    }
}