﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace CustomeResultDemo
{
    public class TestActionResult<T> : ActionResult
    {
        public bool Status { get; set; } = true;
        public T Data { get; set; }
        public string Message { get; set; }
        public TestActionResult()
        {
        }
        public TestActionResult(JsonSerializerOptions serializerSettings)
        {
            _serializerSettings = serializerSettings;
        }
        readonly JsonSerializerOptions  _serializerSettings;
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<IActionResultExecutor<JsonResult>>();           
            await executor.ExecuteAsync(context, new JsonResult(this, _serializerSettings));
        }
    }

}
