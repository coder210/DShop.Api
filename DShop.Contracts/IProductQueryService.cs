using System.Collections.Generic;
using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    /// <summary>
    /// 商品查询服务（Product Query）
    /// </summary>
    public interface IProductQueryService
    {
        /// <summary>
        /// 商品SPU分页列表
        /// </summary>
        PagedResponse<SpuListResponse> GetSpuList(string? keyword, long? categoryId, int status, int pageIndex, int pageSize);

        /// <summary>
        /// 商品SPU详情（含SKU、图片）
        /// </summary>
        SpuDetailResponse? GetSpuDetail(long id, out string msg);

        /// <summary>
        /// 商品分类树
        /// </summary>
        List<CategoryTreeResponse> GetCategoryTree();

        /// <summary>
        /// 品牌分页列表
        /// </summary>
        PagedResponse<BrandResponse> GetBrandList(string? keyword, int pageIndex, int pageSize);
    }
}
