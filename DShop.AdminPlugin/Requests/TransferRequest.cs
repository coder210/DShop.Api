using System.ComponentModel.DataAnnotations;

namespace DShop.AdminPlugin.Requests
{
    public class TransferRequest
    {
        /// <summary>
        /// 要移交的委托单ID列表
        /// </summary>
        [Required]
        public List<long> Ids { get; set; } = new();

        /// <summary>
        /// 处理人用户ID
        /// </summary>
        [Required]
        public long HandlerId { get; set; }
    }
}
