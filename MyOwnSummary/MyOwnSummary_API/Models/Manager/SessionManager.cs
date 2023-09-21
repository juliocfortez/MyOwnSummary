using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Repositories.IRepository;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;

namespace MyOwnSummary_API.Models.Manager
{
    public class SessionManager : ISessionManager
    {
        protected APIResponse _apiResponse;
        private readonly IUserRepository _userRepository;
        public SessionManager(IUserRepository userRepository)
        {
            _apiResponse = new();
            _userRepository = userRepository;
        }

        public async Task<bool> IsAdmin(string userSessionId)
        {
            var u = await _userRepository.GetAllIncluding(x => x.Role).FirstOrDefaultAsync(x => x.Id.ToString() == userSessionId);
            var role = u.Role.Name;
            if (role == "Admin") return true; return false;
        }

        public APIResponse IsAuthenticate(HttpContext context)
        {
            var _keys = context.Session.Keys;
            var _identity = context.User.Identity as ClaimsIdentity;
            if (!_identity.Claims.Any())
            {
                _apiResponse.Errors.Add("Token invalido o expirado");
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }
            var user = _identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value
                + _identity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            if (!_keys.Any(x => x == user))
            {
                _apiResponse.Errors.Add("Inicie sesión");
                _apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                return _apiResponse;
            }
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = _identity.Claims.FirstOrDefault(x => x.Type == "Id").Value;
            return _apiResponse;
        }
    }
}
