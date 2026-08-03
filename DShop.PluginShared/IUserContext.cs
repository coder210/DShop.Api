namespace DShop.PluginShared
{
    public interface IUserContext
    {
        public long CurrentUserId { get; }
        public bool IsAuthenticated { get; }
    }
}
