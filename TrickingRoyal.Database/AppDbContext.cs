using Battles.Domain.Models;
using Battles.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TrickingRoyal.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Match> Matches { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<SubComment> SubComments { get; set; }
        public DbSet<UserInformation> UserInformation { get; set; }
        public DbSet<MatchUser> MatchUser { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Decision> Decisions { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<NotificationMessage> NotificationMessages { get; set; }
        public DbSet<NotificationConfiguration> NotificationConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MatchUser>()
                .HasKey(x => new {x.MatchId, x.UserId});

            modelBuilder.Entity<Like>()
                .HasKey(x => new {x.MatchId, x.UserId});

            modelBuilder.Entity<UserInformation>()
                .Property(x => x.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<UserInformation>()
                .HasKey(x => x.Id);
        }
    }
}