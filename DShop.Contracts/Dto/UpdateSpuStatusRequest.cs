namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 更新商品SPU状态请求
    /// </summary>
    public class UpdateSpuStatusRequest
    {
        /// <summary>SPU Id</summary>
        public long Id { get; set; }
        /// <summary>状态（PutOnShelves/PutOffShelves）</summary>
        public int Status { get; set; }
    }
}
