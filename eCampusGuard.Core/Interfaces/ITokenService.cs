using System;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}

