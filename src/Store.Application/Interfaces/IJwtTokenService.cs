using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface IJwtTokenService
{
    string CreateToken(ApplicationUser user);
}
