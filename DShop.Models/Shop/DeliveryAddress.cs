using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 客户收货地址
    /// </summary>
    [Table("DeliveryAddresses")]
    public class DeliveryAddress : ShopEntityBase
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public long CustomerId { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        [MaxLength(50)]
        public string? ContactPerson { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        public string? Mobile { get; set; }
        /// <summary>
        /// 省代号
        /// </summary>
        public int ProvinceCode { get; set; }
        /// <summary>
        /// 市代号
        /// </summary>
        public int CityCode { get; set; }
        /// <summary>
        /// 区代号
        /// </summary>
        public int DistrictCode { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [MaxLength(200)]
        public string? DetailedAddress { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(200)]
        public string? Address { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
