using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userprofile_api.Models;
using userprofile_api.Utils;

namespace userprofile_api.Repositories
{
    public interface IUserRepository
    {
        Task<Result<User>> Get(string userId);
        Task<Result> Create(UserDto user);
        Task<User> Update(User user);
        Task Delete(string userId);
        // Task<IEnumerable<User>> GetAll();
    }
}
