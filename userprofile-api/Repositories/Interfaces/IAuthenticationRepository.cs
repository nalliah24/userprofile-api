using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userprofile_api.Utils;

namespace userprofile_api.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<Result<bool>> IsValidUser(string userid, string password);
    }
}
