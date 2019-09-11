using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionController : Controller
    {
        private HttpClient _httpClient;
        private string _functionUrl;

        public FunctionController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _functionUrl = configuration["functionurl"];
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"]);
        }

        // GET: api/Function/whatever
        [HttpGet("{id}", Name = "Get")]
        public async Task<OkObjectResult> Get(string id)
        {
            var response = await _httpClient.GetAsync($"{_functionUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Function.Get failed with status code {response.StatusCode}. Message: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseContent);
        }
    }
}
