namespace DShop.Contracts.Dto
{
    public class UpdateUserRequest
    {
        /// <summary>
        /// 头像Base64数据
        /// </summary>
        public string AvatarData { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
    }
}
