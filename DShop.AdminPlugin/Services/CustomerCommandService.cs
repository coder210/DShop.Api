using System;
using System.Linq;
using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.Models;
using DShop.PluginShared;

namespace DShop.AdminPlugin.Services
{
    /// <summary>
    /// 客户命令服务（Customer Command）
    /// </summary>
    public class CustomerCommandService : ICustomerCommandService
    {
        private readonly DatabaseContext _context;
        private readonly IUserContext _userContext;

        public CustomerCommandService(DatabaseContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public (bool Success, string Message) UpdateCustomerStatus(UpdateCustomerStatusRequest request)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == request.Id && !c.IsDeleted);
            if (customer == null)
            {
                return (false, "客户不存在");
            }

            if (!Enum.IsDefined(typeof(CustomerStatus), request.Status))
            {
                return (false, "无效的状态");
            }

            customer.Status = (CustomerStatus)request.Status;
            customer.ModifiedBy = _userContext.CurrentUserId;
            customer.ModifiedAt = DateTime.Now;

            _context.SaveChanges();
            return (true, "更新成功");
        }

        public (bool Success, string Message) CreateCustomer(CreateOrUpdateCustomerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Mobile))
            {
                return (false, "手机号不能为空");
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return (false, "请设置登录密码");
            }
            if (_context.Customers.Any(c => !c.IsDeleted && c.Mobile == request.Mobile))
            {
                return (false, "该手机号已存在");
            }

            var now = DateTime.Now;
            var userId = _userContext.CurrentUserId;
            var (salt, passwordHash) = HashPassword(request.Password);

            _context.Customers.Add(new Customer
            {
                Mobile = request.Mobile,
                Nickname = request.Nickname,
                Email = request.Email,
                Password = passwordHash,
                Salt = salt,
                Idiograph = null,
                Coin = request.Coin,
                Gender = (CustomerGender)request.Gender,
                Avatar = request.Avatar,
                Address = request.Address,
                Status = (CustomerStatus)request.Status,
                IsDeleted = false,
                CreatedBy = userId,
                ModifiedBy = userId,
                CreatedAt = now,
                ModifiedAt = now
            });

            _context.SaveChanges();
            return (true, "创建成功");
        }

        public (bool Success, string Message) UpdateCustomer(CreateOrUpdateCustomerRequest request)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == request.Id && !c.IsDeleted);
            if (customer == null)
            {
                return (false, "客户不存在");
            }
            if (_context.Customers.Any(c => !c.IsDeleted && c.Id != request.Id && c.Mobile == request.Mobile))
            {
                return (false, "该手机号已被其他客户使用");
            }

            var now = DateTime.Now;
            var userId = _userContext.CurrentUserId;

            customer.Mobile = request.Mobile;
            customer.Nickname = request.Nickname;
            customer.Email = request.Email;
            customer.Gender = (CustomerGender)request.Gender;
            customer.Avatar = request.Avatar;
            customer.Address = request.Address;
            customer.Coin = request.Coin;
            customer.Status = (CustomerStatus)request.Status;

            // 仅当传了新密码时重置密码
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var (salt, passwordHash) = HashPassword(request.Password);
                customer.Password = passwordHash;
                customer.Salt = salt;
            }

            customer.ModifiedBy = userId;
            customer.ModifiedAt = now;

            _context.SaveChanges();
            return (true, "更新成功");
        }

        /// <summary>
        /// 生成随机盐 + 加盐后的MD5密码哈希
        /// </summary>
        private static (string Salt, string Hash) HashPassword(string rawPassword)
        {
            var salt = Guid.NewGuid().ToString("N").Substring(0, 16);
            var hash = Md5Helper.ComputeMD5Hash(salt + rawPassword);
            return (salt, hash);
        }
    }
}
