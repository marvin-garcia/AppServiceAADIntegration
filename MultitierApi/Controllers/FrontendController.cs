using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FrontendApi.Interfaces;
using FrontendApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FrontendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrontendController : Controller
    {
        private IHttpClient _httpClient;
        private string _backendUrl;

        public FrontendController(IConfiguration configuration, IHttpClient httpClient)
        {
            _httpClient = httpClient;
            _backendUrl = $"{configuration["backendurl"]}/api/todo";
        }

        [HttpGet]
        public async Task<IEnumerable<TodoItem>> GetAll()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_backendUrl}");
                var items = JsonConvert.DeserializeObject<IEnumerable<TodoItem>>(response);
                return items;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("{id:length(24)}", Name = "GetTodo")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_backendUrl}/{id}");
                var item = JsonConvert.DeserializeObject<TodoItem>(response);

                return new ObjectResult(item);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoItem item)
        {
            try
            {
                var response = await _httpClient.PostAsync<TodoItem>(_backendUrl, item);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.ReasonPhrase);

                var itemOut = JsonConvert.DeserializeObject<TodoItem>(await response.Content.ReadAsStringAsync());
                return new ObjectResult(itemOut);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, [FromBody] TodoItem item)
        {
            try
            {
                //var content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync<TodoItem>($"{_backendUrl}/{id}", item);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.ReasonPhrase);

                return new NoContentResult();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_backendUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.ReasonPhrase);

                return new NoContentResult();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}