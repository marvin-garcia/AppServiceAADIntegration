using System.Threading.Tasks;
using System.Collections.Generic;
using FrontendApi.Models;

namespace FrontendApi.Interfaces
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetAll();
        Task<TodoItem> Get(string id);
        Task<TodoItem> Create(TodoItem item);
        Task Update(string id, TodoItem itemIn);
        Task Remove(string id);
    }
}
