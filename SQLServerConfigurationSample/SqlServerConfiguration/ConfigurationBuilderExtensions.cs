using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Extensions.Configuration;

namespace SQLServerConfigurationSample.SqlServerConfiguration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddSqlServerDb(this IConfigurationBuilder builder,
            bool optional = true, bool reloadOnChange = false)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddSqlServerDb(s => { }, optional, reloadOnChange);
        }

        public static IConfigurationBuilder AddSqlServerDb(
            this IConfigurationBuilder builder, Action<SqlServerConfigurationSource> configureSource,
            bool optional, bool reloadOnChange)
        {
            return builder.Add(configureSource);
        }
    }
}
