using System;
using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using BackendApi.Models;
using BackendApi.Interfaces;

namespace BackendApi.Repositories
{
    public class MongoDbRepository : IDbContext
    {
        private readonly string _contextType;
        private readonly IMongoCollection<TodoItem> _items;

        public MongoDbRepository(IConfiguration configuration)
        {
            this._contextType = "MongoDB";

            string connectionString = configuration["mongodb"];
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Could not find database connection string.");

            string databaseName = configuration["mongodatabase"];
            if (string.IsNullOrEmpty(databaseName))
                throw new Exception("Could not find database name.");

            string collectionName = configuration["mongocollection"];
            if (string.IsNullOrEmpty(collectionName))
                throw new Exception("Could not find database collection name.");

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _items = database.GetCollection<TodoItem>(collectionName);
        }

        private string GenerateId()
        {
            string guid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 24);
            return guid;
        }

        public string GetDbContextType()
        {
            return this._contextType;
        }

        public List<TodoItem> Get()
        {
            return _items.Find(item => true).ToList();
        }

        public TodoItem Get(string id)
        {
            return _items.Find<TodoItem>(item => item.Id == id).FirstOrDefault();
        }

        public TodoItem Create(TodoItem item)
        {
            item.Id = string.IsNullOrEmpty(item.Id) ? GenerateId() : item.Id;
            _items.InsertOne(item);
            return item;
        }

        public void Update(string id, TodoItem itemIn)
        {
            _items.ReplaceOne(item => item.Id == id, itemIn);
        }

        public void Remove(TodoItem itemIn)
        {
            _items.DeleteOne(item => item.Id == itemIn.Id);
        }

        public void Remove(string id)
        {
            _items.DeleteOne(item => item.Id == id);
        }
    }
}
