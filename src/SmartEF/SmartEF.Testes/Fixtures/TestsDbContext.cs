using Microsoft.EntityFrameworkCore;
using SmartEF.Testes.Database.Entities;

namespace SmartEF.Testes.Fixtures
{
    public sealed class TestsDbContext : DbContext
    {
        private readonly bool isTestRun;

        public TestsDbContext(DbContextOptions options, bool isTestRun = false) : base(options)
        {
            this.isTestRun = isTestRun;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (isTestRun)
            {
                // TODO: Run in test mode
            }
            else
            {
                // TODO: Run not in test mode
            }

            modelBuilder.ApplyConfiguration(new PersonMap());
            modelBuilder.ApplyConfiguration(new ContactMap());
        }
    }
}
