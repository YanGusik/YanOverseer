using Microsoft.EntityFrameworkCore;
using YanOverseer.DAL.Models;

namespace YanOverseer.DAL
{
    public class MainContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<GuildSettings> GuildSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=blogging.db");
    }
}