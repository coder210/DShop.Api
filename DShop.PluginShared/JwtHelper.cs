using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DShop.PluginShared
{
    public class JwtHelper
    {
        private static string _secretKey = "d0ecd23c-dfdb-4005-a2ea-0fea220c858a22222222222";
        private static string _issuer = "everyone";
        private static string _audience = "gsw";

        /// <summary>
        /// 从配置文件初始化 JWT 配置（需在 Program.cs 启动时调用，与 JWT 中间件保持一致）
        /// </summary>
        public static void Configure(string secretKey, string issuer, string audience)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        public static string GenerateJwtToken(string userId, string userName, string[] permissionIds, int expireMinutes = 30, string[]? roles = null)
        {
            // 1. 创建安全密钥
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 2. 添加声明（Claims）
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),            // 主题（用户ID）
            new Claim(JwtRegisteredClaimNames.Name, userName),   // 用户名
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID，用于防止重放攻击
            // 将权限ID列表合并为一个字符串，用逗号分隔
            new Claim("permissions", string.Join(",", permissionIds))  // 自定义声明类型
        };

            // 角色编码列表合并为一个字符串，用逗号分隔
            if (roles != null && roles.Length > 0)
            {
                claims.Add(new Claim("roles", string.Join(",", roles)));
            }

            // 3. 设置令牌过期时间
            var expires = DateTime.Now.AddMinutes(expireMinutes);

            // 4. 创建 JWT 描述符
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = credentials
                 
            };

            // 5. 生成令牌
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public static ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,  // 验证过期时间
                ClockSkew = TimeSpan.Zero // 允许的时间偏移（默认5分钟）
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch
            {
                return null; // 验证失败
            }
        }
    }
}
