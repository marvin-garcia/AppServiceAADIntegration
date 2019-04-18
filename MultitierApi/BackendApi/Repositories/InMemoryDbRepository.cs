using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BackendApi.Models;
using BackendApi.Interfaces;

namespace BackendApi.Repositories
{
    public class InMemoryDbRepository : DbContext, IDbContext
    {
        private readonly string _contextType;
        private DbSet<TodoItem> _todoItems { get; set; }
        
        public InMemoryDbRepository(DbContextOptions<InMemoryDbRepository> options) : base(options)
        {
            this._contextType = "InMemory";
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
            return _todoItems.ToList();
        }

        public TodoItem Get(string id)
        {
            return _todoItems.FirstOrDefault(x => x.Id == id);
        }

        public TodoItem Create(TodoItem item)
        {
            item.Id = string.IsNullOrEmpty(item.Id) ? GenerateId() : item.Id;
            _todoItems.Add(item);
            SaveChanges();

            return item;
        }

        public void Update(string id, TodoItem itemIn)
        {
            var todo = _todoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                throw new EntryPointNotFoundException();

            todo.IsComplete = itemIn.IsComplete;
            todo.Name = itemIn.Name;

            _todoItems.Update(todo);
            SaveChanges();
        }

        public void Remove(TodoItem itemIn)
        {
            Remove(itemIn.Id);
        }

        public void Remove(string id)
        {
            var todo = _todoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                throw new EntryPointNotFoundException();

            _todoItems.Remove(todo);
            SaveChanges();
        }
    }
}
