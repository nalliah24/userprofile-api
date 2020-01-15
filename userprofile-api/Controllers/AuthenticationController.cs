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
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationController(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        // POST: api/Authentication
        [HttpPost]
        public async Task<ActionResult<Result<bool>>> Post([FromBody] UserCredentials userCredentials)
        {
            // Result<bool> result = new Result<bool>();
            if (userCredentials.UserId == null || userCredentials.UserId == "" || userCredentials.Password == null || userCredentials.Password == "")
            {
                return BadRequest(new Result<bool>() { Error = "Missing required field(s) for user authentication" });
            }

            try
            {
                var result = await _authenticationRepository.IsValidUser(userCredentials.UserId, userCredentials.Password);
                if (result.Entity)
                {
                    return Ok(result);
                }
                result.Error = "Authentication Failed";
                return NotFound(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new Result<bool>() { Error = ex.Message });
            }
        }
    }
}
