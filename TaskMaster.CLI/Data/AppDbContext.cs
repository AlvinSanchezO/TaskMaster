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
            //No localhost. Docker compose will create a network and the hostname will be the name of the service defined in the docker-compose.yml file
            var connectionString = "Host=postgres;Port=5432;Database=taskmasterdb;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
