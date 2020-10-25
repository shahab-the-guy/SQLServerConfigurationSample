using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SQLServerConfigurationSample.SqlServerConfiguration
{
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
            try
            {
                var fetchQuery = $"SELECT  " +
                                 $"c.[{_configurationSource.KeyColumn}] as 'Key' , " +
                                 $"c.[{_configurationSource.ValueColumn}] as 'Value'  " +
                                 $"FROM   {_configurationSource.Table} as c";

                var sqlCommand = _sqlConnection.CreateCommand();
                sqlCommand.CommandText = fetchQuery;

                if (sqlCommand.Connection.State != ConnectionState.Open) _sqlConnection.Open();

                var reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    var key = reader.GetString("Key");
                    var value = reader.GetString("Value");

                    this.Data.Add(key, value);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                if (!this._configurationSource.Optional)
                    throw;
            }
            finally
            {
                if( _sqlConnection.State != ConnectionState.Closed )
                    _sqlConnection.Close();
            }
        }


        public void Dispose()
        {
            _sqlConnection.Dispose();
        }
    }
}
