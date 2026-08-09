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
    /// 商品查询服务（Product Query）
    /// </summary>
    public class ProductQueryService : IProductQueryService
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public ProductQueryService(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
            foreach (var sku in skus)
            {
                sku.ImageUrl = ReadImageAsBase64(sku.ImageUrl);
                sku.AttrValues = _context.SkuAttrValues
                    .Where(v => v.SkuId == sku.Id && !v.IsDeleted)
                    .OrderBy(v => v.Id)
                    .Select(v => new SpuAttrValueResponse
                    {
                        AttrId = v.AttrId,
                        Name = v.Name,
                        Value = v.Value
                    })
                    .ToList();
            }

            // SPU 属性值
            var spuAttrValues = _context.SpuAttrValues
                .Where(v => v.SpuId == id && !v.IsDeleted)
                .OrderBy(v => v.Id)
                .Select(v => new SpuAttrValueResponse
                {
                    AttrId = v.AttrId,
                    Name = v.Name,
                    Value = v.Value
                })
                .ToList();

            // 规格组（从所有 SKU 的规格值反推）
            var specGroups = new List<SpecGroupResponse>();
            foreach (var sku in skus)
            {
                foreach (var attr in sku.AttrValues)
                {
                    var group = specGroups.FirstOrDefault(g => g.Name == attr.Name);
                    if (group == null)
                    {
                        group = new SpecGroupResponse { AttrId = attr.AttrId, Name = attr.Name };
                        specGroups.Add(group);
                    }
                    if (!group.Values.Contains(attr.Value))
                    {
                        group.Values.Add(attr.Value);
                    }
                }
            }

            var imagePaths = _context.SpuImages
                .Where(i => i.SpuId == id && !i.IsDeleted)
                .OrderBy(i => i.SortOrder)
                .Select(i => i.ImageUrl)
                .Where(u => u != null)
                .ToList();
            var images = imagePaths.Select(ReadImageAsBase64).Where(b => b != null).ToList();

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
                Images = images,
                SpuAttrValues = spuAttrValues,
                SpecGroups = specGroups
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
            foreach (var brand in items)
            {
                brand.Logo = ReadImageAsBase64(brand.Logo);
            }

            return new PagedResponse<BrandResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public List<AttrResponse> GetAttrList(long? categoryId, int attrType)
        {
            var query = _context.Attrs.Where(a => !a.IsDeleted && a.Status == AttrStatus.Enable);

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }
            if (attrType >= 0)
            {
                var enumAttrType = (AttrType)attrType;
                if (Enum.IsDefined(typeof(AttrType), enumAttrType))
                {
                    query = query.Where(a => a.AttrType == enumAttrType || a.AttrType == AttrType.Both);
                }
            }

            return query
                .OrderBy(a => a.Id)
                .Select(a => new AttrResponse
                {
                    Id = a.Id,
                    CategoryId = a.CategoryId,
                    Name = a.Name,
                    AttrType = (int)a.AttrType,
                    ValueSelect = a.ValueSelect,
                    Status = (int)a.Status
                })
                .ToList();
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
