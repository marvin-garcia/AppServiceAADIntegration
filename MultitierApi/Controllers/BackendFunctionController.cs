using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrontendApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace FrontendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackendFunctionController : Controller
    {
        private IHttpClient _httpClient;
        private string _backendUrl;

        public BackendFunctionController(IConfiguration configuration, IHttpClient httpClient)
        {
            _httpClient = httpClient;
            _backendUrl = $"{configuration["backendurl"]}/api/BackendFunction";
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _httpClient.SetAuthenticationHeader("Bearer", Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"]);
        }

        // GET: api/Function/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<OkObjectResult> Get(string id)
        {
            var response = await _httpClient.GetAsync($"{_backendUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Function.Get failed with status code {response.StatusCode}. Message: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseContent);
        }
    }
}
