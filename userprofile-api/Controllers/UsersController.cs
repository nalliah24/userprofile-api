using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using userprofile_api.Models;
using userprofile_api.Repositories;
using userprofile_api.Utils;

namespace userprofile_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/Users/5
        [HttpGet("{userId}", Name = "Get")]
        public async Task<ActionResult<Result>> Get(string userId)
        {
            if (userId == null || userId == "")
            {
                return BadRequest($"Must provide a user id");
            }

            try
            {
                Result<User> userResult = await _userRepository.Get(userId);
                if (userResult.Entity == null)
                {
                    userResult.AddError($"User not found for the provided id {userId}");
                    return NotFound(userResult);
                }

                userResult.IsSuccess = true;
                return Ok(userResult);
            }
            catch (Exception ex)
            {
                Result result = new Result() { IsSuccess = false };
                result.AddError(ex.Message);
                return result;
                // throw new Exception("Error getting user by id. " + ex.Message);
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<Result>> Post([FromBody] UserDto user)
        {
            Result result = Validate(user);
            if (!result.IsSuccess) {
                return BadRequest(result);
            }

            try
            {
                Result userResult = await _userRepository.Create(user);
                if (userResult.IsSuccess == true) {
                    return Created("", userResult);
                } else
                {
                    return BadRequest(userResult);
                }
            }
            catch (Exception ex)
            {
                Result resultErr = new Result();
                resultErr.AddError($"Error creating user. {ex.Message}");
                return resultErr;
                // throw new Exception("Error creating user. " + ex.Message);
            }
        }

        // PUT: api/Users/5
        [HttpPut("{userId}")]
        public void Put(int userId, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        private Result Validate(UserDto user)
        {
            Result result = new Result();
            if (user.UserId == null || user.UserId == "")
            {
                result.Errors.Add("Must provide a user id");
            }
            if (user.FirstName == null || user.FirstName == "")
            {
                result.Errors.Add("Must provide first name");
            }
            if (user.LastName == null || user.LastName == "")
            {
                result.Errors.Add("Must provide last name");
            }
            if (user.CostCentre == null || user.CostCentre == "")
            {
                result.Errors.Add("Must provide costcentre");
            }
            if (user.Password == null || user.Password == "")
            {
                result.Errors.Add("Must provide password");
            }

            if (result.Errors.Count == 0)
            {
                result.IsSuccess = true;
            }
            else {
                result.IsSuccess = false;
            }
            return result;
        }
    }
}
