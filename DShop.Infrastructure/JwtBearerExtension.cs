using DShop.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DShop.Infrastructure
{
    public static class JwtBearerExtension
    {
        /// <summary>
        /// 注入Ocelot下JwtBearer
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="issuer">发行人</param>
        /// <param name="audience">订阅人</param>
        /// <param name="secret">密钥</param>
        /// <param name="defaultScheme">默认架构</param>
        /// <param name="isHttps">是否https</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwtBearer(this IServiceCollection services, string issuer, string audience, string secret, string defaultScheme, bool isHttps = false)
        {
            var keyByteArray = Encoding.ASCII.GetBytes(secret);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = issuer,//发行人
                ValidateAudience = true,
                ValidAudience = audience,//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            return services.AddAuthentication(options =>
            {
                options.DefaultScheme = defaultScheme;
            })
             .AddJwtBearer(defaultScheme, opt =>
             {
                 //不使用https
                 opt.RequireHttpsMetadata = isHttps;
                 opt.TokenValidationParameters = tokenValidationParameters;
                 opt.Events = new JwtBearerEvents 
                 {
                     //此处为权限验证失败后触发的事件
                     //OnChallenge = context =>
                     //{
                     //    //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
                     //    context.HandleResponse();
                     //    //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换
                     //    var payload = JsonConvert.SerializeObject(new R
                     //    {
                     //        Code = RC.UNAUTH,
                     //        Msg = "授权校验未通过",
                     //        Data = string.Empty,
                     //    });
                     //    //自定义返回的数据类型
                     //    context.Response.ContentType = "application/json";
                     //    //自定义返回状态码，默认为401 我这里改成 200
                     //    context.Response.StatusCode = StatusCodes.Status200OK;
                     //    //输出Json数据结果
                     //    context.Response.WriteAsync(payload);
                     //    return Task.FromResult(0);
                     //}
                 };
             });
        }

    }
}
