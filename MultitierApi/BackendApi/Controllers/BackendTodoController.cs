﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BackendApi.Interfaces;
using BackendApi.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web.Resource;

namespace BackendApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BackendTodoController : Controller
    {
        private IConfiguration _configuration;
        private TelemetryClient _telemetryClient;
        private readonly IDbContext _dbContext;

        public BackendTodoController(IDbContext dbRepository, IConfiguration configuration, TelemetryClient telemetryClient)
        {
            try
            {
                _dbContext = dbRepository;
                if (_dbContext.Get().Count() == 0)
                    _dbContext.Create(new TodoItem()
                    {
                        Name = "Item1",
                        IsComplete = false,
                    });

                _configuration = configuration;
                _telemetryClient = telemetryClient;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            HttpContext.VerifyUserHasAnyAcceptedScope(_configuration["AzureAd:RequiredScopes"].Split(','));
        }

        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            try
            {
                return _dbContext.Get();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("{id:length(24)}", Name = "GetTodo")]
        public IActionResult GetById(string id)
        {
            try
            {
                var item = _dbContext.Get(id);
                if (item == null)
                    return NotFound();

                return new ObjectResult(item);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            try
            {
                if (item == null)
                    return BadRequest();

                _dbContext.Create(item);
                return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, [FromBody] TodoItem item)
        {
            try
            {
                if (item == null || item.Id != id)
                    return BadRequest();

                var todo = _dbContext.Get(id);
                if (todo == null)
                    return NotFound();

                todo.IsComplete = item.IsComplete;
                todo.Name = item.Name;

                _dbContext.Update(id, todo);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var todo = _dbContext.Get(id);
                if (todo == null)
                    return NotFound();

                _dbContext.Remove(id);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("DbContextType")]
        public string GetDbContextType()
        {
            return _dbContext.GetDbContextType();
        }
    }
}
