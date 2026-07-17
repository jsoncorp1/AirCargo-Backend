using AC.Domain.Modules.Users;

namespace AC.Application.Services.Security;

public interface ITokenService
{
    string GenerateToken(User user);
}