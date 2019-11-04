using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Common.Models;
using Common.Interfaces;
using Microsoft.ApplicationInsights;

namespace FrontendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrontendTodoController : Controller
    {
        private IAuthToken _authToken;
        private IHttpClient _httpClient;
        private IConfiguration _configuration;
        private string _backendUrl;
        private TelemetryClient _telemetryClient;

        public FrontendTodoController(IConfiguration configuration, IHttpClient httpClient, IAuthToken authToken, TelemetryClient telemetryClient)
        {
            _authToken = authToken;
            _httpClient = httpClient;
            _configuration = configuration;
            _backendUrl = $"{configuration["backendurl"]}/api/BackendTodo";
            _telemetryClient = telemetryClient;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            //_httpClient.SetAuthenticationHeader("Bearer", Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"]);

            var accessTokenResult = _authToken.GetOnBehalfOf(
                _configuration["AzureAd:TenantId"],
                _configuration["AzureAd:ClientId"],
                _configuration["AzureAd:ClientSecret"],
                Request.Headers["X-MS-TOKEN-AAD-ID-TOKEN"],
                new string[] { _configuration["AzureAd:BackendScope"] }
            ).ContinueWith((r) =>
            {
                return r.Result;
            }).Result;

            _httpClient.SetAuthenticationHeader("Bearer", accessTokenResult.AccessToken);
        }

        [HttpGet]
        public async Task<IEnumerable<TodoItem>> GetAll()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_backendUrl}");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"GetAll failed with the status code {response.StatusCode}. Message: {response.ReasonPhrase}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<IEnumerable<TodoItem>>(responseContent);
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
                var response = await _httpClient.GetAsync($"{_backendUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"GetAll failed with the status code {response.StatusCode}. Message: {response.ReasonPhrase}");

                var responseContent = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<TodoItem>(responseContent);

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