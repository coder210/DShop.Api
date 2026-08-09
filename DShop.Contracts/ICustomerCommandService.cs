using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    /// <summary>
    /// 客户命令服务（Customer Command）
    /// </summary>
    public interface ICustomerCommandService
    {
        /// <summary>
        /// 更新客户状态（启用/禁用）
        /// </summary>
        (bool Success, string Message) UpdateCustomerStatus(UpdateCustomerStatusRequest request);

        /// <summary>
        /// 新建客户
        /// </summary>
        (bool Success, string Message) CreateCustomer(CreateOrUpdateCustomerRequest request);

        /// <summary>
        /// 更新客户信息
        /// </summary>
        (bool Success, string Message) UpdateCustomer(CreateOrUpdateCustomerRequest request);
    }
}
