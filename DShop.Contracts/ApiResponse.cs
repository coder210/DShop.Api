namespace DShop.Contracts
{
    public class ApiResponse
    {
        public int Code { get; set; }
        public object Data { get; set; }
        public string Msg { get; set; }
    }
}
