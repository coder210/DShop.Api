using System;
using System.Collections.Generic;
using System.Linq;
using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.Models;
using DShop.PluginShared;
using Microsoft.EntityFrameworkCore;

namespace DShop.AdminPlugin.Services
{
    /// <summary>
    /// 商品命令服务（Product Command）
    /// </summary>
    public class ProductCommandService : IProductCommandService
    {
        private readonly DatabaseContext _context;
        private readonly IUserContext _userContext;

        public ProductCommandService(DatabaseContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public (bool Success, string Message) SaveSpu(CreateOrUpdateSpuRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (false, "商品名称不能为空");
            }
            if (request.CategoryId <= 0)
            {
                return (false, "请选择分类");
            }
            if (request.BrandId <= 0)
            {
                return (false, "请选择品牌");
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var now = DateTime.Now;
                var userId = _userContext.CurrentUserId;

                Spu spu;
                if (request.Id > 0)
                {
                    spu = _context.Spus.FirstOrDefault(s => s.Id == request.Id && !s.IsDeleted);
                    if (spu == null)
                    {
                        return (false, "商品不存在");
                    }
                    spu.Name = request.Name;
                    spu.CategoryId = request.CategoryId;
                    spu.BrandId = request.BrandId;
                    spu.Weight = request.Weight;
                    spu.Desc = request.Desc;
                    spu.Status = (SpuStatus)request.Status;
                    spu.ModifiedBy = userId;
                    spu.ModifiedAt = now;
                }
                else
                {
                    spu = new Spu
                    {
                        Name = request.Name,
                        CategoryId = request.CategoryId,
                        BrandId = request.BrandId,
                        Weight = request.Weight,
                        Desc = request.Desc,
                        Status = (SpuStatus)request.Status,
                        IsDeleted = false,
                        CreatedBy = userId,
                        ModifiedBy = userId,
                        CreatedAt = now,
                        ModifiedAt = now
                    };
                    _context.Spus.Add(spu);
                }
                _context.SaveChanges();

                // 整体替换 SKU
                var oldSkus = _context.Skus.Where(s => s.SpuId == spu.Id && !s.IsDeleted).ToList();
                foreach (var old in oldSkus)
                {
                    old.IsDeleted = true;
                    old.ModifiedBy = userId;
                    old.ModifiedAt = now;
                }
                foreach (var skuReq in request.Skus)
                {
                    _context.Skus.Add(new Sku
                    {
                        SpuId = spu.Id,
                        ImageUrl = skuReq.ImageUrl,
                        Price = skuReq.Price,
                        SaleCount = 0,
                        BarCode = skuReq.BarCode,
                        QrCode = skuReq.QrCode,
                        IsDeleted = false,
                        CreatedBy = userId,
                        ModifiedBy = userId,
                        CreatedAt = now,
                        ModifiedAt = now
                    });
                }

                // 整体替换图片
                var oldImages = _context.SpuImages.Where(i => i.SpuId == spu.Id && !i.IsDeleted).ToList();
                foreach (var old in oldImages)
                {
                    old.IsDeleted = true;
                    old.ModifiedBy = userId;
                    old.ModifiedAt = now;
                }
                for (int i = 0; i < request.Images.Count; i++)
                {
                    _context.SpuImages.Add(new SpuImage
                    {
                        SpuId = spu.Id,
                        ImageUrl = request.Images[i],
                        SortOrder = i,
                        IsDeleted = false,
                        CreatedBy = userId,
                        ModifiedBy = userId,
                        CreatedAt = now,
                        ModifiedAt = now
                    });
                }

                _context.SaveChanges();
                transaction.Commit();
                return (true, request.Id > 0 ? "更新成功" : "创建成功");
            }
            catch
            {
                transaction.Rollback();
                return (false, "保存失败");
            }
        }

        public (bool Success, string Message) UpdateSpuStatus(UpdateSpuStatusRequest request)
        {
            var spu = _context.Spus.FirstOrDefault(s => s.Id == request.Id && !s.IsDeleted);
            if (spu == null)
            {
                return (false, "商品不存在");
            }
            if (!Enum.IsDefined(typeof(SpuStatus), request.Status))
            {
                return (false, "无效的状态");
            }

            spu.Status = (SpuStatus)request.Status;
            spu.ModifiedBy = _userContext.CurrentUserId;
            spu.ModifiedAt = DateTime.Now;
            _context.SaveChanges();
            return (true, "操作成功");
        }

