using System.Data;
using Microsoft.Data.SqlClient; 
using Microsoft.Extensions.Configuration;

namespace HRSupport.Infrastructure.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            // DÜZELTME: appsettings.json dosyasından bağlantı adresini okuyoruz.
            // Buradaki "SqlConnection" isminin appsettings.json dosyanla aynı olduğundan emin ol.
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}