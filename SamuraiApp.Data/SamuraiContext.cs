using Microsoft.EntityFrameworkCore;
using SamuraiApp.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace SamuraiApp.Data
{
    public class SamuraiContext:DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory = 
            LoggerFactory.Create(builder =>
            {
                builder.AddFilter((category, level) => 
                    category == DbLoggerCategory.Database.Command.Name
                    && level == LogLevel.Information)
                .AddConsole();
            });
        
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<Clan> Clans { get; set; }
        //To avoid having a publicly accessible Entity, use the modelBuilder.Entity<[entity-type]>().ToTable("[table-name]") extention in OnModelCreating.
        //public DbSet<Horse> Horses {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)
                .EnableSensitiveDataLogging()//This will reveal the data that was inserted or updated to the DB. Use this only in development.
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SamuraiAppData;Trusted_Connection=True;MultipleActiveResultSets=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>()
                .HasKey(s => new { s.SamuraiId, s.BattleId });
            modelBuilder.Entity<Horse>().ToTable("Horses");

            base.OnModelCreating(modelBuilder);
        }
    }
}
