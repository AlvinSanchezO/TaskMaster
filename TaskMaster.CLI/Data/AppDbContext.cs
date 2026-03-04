using Microsoft.EntityFrameworkCore;
using TaskMaster.CLI.Models;

namespace TaskMaster.CLI.Data
{
    public class AppDbContext : DbContext
    {
        // 1. CONSTRUCTOR PARA LA API
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 2. CONSTRUCTOR PARA LA CLI
        public AppDbContext()
        {
        }

        // --- TABLAS DE LA BASE DE DATOS ---
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        // 3. NUEVA TABLA DE CATEGORÍAS (#TM-027)
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Host=localhost;Port=5433;Database=taskmaster_db;Username=alvin_user;Password=password123";
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        // 4. CONFIGURACIÓN DE RELACIONES (Opcional pero recomendado)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura que una Categoría tiene muchas Tareas
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId);
        }
    }
}