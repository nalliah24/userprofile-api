using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userprofile_api.Models;
using userprofile_api.Utils;
using Dapper;

namespace userprofile_api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ISqlConnHelper _sqlConnHelper;

        public UserRepository(ISqlConnHelper sqlConnHelper)
        {
            _sqlConnHelper = sqlConnHelper;
        }

        public async Task<Result<User>> Get(string userId)
        {
            Result<User> result = new Result<User>();
            try
            {
                using (var conn = _sqlConnHelper.Connection)
                {
                    string sql = @"select 
                                        user_id as userid,
                                        first_name as firstname,
                                        last_name as lastname,
                                        manager_id as managerid,
                                        cost_centre as costcentre,
                                        active,
                                        email
                                    from [dbo].[users]
                                    where user_id = @userid
                                    and active = 'Y'";
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("userId", userId, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);

                    conn.Open();
                    var response = await conn.QueryFirstOrDefaultAsync<User>(sql, dp);
                    result.Entity = response;
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Log error
                result.AddError(ex.Message);
                return result;
                // throw new Exception("Error fetching user data: " + ex.Message);
            }
        }

        public async Task<Result> Create(UserDto user)
        {
            try
            {
                using (var conn = _sqlConnHelper.Connection)
                {
                    string insertActive = "Y";
                    DateTime updatedDateTime = DateTime.Now;
                    string sqlInsertUser = @"insert into [dbo].[users] (
                                            user_id,
                                            first_name,
                                            last_name,
                                            cost_centre,
                                            email,
                                            active,
                                            updated_date)
                                    values (@userId,
                                            @firstName,
                                            @lastName,
                                            @costCentre,
                                            @email,
                                            @active,
                                            @updatedDate)";

                    string sqlInsertCredential = @"insert into [dbo].[user_credentials] (user_id, password)
                                    values (@userId, @password)";


                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("userId", user.UserId, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                    dp.Add("firstName", user.FirstName, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                    dp.Add("lastName", user.LastName, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                    // dp.Add("managerId", user.ManagerId, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                    dp.Add("costCentre", user.CostCentre, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                    dp.Add("email", user.Email, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                    dp.Add("active", insertActive, System.Data.DbType.String, System.Data.ParameterDirection.Input, 1);
                    dp.Add("updateddate", updatedDateTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);

                    DynamicParameters dpCredn = new DynamicParameters();
                    dpCredn.Add("userId", user.UserId, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);
                    dpCredn.Add("password", user.Password, System.Data.DbType.String, System.Data.ParameterDirection.Input, 100);

                    conn.Open();
                    var transaction = conn.BeginTransaction();
                    try
                    {
                        var result = await conn.ExecuteAsync(sqlInsertUser, dp, transaction, commandType: System.Data.CommandType.Text);
                        var resultCredn = await conn.ExecuteAsync(sqlInsertCredential, dpCredn, transaction, commandType: System.Data.CommandType.Text);

                        transaction.Commit();
                        return new Result() { IsSuccess = true };
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        List<string> err = new List<string>();
                        if (ex.Message.ToLower().Contains("primary key") || ex.Message.ToLower().Contains("duplicate key"))
                        {
                            err.Add($"Error: User Id {user.UserId} already exists in the database.");
                        } else
                        {
                            err.Add(ex.Message);
                        }
                        return new Result() { IsSuccess = false, Errors = err };
                    }

                }
            }
            catch (Exception ex)
            {
                // Log error
                List<string> err = new List<string>();
                err.Add(ex.Message);
                return new Result() { IsSuccess = false, Errors = err };
                // throw new Exception("Error creating user: " + ex.Message);
            }
        }

        public Task Delete(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> Update(User user)
        {
            throw new NotImplementedException();
        }


    }
}
