﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.Tables.Templates
{

    public class SetupEntity : TableEntity
    {
        public string Value { get; set; }

        public static SetupEntity Create(string partitionKey, string rowKey, string value)
        {
            return new SetupEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Value = value
            };
        }
    }


    public class SetupByPartitionEntity : TableEntity
    {

        public string Value { get; set; }
        
    }

    public abstract class NoSqlSetupByPartition
    {
        private readonly INoSQLTableStorage<SetupByPartitionEntity> _tableStorage;

        protected NoSqlSetupByPartition(INoSQLTableStorage<SetupByPartitionEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public string GetValue(string partition, string field)
        {
            var entity = _tableStorage[partition, field];
            return entity?.Value;
        }

        public void SetValue(string partition, string field, string value)
        {
            var entity = new SetupByPartitionEntity { PartitionKey = partition, RowKey = field, Value = value };

            _tableStorage.InsertOrReplace(entity);
        }

        public void SetValue<T>(string partition, string field, T value)
        {
            SetValue(partition, field, (string)Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture));
        }

        public T GetValue<T>(string partition, string field, T @default)
        {
            var resultStr = GetValue(partition, field);
            if (resultStr == null)
                return @default;

            try
            {
                return (T)Convert.ChangeType(resultStr, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return @default;
            }
        }



        public async Task<string> GetValueAsync(string partition, string field)
        {
            var entity = await _tableStorage.GetDataAsync(partition, field);
            return entity?.Value;
        }

        public Task SetValueAsync(string partition, string field, string value)
        {
            var entity = new SetupByPartitionEntity { PartitionKey = partition, RowKey = field, Value = value };

            return _tableStorage.InsertOrReplaceAsync(entity);
        }

        public Task SetValueAsync<T>(string partition, string field, T value)
        {
            return SetValueAsync(partition, field, (string)Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture));
        }

        public async Task<T> GetValueAsync<T>(string partition, string field, T @default)
        {
            var resultStr = await GetValueAsync(partition, field);
            if (resultStr == null)
                return @default;

            try
            {
                return (T)Convert.ChangeType(resultStr, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return @default;
            }
        }

    }


    public class AzureSetupByPartition : NoSqlSetupByPartition
    {
        public AzureSetupByPartition(string connStr, string tableName, ILog log, bool caseSensitive = true)
            : base(new AzureTableStorage<SetupByPartitionEntity>(connStr, tableName, log, caseSensitive))
        {
        }
    }

    public class NoSqlSetupByPartitionInMemory : NoSqlSetupByPartition
    {
        public NoSqlSetupByPartitionInMemory()
            : base(new NoSqlTableInMemory<SetupByPartitionEntity>())
        {
        }
    }
}
