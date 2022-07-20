using Azure;
using Azure.Data.Tables;
using System;

namespace AzureTable
{
    internal class Program
    {
        class PersonEntity : ITableEntity
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public string Country { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
        }

        static void Main(string[] args)
        {
            var connectionString = "UseDevelopmentStorage=true";
            var tableName = "Demo1";

            var client = new TableClient(connectionString, tableName);

            // Create the table if it doesn't already exist to verify we've successfully authenticated.
            client.CreateIfNotExists();

            // AddEntity(client);
            // UpdateEntity(client, "User", "1");
            // GetPersonEntities(client, "User");
            DeleteEntity(client, "User", "2");
        }

        static void AddEntity(TableClient client)
        {
            PersonEntity personEntity = new PersonEntity
            {
                PartitionKey = "User",
                RowKey = "1",
                FirstName = "John",
                LastName = "Doe",
                Age = 20,
                Country = "PH"
            };
            client.AddEntity(personEntity);
        }

        static void UpdateEntity(TableClient client, string partitionKey, string rowKey)
        {
            PersonEntity personEntity = client.GetEntity<PersonEntity>(partitionKey, rowKey);
            personEntity.FirstName = "Jane";
            client.UpdateEntity(personEntity, ETag.All, TableUpdateMode.Replace);
        }

        static void GetPersonEntities(TableClient client, string partitionKey)
        {
            // Using TableEntity from Azure.Data.Tables
            Pageable<TableEntity> oDataQueryEntities = client.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {partitionKey}"));
            foreach (TableEntity entity in oDataQueryEntities)
            {
                Console.WriteLine($"TableEntity : {entity.GetString("PartitionKey")}:{entity.GetString("RowKey")}, {entity.GetString("FirstName")}, {entity.GetString("LastName")}");
            }

            // Using custom entity
            Pageable<PersonEntity> oDataQueryEntities2 = client.Query<PersonEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {partitionKey}"));
            foreach (PersonEntity entity in oDataQueryEntities2)
            {
                Console.WriteLine($"CustomEntity : {entity.PartitionKey}:{entity.RowKey}, {entity.FirstName}, {entity.LastName}");
            }

            // Using LINQ
            Pageable<PersonEntity> linqEntities = client.Query<PersonEntity>(customer => customer.PartitionKey == "User");
            foreach (PersonEntity entity in linqEntities)
            {
                Console.WriteLine($"LINQ : {entity.RowKey} {entity.PartitionKey}");
            }
        }

        static void DeleteEntity(TableClient client, string partitionKey, string rowKey)
        {
            Response response = client.DeleteEntity(partitionKey, rowKey);           
        }


    }
}
