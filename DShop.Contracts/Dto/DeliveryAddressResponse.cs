using System;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 客户收货地址
    /// </summary>
    public class DeliveryAddressResponse
    {
        public long Id { get; set; }
        /// <summary>联系人</summary>
        public string? ContactPerson { get; set; }
        /// <summary>手机号</summary>
        public string? Mobile { get; set; }
        /// <summary>省代号</summary>
        public int ProvinceCode { get; set; }
        /// <summary>市代号</summary>
        public int CityCode { get; set; }
        /// <summary>区代号</summary>
        public int DistrictCode { get; set; }
        /// <summary>详细地址</summary>
        public string? DetailedAddress { get; set; }
        /// <summary>是否默认</summary>
        public bool IsDefault { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }
}
