using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Extensions.Configuration;

namespace SQLServerConfigurationSample.SqlServerConfiguration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddSqlServerDb(this IConfigurationBuilder builder,
            bool optional = true)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddSqlServerDb(s =>
            {
                s.Optional = optional;
            });
        }

        public static IConfigurationBuilder AddSqlServerDb(
            this IConfigurationBuilder builder, Action<SqlServerConfigurationSource> configureSource)
        {
            return builder.Add(configureSource);
        }
    }
}
