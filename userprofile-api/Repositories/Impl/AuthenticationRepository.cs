using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userprofile_api.Models;
using userprofile_api.Utils;

namespace userprofile_api.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly ISqlConnHelper _sqlConnHelper;

        public AuthenticationRepository(ISqlConnHelper sqlConnHelper)
        {
            _sqlConnHelper = sqlConnHelper;
        }

        public async Task<Result<bool>> IsValidUser(string userid, string password)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                using (var conn = _sqlConnHelper.Connection)
                {
                    string sql = @"select u.user_id as UserId, uc.password from [dbo].[users] u inner join [dbo].[user_credentials] uc
                                    on u.user_id = uc.user_id
                                    where u.active = 'Y' 
                                    and u.user_id = @userid 
                                    and uc.password = @password";
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("userid", userid, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                    dp.Add("password", password, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);

                    conn.Open();
                    var response = await conn.QueryAsync<UserCredentials>(sql, dp);
                    if (response.Count() == 1)
                    {
                        result.IsSuccess = true;
                        result.Entity = true;
                        return result;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                return result;
                // throw new Exception("Error: " + ex.Message);
            }
        }


    }
}
