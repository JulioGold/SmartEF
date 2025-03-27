using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace SmartEF.Testes.Fixtures
{
    [CollectionDefinition(nameof(TestsDbContext))]
    public sealed class SqlDbContextFactoryCollection : ICollectionFixture<SqlDbContextFactory>
    {
    }

    public sealed class SqlDbContextFactory : IDisposable
    {
        private readonly SqlConnection _connection;
        private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SmartEF_Tests;Trusted_Connection=True;MultipleActiveResultSets=True;";
        private bool _disposed = false;

        public string ConnectionString => _connectionString;

        public SqlDbContextFactory()
        {
            _connection = new SqlConnection(ConnectionString);
        }
        
        public async Task<DbContext> CriarAsync()
        {
            var contexto = CriarContexto();

            await contexto.Database.EnsureCreatedAsync();

            CriarConexao();

            return contexto;
        }
        
        private void CriarConexao()
        {
            if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        private TestsDbContext CriarContexto()
        {
            var options = new DbContextOptionsBuilder<TestsDbContext>()
                            .UseSqlServer(ConnectionString)
                            .Options;

            var contexto = new TestsDbContext(options, true);

            return contexto;
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _connection.Close();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            CriarContexto().Database.EnsureDeleted();

            Dispose(true);
        }
    }
}
