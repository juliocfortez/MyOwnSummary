using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Models.Dtos.UserDtos;
using MyOwnSummary_API.Repositories.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MyOwnSummary_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly string secretKey;
        private readonly IUserRepository _userRepository;
        private readonly APIResponse _apiResponse;
        public AuthenticationController(IConfiguration config, IUserRepository userRepository) {
            secretKey = config.GetSection("Jwt").GetValue<string>("Key");
            _userRepository = userRepository;
            _apiResponse = new();
        }

        [HttpPost(Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        public async Task<ActionResult<APIResponse>> LogIn([FromBody]CreateUserDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var item in ModelState.Values)
                    {
                        foreach (var error in item.Errors)
                        {
                            _apiResponse.Errors.Add(error.ErrorMessage);
                        }
                    }
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var userDb = await _userRepository.Get(x => x.Password == user.Password && x.UserName == user.UserName);
                if (userDb != null)
                {
                    var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                    var claims = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.UserName), new Claim("Id",userDb.Id.ToString()) }, CookieAuthenticationDefaults.AuthenticationScheme); ;
                    var token = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddMinutes(60),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenConfig = tokenHandler.CreateToken(token);
                    var prnicipal = new ClaimsPrincipal(claims);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prnicipal);
                    HttpContext.Session.SetString(userDb.UserName+userDb.Id, user.UserName);
                    _apiResponse.Result = tokenHandler.WriteToken(tokenConfig);
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    return Ok(_apiResponse);
                }
                else
                {
                    _apiResponse.Errors.Add("Usuario no encontrado");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return BadRequest(_apiResponse);
                }
            }
            catch (Exception ex)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.IsSuccess = false;
            }
            return BadRequest(_apiResponse);
            
        }
        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!identity.Claims.Any()) return BadRequest("Token invalido o expirado");
            var key = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value + identity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if(HttpContext.Session.Keys.Any(x => x == key)) HttpContext.Session.Remove(key);
            return Ok("Session closed");
        }
    }
}
