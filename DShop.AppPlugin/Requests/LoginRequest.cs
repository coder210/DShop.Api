using System.ComponentModel.DataAnnotations;

namespace DShop.AppPlugin.Requests;

/// <summary>
/// 顾客登录请求占位。
/// TODO: 用你现成的登录请求替换此占位。
/// </summary>
public class LoginRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
