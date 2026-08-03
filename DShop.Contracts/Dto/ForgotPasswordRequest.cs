namespace DShop.Contracts.Dto
{
    public class ForgotPasswordRequest
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string Captcha { get; set; }
    }
}
