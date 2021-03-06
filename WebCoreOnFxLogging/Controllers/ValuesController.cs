﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebCoreOnFxLogging.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger<ValuesController> _log;
        public ValuesController(ILogger<ValuesController> log)
        {
            _log = log;
        }

        // GET api/values
        [HttpGet]
        public async ValueTask<IEnumerable<string>> GetAsync()
        {
            using (_log.BeginScope("Just get"))
            {
                await Task.Delay(1000);
                return new string[] { "value1", "value2" };
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> GetAsync(int id)
        {
            using (_log.BeginScope("Just get"))
            {
                await Task.Delay(1000);
                return $"value {id}";
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
