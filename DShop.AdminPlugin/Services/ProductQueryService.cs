using System;
using System.Collections.Generic;
using System.Linq;
using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.Models;

namespace DShop.AdminPlugin.Services
{
    /// <summary>
    /// 商品查询服务（Product Query）
    /// </summary>
    public class ProductQueryService : IProductQueryService
    {
        private readonly DatabaseContext _context;

        public ProductQueryService(DatabaseContext context)
        {
            _context = context;
        }

        public PagedResponse<SpuListResponse> GetSpuList(string? keyword, long? categoryId, int status, int pageIndex, int pageSize)
        {
            var query = _context.Spus.Where(s => !s.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(s => s.Name.Contains(keyword));
            }
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(s => s.CategoryId == categoryId.Value);
            }
            if (status >= 0)
            {
                var enumStatus = (SpuStatus)status;
                if (Enum.IsDefined(typeof(SpuStatus), enumStatus))
                {
                    query = query.Where(s => s.Status == enumStatus);
                }
            }

            var totalCount = query.Count();

            var categoryNames = _context.Categories
                .Where(c => !c.IsDeleted)
                .ToDictionary(c => c.Id, c => c.Name);
            var brandNames = _context.Brands
                .Where(b => !b.IsDeleted)
                .ToDictionary(b => b.Id, b => b.Name);

            var items = query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SpuListResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    CategoryId = s.CategoryId,
                    BrandId = s.BrandId,
                    Status = (int)s.Status,
                    CreatedAt = s.CreatedAt
                })
                .ToList();

            foreach (var item in items)
            {
                categoryNames.TryGetValue(item.CategoryId, out var cname);
                item.CategoryName = cname;
                brandNames.TryGetValue(item.BrandId, out var bname);
                item.BrandName = bname;
            }

            return new PagedResponse<SpuListResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public SpuDetailResponse? GetSpuDetail(long id, out string msg)
        {
            var spu = _context.Spus.FirstOrDefault(s => s.Id == id && !s.IsDeleted);
            if (spu == null)
            {
                msg = "商品不存在";
                return null;
            }

            var skus = _context.Skus
                .Where(s => s.SpuId == id && !s.IsDeleted)
                .OrderBy(s => s.Id)
                .Select(s => new SkuResponse
                {
                    Id = s.Id,
                    SpuId = s.SpuId,
                    ImageUrl = s.ImageUrl,
                    Price = s.Price,
                    SaleCount = s.SaleCount,
                    BarCode = s.BarCode,
                    QrCode = s.QrCode
                })
                .ToList();

            var images = _context.SpuImages
                .Where(i => i.SpuId == id && !i.IsDeleted)
                .OrderBy(i => i.SortOrder)
                .Select(i => i.ImageUrl)
                .Where(u => u != null)
                .ToList();

            msg = "获取成功";
            return new SpuDetailResponse
            {
                Id = spu.Id,
                Name = spu.Name,
                CategoryId = spu.CategoryId,
                BrandId = spu.BrandId,
                Weight = spu.Weight,
                Desc = spu.Desc,
                Status = (int)spu.Status,
                CreatedAt = spu.CreatedAt,
                Skus = skus,
                Images = images
            };
        }

        public List<CategoryTreeResponse> GetCategoryTree()
        {
            var all = _context.Categories
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.SortOrder)
                .ToList();
            return BuildCategoryTree(all.Where(c => c.ParentId == 0).ToList(), all);
        }

        private List<CategoryTreeResponse> BuildCategoryTree(List<Category> parentNodes, List<Category> all)
        {
            var result = new List<CategoryTreeResponse>();
            foreach (var parent in parentNodes)
            {
                var node = new CategoryTreeResponse
                {
                    Id = parent.Id,
                    ParentId = parent.ParentId,
                    Name = parent.Name,
                    Icon = parent.Icon,
                    Level = parent.Level,
                    SortOrder = parent.SortOrder,
                    Status = (int)parent.Status
                };
                var children = all.Where(c => c.ParentId == parent.Id).ToList();
                node.Children = BuildCategoryTree(children, all);
                result.Add(node);
            }
            return result;
        }

        public PagedResponse<BrandResponse> GetBrandList(string? keyword, int pageIndex, int pageSize)
        {
            var query = _context.Brands.Where(b => !b.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(b => b.Name.Contains(keyword));
            }

            var totalCount = query.Count();

            var items = query
                .OrderBy(b => b.SortOrder)
                .ThenByDescending(b => b.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BrandResponse
                {
                    Id = b.Id,
                    Name = b.Name,
                    Logo = b.Logo,
                    Desc = b.Desc,
                    FirstLetter = b.FirstLetter,
                    Status = (int)b.Status,
                    SortOrder = b.SortOrder
                })
                .ToList();

            return new PagedResponse<BrandResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
