using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    /// <summary>
    /// 商品命令服务（Product Command）
    /// </summary>
    public interface IProductCommandService
    {
        /// <summary>
        /// 新建/更新商品SPU（含SKU、图片）
        /// </summary>
        (bool Success, string Message) SaveSpu(CreateOrUpdateSpuRequest request);

        /// <summary>
        /// 更新商品SPU状态（上下架）
        /// </summary>
        (bool Success, string Message) UpdateSpuStatus(UpdateSpuStatusRequest request);

        /// <summary>
        /// 删除商品SPU
        /// </summary>
        (bool Success, string Message) DeleteSpu(long id);

        /// <summary>
        /// 新建/更新商品分类
        /// </summary>
        (bool Success, string Message) SaveCategory(CreateOrUpdateCategoryRequest request);

        /// <summary>
        /// 删除商品分类
        /// </summary>
        (bool Success, string Message) DeleteCategory(long id);

        /// <summary>
        /// 新建/更新品牌
        /// </summary>
        (bool Success, string Message) SaveBrand(CreateOrUpdateBrandRequest request);

        /// <summary>
        /// 删除品牌
        /// </summary>
        (bool Success, string Message) DeleteBrand(long id);

        /// <summary>
        /// 新建/更新属性库属性
        /// </summary>
        (bool Success, string Message) SaveAttr(CreateOrUpdateAttrRequest request);

        /// <summary>
        /// 删除属性库属性
        /// </summary>
        (bool Success, string Message) DeleteAttr(long id);
    }
}
