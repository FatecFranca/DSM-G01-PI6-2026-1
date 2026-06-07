using Microsoft.EntityFrameworkCore;
using Sleep.Domain.Entities;

namespace Sleep.Infrasctucture.DataAccess
{
    public class SleepDbContext : DbContext
    {
        public SleepDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<SleepRecord> SleepRecord { get; set; }
        public DbSet<SleepAnalysis> SleepAnalysis { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SleepDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is EntityBase && e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = (EntityBase)entry.Entity;

                entity.Last_Updated = DateTime.UtcNow;
            }
        }
    }
}
