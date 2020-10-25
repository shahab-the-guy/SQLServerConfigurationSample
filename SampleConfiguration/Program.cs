using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace SampleConfiguration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configure =>
                {
                    configure.AddSqlServerDb();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddSqlServerDb(this IConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            return builder.AddSqlServerDb(s =>{ });
        }
        
        public static IConfigurationBuilder AddSqlServerDb(
            this IConfigurationBuilder builder, Action<SqlServerConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
    
    
    public class SqlServerConfigurationSource : IConfigurationSource
    {
        public string Server { get; set; } = "localhost";
        public string Database { get; set; } = "ConfigurationDb";
        public string User { get; set; } = "sa";
        public string Password { get; set; } = "yourStrong(!)Password";
        public string Table { get; set; } = "dbo.Configuration";
        public string KeyColumn { get; set; } = "Key";
        public string ValueColumn { get; set; } = "Value";
        
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new SqlServerConfigurationProvider(this);
    }

    public class SqlServerConfigurationProvider : ConfigurationProvider , IDisposable
    {
        private readonly SqlConnection _sqlConnection;
        private readonly SqlServerConfigurationSource _configurationSource;

        public SqlServerConfigurationProvider(SqlServerConfigurationSource configurationSource)
        {
            _configurationSource = configurationSource;
            
            _sqlConnection = new SqlConnection($"Server={_configurationSource.Server}; " +
                                                  $"Database={_configurationSource.Database};" +
                                                  $"UID={_configurationSource.User};" +
                                                  $"PWD={_configurationSource.Password}");
        }
        
        public override void Load()
        {
            var fetchQuery = $"SELECT  " +
                             $"c.[{_configurationSource.KeyColumn}] as 'Key' , " +
                             $"c.[{_configurationSource.ValueColumn}] as 'Value'  " +
                             $"FROM   {_configurationSource.Table} as c";

            var sqlCommand = _sqlConnection.CreateCommand();
            sqlCommand.CommandText = fetchQuery;
            
            if(sqlCommand.Connection.State != ConnectionState.Open) _sqlConnection.Open();

            var reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                var key = reader.GetString("Key");
                var value = reader.GetString("Value");
                
                this.Data.Add(key,value);
            }
            
            reader.Close();
            
            if( sqlCommand.Connection.State != ConnectionState.Closed )
                sqlCommand.Connection.Close();
        }


        public void Dispose()
        {
            _sqlConnection.Dispose();
        }
    }
}
