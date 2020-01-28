using Microsoft.EntityFrameworkCore;
using SamuraiApp.Domain;

namespace SamuraiApp.Data
{
    public class SamuraiContext:DbContext
    {
        //logging is built-in to asp.net core so we dont need this either
        //public static readonly ILoggerFactory MyLoggerFactory = 
        //    LoggerFactory.Create(builder =>
        //    {
        //        builder.AddFilter((category, level) => 
        //            category == DbLoggerCategory.Database.Command.Name
        //            && level == LogLevel.Information)
        //        .AddConsole();
        //    });

        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<Clan> Clans { get; set; }
        //Readonly table (untracked due to absense of PK (intentional)):
        public DbSet<SamuraiBattleStat> SamuraiBattleStats { get; set; }
        //To avoid having a publicly accessible Entity, use the modelBuilder.Entity<[entity-type]>().ToTable("[table-name]") extention in OnModelCreating.
        //public DbSet<Horse> Horses {get;set;}

        public SamuraiContext()
        { }

        public SamuraiContext(DbContextOptions<SamuraiContext> options):base(options)
        {
            //Comment out for unit testing
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SamuraiAppData;Trusted_Connection=True;MultipleActiveResultSets=True");
            //Example for setting batch size
            //.UseSqlServer(connString, options => options.MaxBatchSize(150));
        }


        //NOTE: Config info is now setup in web app so we dont need this, logging is built-in to asp.net core so we dont need that either
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var connString = "Server=(localdb)\\mssqllocaldb;Database=SamuraiAppData;Trusted_Connection=True;MultipleActiveResultSets=True";
        //    optionsBuilder
        //        .UseLoggerFactory(MyLoggerFactory)
        //        .EnableSensitiveDataLogging()//This will reveal the data that was inserted or updated to the DB. Use this only in development.
        //        .UseSqlServer(connString);
        //        //Example for setting batch size
        //        //.UseSqlServer(connString, options => options.MaxBatchSize(150));
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>()
                .HasKey(s => new { s.SamuraiId, s.BattleId });
            modelBuilder.Entity<Horse>().ToTable("Horses");
            //Add the readonly entity and instruct EFCore to use the previously created view
            //NOTE: EFCore doesnt know how to create view, this instruction will prevent it from trying
            //to create a table
            modelBuilder.Entity<SamuraiBattleStat>().HasNoKey().ToView("SamuraiBattleStats");

            base.OnModelCreating(modelBuilder);
        }
    }
}
