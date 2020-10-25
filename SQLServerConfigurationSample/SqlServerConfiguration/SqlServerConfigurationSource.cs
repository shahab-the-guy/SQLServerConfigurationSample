using Microsoft.Extensions.Configuration;

namespace SQLServerConfigurationSample.SqlServerConfiguration
{
    public class SqlServerConfigurationSource : IConfigurationSource
    {
        public string Server { get; set; } = "localhost";
        public string Database { get; set; } = "ConfigurationDb";
        public string User { get; set; } = "sa";
        public string Password { get; set; } = "yourStrong(!)Password";
        public string Table { get; set; } = "dbo.Configuration";
        public string KeyColumn { get; set; } = "Key";
        public string ValueColumn { get; set; } = "Value";

        public bool Optional { get; set; } = true;

        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new SqlServerConfigurationProvider(this);
    }
}
