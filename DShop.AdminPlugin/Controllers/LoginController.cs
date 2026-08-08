using DShop.Contracts;
using DShop.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 登录页 - 用户登录
    /// </summary>
    [Route("api/admin/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IIdentityCommandService _identityCommandService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IIdentityCommandService identityCommandService, ILogger<LoginController> logger)
        {
            _identityCommandService = identityCommandService;
            _logger = logger;
        }

        /// <summary>
        /// 用户登录（获取令牌）
        /// </summary>
        [SwaggerOperation(Summary = "用户登录", Description = "用户通过帐号和密码登录")]
        [SwaggerResponse(200, "登录成功", typeof(ApiResponse))]
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Ok(new ApiResponse { Code = 400, Msg = "用户名和密码不能为空" });
            }
            _logger.LogInformation("Login Contorller Update1");
            var loginResult = _identityCommandService.Login(request.UserName, request.Password, request.Captcha);
            if (loginResult.Success)
            {
                return Ok(new ApiResponse { Code = 200, Data = loginResult.Token, Msg = "登录成功" });
            }
            return Ok(new ApiResponse { Code = 400, Data = loginResult.Token, Msg = "登录失败" });
        }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Captcha { get; set; }
    }
}
