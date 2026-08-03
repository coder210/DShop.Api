using DShop.Models;

namespace DShop.Infrastructure;

public interface ITokenValidator
{
    bool ValidateToken(long id, out RefreshToken? tokenInfo);
}
