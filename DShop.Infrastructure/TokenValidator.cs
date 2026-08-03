using DShop.Models;

namespace DShop.Infrastructure;

public class TokenValidator : ITokenValidator
{
    private readonly DatabaseContext _context;

    public TokenValidator(DatabaseContext context)
    {
        _context = context;
    }

    public bool ValidateToken(long id, out RefreshToken? tokenInfo)
    {
        var now = DateTime.Now;
        tokenInfo = _context.RefreshTokens
            .Where(it => it.UserId == id && it.ExpiresAt > now && (it.RevokedAt == null || it.RevokedAt > now))
            .OrderByDescending(it => it.CreatedAt)
            .FirstOrDefault();
        return tokenInfo != null;
    }
}
