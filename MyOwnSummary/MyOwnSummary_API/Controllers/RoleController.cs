using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyOwnSummary_API.Models.Dtos.UserDtos;
using MyOwnSummary_API.Models.Manager;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Repositories.IRepository;
using System.Net;
using MyOwnSummary_API.Models.Dtos.RoleDtos;

namespace MyOwnSummary_API.Controllers
{
    
        [Route("api/[controller]")]
        [ApiController]
        [Authorize]
        public class RoleController : ControllerBase
        {
            private readonly IRoleRepository _roleRepository;
            private readonly IMapper _mapper;
            private readonly ISessionManager _sessionManager;
            protected APIResponse _apiResponse;
            public RoleController(IRoleRepository roleRepository, IMapper mapper, ISessionManager sessionManager)
            {
                _roleRepository = roleRepository;
                _mapper = mapper;
                _apiResponse = new();
                _sessionManager = sessionManager;
            }
            [HttpGet("{id:int}", Name = "GetRole")]
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
                        _apiResponse.Errors.Add("El id no puede ser 0");
                        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_apiResponse);
                    }
                    var role = await _roleRepository.Get(x => x.Id == id);
                    if (role == null)
                    {
                        _apiResponse.Errors.Add($"El rol con id {id} no existe");
                        _apiResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_apiResponse);
                    }
                    _apiResponse.Result = _mapper.Map<RoleDto>(role);
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
                _apiResponse.Result = _mapper.Map<IEnumerable<RoleDto>>(await _roleRepository.GetAll());
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
                if (!await _sessionManager.IsAdmin(t.Result.ToString()))
                {
                    _apiResponse.Errors.Add("No tienes permisos para esto");
                    _apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    return Unauthorized(_apiResponse);
                }
                if (id == 0)
                    {
                        _apiResponse.Errors.Add("El id no puede ser 0");
                        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_apiResponse);
                    }
                    var role = await _roleRepository.Get(x => x.Id == id);
                    if (role == null)
                    {
                        _apiResponse.Errors.Add($"El usuario con id {id} no existe");
                        _apiResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_apiResponse);
                    }
                    await _roleRepository.Remove(role);
                    _apiResponse.StatusCode = HttpStatusCode.NoContent;
                    _apiResponse.IsSuccess = true;
                    return Ok(_apiResponse);
                }
                catch (Exception ex)
                {
                    _apiResponse.Errors.Add(ex.Message);
                    _apiResponse.IsSuccess = false;
                }
                return BadRequest(_apiResponse);
            }

            [HttpPost]
            [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(APIResponse))]
            [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResponse))]
            public async Task<ActionResult<APIResponse>> Create([FromBody] CreateRoleDto createRole)
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
                    if (await _roleRepository.Get(x => x.Name == createRole.Name) != null)
                    {
                        _apiResponse.Errors.Add("Este role ya existe");
                        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_apiResponse);
                    }
                    var role = _mapper.Map<Role>(createRole);
                    await _roleRepository.Create(role);
                    _apiResponse.IsSuccess = true;
                    _apiResponse.StatusCode = HttpStatusCode.Created;
                    _apiResponse.Result = _mapper.Map<RoleDto>(role);
                    return CreatedAtRoute("GetRole", new { id = role.Id }, _apiResponse);
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
            public async Task<IActionResult> Update([FromBody] RoleDto roleUpdate, int id)
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
                if (id != roleUpdate.Id)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Errors.Add("El id por parametro no coincide con el id de la entidad a editar");
                    return BadRequest(_apiResponse);
                }
                if (await _roleRepository.Get(x => x.Name == roleUpdate.Name) != null)
                    {
                        _apiResponse.Errors.Add("Este role ya existe");
                        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_apiResponse);
                    }

                    var role = await _roleRepository.Get(x => x.Id == id, false);
                    if (role == null)
                    {
                        _apiResponse.Errors.Add($"El role con id {id} no existe");
                        _apiResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_apiResponse);
                    }
                    role = _mapper.Map<Role>(roleUpdate);
                    await _roleRepository.Update(role);
                    _apiResponse.StatusCode = HttpStatusCode.NoContent;
                    _apiResponse.IsSuccess = true;
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
