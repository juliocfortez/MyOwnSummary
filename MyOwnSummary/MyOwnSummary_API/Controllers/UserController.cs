using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Models.Dtos.UserDtos;
using MyOwnSummary_API.Models.Manager;
using MyOwnSummary_API.Repositories.IRepository;
using System.Net;

namespace MyOwnSummary_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly ISessionManager _sessionManager;
        protected APIResponse _apiResponse;
        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IMapper mapper, ISessionManager sessionManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
            _apiResponse = new();
            _sessionManager = sessionManager;
        }
        [HttpGet("{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        public async Task<ActionResult<APIResponse>> Get(int id)
        {
            try
            {
                var t = _sessionManager.IsAuthenticate(this.HttpContext);
                if (!t.IsSuccess) return Unauthorized(t);
                if (!await _sessionManager.IsAdmin(t.Result.ToString())) 
                {
                    _apiResponse.Errors.Add("No tienes permisos para esto");
                    _apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    return Unauthorized(_apiResponse);
                }
                if (id == 0)
                {
                    _logger.LogError("El id por parametro no puede ser 0", id);
                    _apiResponse.Errors.Add("El id no puede ser 0");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var user =await _userRepository.Get(x=> x.Id == id);
                if (user == null)
                {
                    _logger.LogError($"El usuario con id {id} no existe", id);
                    _apiResponse.Errors.Add($"El usuario con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                _apiResponse.Result = _mapper.Map<UserDto>(user);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.IsSuccess = false;
            }
            return _apiResponse;   
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]

        public async Task<ActionResult<APIResponse>> GetAll()
        {
            try
            {
                var t = _sessionManager.IsAuthenticate(this.HttpContext);
                if (!t.IsSuccess) return Unauthorized(t);
                if (!await _sessionManager.IsAdmin(t.Result.ToString()))
                {
                    _apiResponse.Errors.Add("No tienes permisos para esto");
                    _apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    return Unauthorized(_apiResponse);
                }
                _apiResponse.Result = _mapper.Map<IEnumerable<UserDto>>(await _userRepository.GetAll());
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.IsSuccess = false;
            }

            return _apiResponse;
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var t = _sessionManager.IsAuthenticate(this.HttpContext);
                if (!t.IsSuccess) return Unauthorized(t);
                var u =await _userRepository.GetAllIncluding(x=>x.Role).FirstOrDefaultAsync(x=>x.Id.ToString()==t.Result.ToString());
                var role = u.Role.Name;
                if(role != "Admin")
                {
                    _apiResponse.Errors.Add("No tienes permisos para eliminar");
                    _apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    return Unauthorized(_apiResponse);
                }
                if (id == 0)
                {
                    _logger.LogError("El id por parametro no puede ser 0", id);
                    _apiResponse.Errors.Add("El id no puede ser 0");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var user = await _userRepository.Get(x => x.Id == id);
                if (user == null)
                {
                    _logger.LogError($"El usuario con id {id} no existe", id);
                    _apiResponse.Errors.Add($"El usuario con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _userRepository.Remove(user);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch(Exception ex) 
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.IsSuccess = false;
            }
                return BadRequest(_apiResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        public async Task<ActionResult<APIResponse>> Create([FromBody] CreateUserDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach(var item in ModelState.Values)
                    {
                        foreach (var error in item.Errors)
                        {
                            _apiResponse.Errors.Add(error.ErrorMessage);
                        }
                    }
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                if (await _userRepository.Get(x => x.UserName == user.UserName) != null)
                {
                    _apiResponse.Errors.Add("Este nombre de usuario ya existe");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var u = _mapper.Map<User>(user);
                await _userRepository.Create(u);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = _mapper.Map<UserDto>(u);
                return CreatedAtRoute("GetUser", new { id = u.Id }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.IsSuccess = false;
            }
            return _apiResponse;
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResponse))]
        public async Task<IActionResult> Update([FromBody] UserDto user, int id) 
        {
            try
            {
                var t = _sessionManager.IsAuthenticate(this.HttpContext);
                if (!t.IsSuccess) return Unauthorized(t);
                var u = await _userRepository.GetAllIncluding(x => x.Role).FirstOrDefaultAsync(x => x.Id.ToString() == t.Result.ToString());
                var role = u.Role.Name;
                if (role != "Admin")
                {
                    _apiResponse.Errors.Add("No tienes permisos para editar");
                    _apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    return Unauthorized(_apiResponse);
                }
                if (id != user.Id)
                {
                    _logger.LogError("El id por parametro no coincide con el id de la entidad a editar", id);
                    _apiResponse.StatusCode=HttpStatusCode.BadRequest;
                    _apiResponse.Errors.Add("El id por parametro no coincide con el id de la entidad a editar");
                    return BadRequest(_apiResponse);
                }
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
                var u = await _userRepository.Get(x => x.Id == id, false);
                var uRepetido = await _userRepository.Get(x => x.UserName == user.UserName && x.Id != id);
                if (uRepetido != null)
                {
                    _apiResponse.Errors.Add("Este nombre de usuario ya existe");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                if (u == null)
                {
                    _apiResponse.Errors.Add($"El usuario con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                u = _mapper.Map<User>(user);
                await _userRepository.Update(u);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                _apiResponse.Result = user;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.IsSuccess = false;
            }
            return BadRequest(_apiResponse);
        }
    }
}
