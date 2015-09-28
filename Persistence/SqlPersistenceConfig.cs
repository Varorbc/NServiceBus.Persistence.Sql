using System;
using NServiceBus.Configuration.AdvanceExtensibility;
using NServiceBus.Persistence;
using NServiceBus.Settings;

namespace NServiceBus
{
    public static class SqlPersistenceConfig
    {
        public static void ConnectionString(this PersistenceExtentions<Persistence.SqlPersistence> persistenceConfiguration, string connectionString)
        {
            persistenceConfiguration.GetSettings()
                .Set("SqlPersistence.ConnectionString", connectionString);
        }

        public static void ConnectionString<TStorageType>(this PersistenceExtentions<Persistence.SqlPersistence, TStorageType> persistenceConfiguration, string connectionString)
            where TStorageType : StorageType
        {
            var key = "SqlPersistence." + typeof (TStorageType).Name + ".ConnectionString";
            persistenceConfiguration.GetSettings()
                .Set(key, connectionString);
        }

        internal static string GetConnectionString<TStorageType>(this ReadOnlySettings settings)
            where TStorageType : StorageType
        {
            return settings.GetValue<string, TStorageType>("ConnectionString", () => { throw new Exception("ConnectionString must be defined."); });
        }


        public static void DisableInstaller(this PersistenceExtentions<Persistence.SqlPersistence> persistenceConfiguration)
        {
            persistenceConfiguration.GetSettings()
                .Set("SqlPersistence.DisableInstaller", true);
        }

        public static void DisableInstaller<TStorageType>(this PersistenceExtentions<Persistence.SqlPersistence, TStorageType> persistenceConfiguration)
            where TStorageType : StorageType
        {
            var key = "SqlPersistence." + typeof (TStorageType).Name + ".DisableInstaller";
            persistenceConfiguration.GetSettings()
                .Set(key, true);
        }

        internal static bool GetDisableInstaller<TStorageType>(this ReadOnlySettings settings)
            where TStorageType : StorageType
        {
            return settings.GetValue<bool, TStorageType>("DisableInstaller", () => false);
        }

        public static void Schema(this PersistenceExtentions<Persistence.SqlPersistence> persistenceConfiguration, string schema)
        {
            persistenceConfiguration.GetSettings()
                .Set("SqlPersistence.Schema", schema);
        }

        public static void Schema<TStorageType>(this PersistenceExtentions<Persistence.SqlPersistence, TStorageType> persistenceConfiguration, string schema)
            where TStorageType : StorageType
        {
            var key = "SqlPersistence." + typeof (TStorageType).Name + ".Schema";
            persistenceConfiguration.GetSettings()
                .Set(key, schema);
        }

        internal static string GetSchema<TStorageType>(this ReadOnlySettings settings) where TStorageType : StorageType
        {
            return settings.GetValue<string, TStorageType>("Schema", () => "dbo");
        }

        internal static TValue GetValue<TValue, TStorageType>(this ReadOnlySettings settings, string suffix, Func<TValue> defaultValue)
            where TStorageType : StorageType
        {
            var key = string.Format("SqlPersistence.{0}.{1}", typeof (TStorageType).Name, suffix);
            TValue value;
            if (settings.TryGet(key, out value))
            {
                return value;
            }
            if (settings.TryGet("SqlPersistence." + suffix, out value))
            {
                return value;
            }
            return defaultValue();
        }

    }
}