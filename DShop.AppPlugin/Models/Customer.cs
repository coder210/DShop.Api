using System.ComponentModel.DataAnnotations;

namespace DShop.AppPlugin.Models;

/// <summary>
/// 顾客表（前端 App 用户），独立表，不挂后台 RBAC。
/// TODO: 用你现成的顾客实体替换此占位。
/// </summary>
public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(128)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(32)]
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
