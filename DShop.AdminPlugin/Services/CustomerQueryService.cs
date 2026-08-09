using System;
using System.Collections.Generic;
using System.Linq;
using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.Models;
using DShop.PluginShared;
using Microsoft.Extensions.Configuration;

namespace DShop.AdminPlugin.Services
{
    /// <summary>
    /// 客户查询服务（Customer Query）
    /// </summary>
    public class CustomerQueryService : ICustomerQueryService
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public CustomerQueryService(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public PagedResponse<CustomerListResponse> GetCustomerList(string? keyword, int pageIndex, int pageSize)
        {
            var query = _context.Customers.Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(c =>
                    (c.Mobile != null && c.Mobile.Contains(keyword)) ||
                    (c.Nickname != null && c.Nickname.Contains(keyword)) ||
                    (c.Email != null && c.Email.Contains(keyword)));
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerListResponse
                {
                    Id = c.Id,
                    Mobile = c.Mobile,
                    Nickname = c.Nickname,
                    Email = c.Email,
                    Coin = c.Coin,
                    Gender = (int)c.Gender,
                    Status = (int)c.Status,
                    CreatedAt = c.CreatedAt
                })
                .ToList();

            return new PagedResponse<CustomerListResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public CustomerDetailResponse? GetCustomerDetail(long id, out string msg)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
            if (customer == null)
            {
                msg = "客户不存在";
                return null;
            }

            var addressCount = _context.DeliveryAddresses.Count(a => a.CustomerId == id && !a.IsDeleted);
            var orderCount = _context.Orders.Count(o => o.CustomerId == id && !o.IsDeleted);
            var collectCount = _context.CollectSpus.Count(c => c.CustomerId == id && !c.IsDeleted);

            msg = "获取成功";
            return new CustomerDetailResponse
            {
                Id = customer.Id,
                Mobile = customer.Mobile,
                Nickname = customer.Nickname,
                Email = customer.Email,
                Idiograph = customer.Idiograph,
                Coin = customer.Coin,
                Gender = (int)customer.Gender,
                Avatar = ReadImageAsBase64(customer.Avatar),
                Address = customer.Address,
                Status = (int)customer.Status,
                CreatedAt = customer.CreatedAt,
                AddressCount = addressCount,
                OrderCount = orderCount,
                CollectCount = collectCount
            };
        }

        public List<DeliveryAddressResponse> GetCustomerAddresses(long customerId)
        {
            return _context.DeliveryAddresses
                .Where(a => a.CustomerId == customerId && !a.IsDeleted)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .Select(a => new DeliveryAddressResponse
                {
                    Id = a.Id,
                    ContactPerson = a.ContactPerson,
                    Mobile = a.Mobile,
                    ProvinceCode = a.ProvinceCode,
                    CityCode = a.CityCode,
                    DistrictCode = a.DistrictCode,
                    DetailedAddress = a.DetailedAddress,
                    IsDefault = a.IsDefault,
                    CreatedAt = a.CreatedAt
                })
                .ToList();
        }

        public PagedResponse<CoinRecordResponse> GetCoinRecords(long customerId, int pageIndex, int pageSize)
        {
            var query = _context.CoinRecords.Where(r => r.CustomerId == customerId && !r.IsDeleted);

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new CoinRecordResponse
                {
                    Id = r.Id,
                    Type = (int)r.Type,
                    Title = r.Title,
                    Amount = r.Amount,
                    Remark = r.Remark,
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            return new PagedResponse<CoinRecordResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public PagedResponse<BrowsingSpuResponse> GetBrowsingSpus(long customerId, int pageIndex, int pageSize)
        {
            var query = _context.BrowsingSpus.Where(b => b.CustomerId == customerId && !b.IsDeleted);

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BrowsingSpuResponse
                {
                    Id = b.Id,
                    SpuId = b.SpuId,
                    SpuName = b.SpuName,
                    SpuPrice = b.SpuPrice,
                    SpuImageUrl = b.SpuImageUrl,
                    CreatedAt = b.CreatedAt
                })
                .ToList();

            return new PagedResponse<BrowsingSpuResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public PagedResponse<CollectSpuResponse> GetCollectSpus(long customerId, int pageIndex, int pageSize)
        {
            var query = _context.CollectSpus.Where(c => c.CustomerId == customerId && !c.IsDeleted);

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CollectSpuResponse
                {
                    Id = c.Id,
                    SpuId = c.SpuId,
                    SpuName = c.SpuName,
                    SpuPrice = c.SpuPrice,
                    SpuImageUrl = c.SpuImageUrl,
                    CreatedAt = c.CreatedAt
                })
                .ToList();

            return new PagedResponse<CollectSpuResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 读取相对路径图片并转成Base64（用于返回给前端展示）
        /// </summary>
        private string? ReadImageAsBase64(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return null;
            }
            try
            {
                string basePath = _configuration[Constants.FileStorageBasePath] ?? "D:/Uploads/";
                string fullDir = basePath + relativePath;
                return ImageToBase64.GetBase64FromImage(fullDir);
            }
            catch
            {
                return null;
            }
        }
    }
}
