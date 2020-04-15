﻿using System.Threading.Tasks;
using Grpc.Core;
using GRPCDemo01Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRPCDemo01WebTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        /// <summary>
        /// 客户端
        /// </summary>
        private readonly Goodser.GoodserClient _client;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, Goodser.GoodserClient client)
        {
            _client = client;
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            //登录
            var tokenResponse = await _client.LoginAsync(new LoginRequest { Username = "gsw", Password = "111111" });
            var token = $"Bearer {tokenResponse.Token }";
            var headers = new Metadata { { "Authorization", token } };
            var request = new QueryRequest { Name = "桂素伟" };
            //查询
            var query = await _client.GetGoodsAsync(request, headers);
            return $"Name:{query.Name},Quantity:{query.Quantity}";
        }
    }
}