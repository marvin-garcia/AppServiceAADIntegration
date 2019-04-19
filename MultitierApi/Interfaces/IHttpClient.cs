using System.Net.Http;
using System.Threading.Tasks;

namespace FrontendApi.Interfaces
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<string> GetStringAsync(string uri);
        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string requestId = null);
        Task<HttpResponseMessage> DeleteAsync(string uri, string requestId = null);
        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string requestId = null);
    }
}