        public (bool Success, string Message) DeleteSpu(long id)
        {
            var spu = _context.Spus.FirstOrDefault(s => s.Id == id && !s.IsDeleted);
            if (spu == null)
            {
                return (false, "商品不存在");
            }

            var now = DateTime.Now;
            var userId = _userContext.CurrentUserId;
            spu.IsDeleted = true;
            spu.ModifiedBy = userId;
            spu.ModifiedAt = now;

            foreach (var sku in _context.Skus.Where(s => s.SpuId == id && !s.IsDeleted))
            {
                sku.IsDeleted = true;
            }
            foreach (var img in _context.SpuImages.Where(i => i.SpuId == id && !i.IsDeleted))
            {
                img.IsDeleted = true;
            }

            _context.SaveChanges();
            return (true, "删除成功");
        }

        public (bool Success, string Message) SaveCategory(CreateOrUpdateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (false, "分类名称不能为空");
            }

            var now = DateTime.Now;
            var userId = _userContext.CurrentUserId;

            if (request.Id > 0)
            {
                var category = _context.Categories.FirstOrDefault(c => c.Id == request.Id && !c.IsDeleted);
                if (category == null)
                {
                    return (false, "分类不存在");
                }
                category.Name = request.Name;
                category.Icon = request.Icon;
                category.SortOrder = request.SortOrder;
                category.Status = (CategoryStatus)request.Status;
                category.ModifiedBy = userId;
                category.ModifiedAt = now;
            }
            else
            {
                var level = request.ParentId > 0
                    ? (_context.Categories.FirstOrDefault(c => c.Id == request.ParentId && !c.IsDeleted)?.Level ?? 0) + 1
                    : 1;
                _context.Categories.Add(new Category
                {
                    ParentId = request.ParentId,
                    Name = request.Name,
                    Icon = request.Icon,
                    Level = level,
                    SortOrder = request.SortOrder,
                    Status = (CategoryStatus)request.Status,
                    IsDeleted = false,
                    CreatedBy = userId,
                    ModifiedBy = userId,
                    CreatedAt = now,
                    ModifiedAt = now
                });
            }

            _context.SaveChanges();
            return (true, request.Id > 0 ? "更新成功" : "创建成功");
        }

        public (bool Success, string Message) DeleteCategory(long id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
            if (category == null)
            {
                return (false, "分类不存在");
            }

            var hasChildren = _context.Categories.Any(c => c.ParentId == id && !c.IsDeleted);
            if (hasChildren)
            {
                return (false, "该分类下有子分类，无法删除");
            }
            var hasSpu = _context.Spus.Any(s => s.CategoryId == id && !s.IsDeleted);
            if (hasSpu)
            {
                return (false, "该分类下存在商品，无法删除");
            }

            var now = DateTime.Now;
            category.IsDeleted = true;
            category.ModifiedBy = _userContext.CurrentUserId;
            category.ModifiedAt = now;
            _context.SaveChanges();
            return (true, "删除成功");
        }

        public (bool Success, string Message) SaveBrand(CreateOrUpdateBrandRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (false, "品牌名称不能为空");
            }

            var now = DateTime.Now;
            var userId = _userContext.CurrentUserId;

            if (request.Id > 0)
            {
                var brand = _context.Brands.FirstOrDefault(b => b.Id == request.Id && !b.IsDeleted);
                if (brand == null)
                {
                    return (false, "品牌不存在");
                }
                brand.Name = request.Name;
                brand.Logo = request.Logo;
                brand.Desc = request.Desc;
                brand.Status = (BrandStatus)request.Status;
                brand.SortOrder = request.SortOrder;
                brand.ModifiedBy = userId;
                brand.ModifiedAt = now;
            }
            else
            {
                var firstLetter = request.Name.Length > 0 ? request.Name.Substring(0, 1) : "";
                _context.Brands.Add(new Brand
                {
                    Name = request.Name,
                    Logo = request.Logo,
                    Desc = request.Desc,
                    FirstLetter = firstLetter,
                    Status = (BrandStatus)request.Status,
                    SortOrder = request.SortOrder,
                    IsDeleted = false,
                    CreatedBy = userId,
                    ModifiedBy = userId,
                    CreatedAt = now,
                    ModifiedAt = now
                });
            }

            _context.SaveChanges();
            return (true, request.Id > 0 ? "更新成功" : "创建成功");
        }

        public (bool Success, string Message) DeleteBrand(long id)
        {
            var brand = _context.Brands.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
            if (brand == null)
            {
                return (false, "品牌不存在");
            }

            var hasSpu = _context.Spus.Any(s => s.BrandId == id && !s.IsDeleted);
            if (hasSpu)
            {
                return (false, "该品牌下存在商品，无法删除");
            }

            var now = DateTime.Now;
            brand.IsDeleted = true;
            brand.ModifiedBy = _userContext.CurrentUserId;
            brand.ModifiedAt = now;
            _context.SaveChanges();
            return (true, "删除成功");
        }
    }
}
