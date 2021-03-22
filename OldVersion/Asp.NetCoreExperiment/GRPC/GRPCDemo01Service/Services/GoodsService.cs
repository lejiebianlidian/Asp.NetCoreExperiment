using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCDemo01Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace GRPCDemo01Service
{
    [Authorize("Permission")]
    public class GoodsService : Goodser.GoodserBase
    {
        private readonly ILogger<GoodsService> _logger;
        readonly PermissionRequirement _requirement;
        public GoodsService(ILogger<GoodsService> logger, PermissionRequirement requirement)
        {
            _requirement = requirement;
            _logger = logger;
        }
        public override Task<QueryResponse> GetGoods(QueryRequest request, ServerCallContext context)
        {
            return Task.FromResult(new QueryResponse
            {
                Name = "Hello " + request.Name,
                Quantity = 10
            });
        }
        [AllowAnonymous]
        public override Task<LoginResponse> Login(LoginRequest user, ServerCallContext context)
        {
            //todo ��ѯ���ݿ�˶��û�������
            var isValidated = user.Username == "gsw" && user.Password == "111111";
            if (!isValidated)
            {
                return Task.FromResult(new LoginResponse()
                {
                    Message = "��֤ʧ��"
                });
            }
            else
            {
                //����ǻ����û�����Ȩ���ԣ�����Ҫ����û�;����ǻ��ڽ�ɫ����Ȩ���ԣ�����Ҫ��ӽ�ɫ
                var claims = new Claim[] {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString())
                };

                var token = JwtToken.BuildJwtToken(claims, _requirement);
                return Task.FromResult(new LoginResponse()
                {
                    Result = true,
                    Token = token.access_token
                });

            }
        }
    }
}
