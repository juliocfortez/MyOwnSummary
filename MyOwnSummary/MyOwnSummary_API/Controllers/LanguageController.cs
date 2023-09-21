using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Models.Dtos.LanguageDtos;
using MyOwnSummary_API.Models.Dtos.RoleDtos;
using MyOwnSummary_API.Models.Manager;
using MyOwnSummary_API.Repositories.IRepository;
using System.Net;

namespace MyOwnSummary_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;
        private readonly ISessionManager _sessionManager;
        protected APIResponse _apiResponse;
        public LanguageController(ILanguageRepository languageRepository, IMapper mapper, ISessionManager sessionManager)
        {
            _languageRepository = languageRepository;
            _mapper = mapper;
            _apiResponse = new();
            _sessionManager = sessionManager;
        }
        [HttpGet("{id:int}", Name = "GetLanguage")]
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
                var language = await _languageRepository.Get(x => x.Id == id);
                if (language == null)
                {
                    _apiResponse.Errors.Add($"El idioma con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                _apiResponse.Result = _mapper.Map<LanguageDto>(language);
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
                _apiResponse.Result = _mapper.Map<IEnumerable<LanguageDto>>(await _languageRepository.GetAll());
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
                var language = await _languageRepository.Get(x => x.Id == id);
                if (language == null)
                {
                    _apiResponse.Errors.Add($"El usuario con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _languageRepository.Remove(language);
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
        public async Task<ActionResult<APIResponse>> Create([FromBody] CreateLanguageDto createLanguage)
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
                if (await _languageRepository.Get(x => x.Name == createLanguage.Name) != null)
                {
                    _apiResponse.Errors.Add("Este idioma ya existe");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var language = _mapper.Map<Language>(createLanguage);
                await _languageRepository.Create(language);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = _mapper.Map<LanguageDto>(language);
                return CreatedAtRoute("GetLanguage", new { id = language.Id }, _apiResponse);
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
        public async Task<IActionResult> Update([FromBody] LanguageDto languageUpdate, int id)
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
                if (id != languageUpdate.Id)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Errors.Add("El id por parametro no coincide con el id de la entidad a editar");
                    return BadRequest(_apiResponse);
                }
                if (await _languageRepository.Get(x => x.Name == languageUpdate.Name) != null)
                {
                    _apiResponse.Errors.Add("Este idioma ya existe");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var language = await _languageRepository.Get(x => x.Id == id, false);
                if (language == null)
                {
                    _apiResponse.Errors.Add($"El idioma con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                language = _mapper.Map<Language>(languageUpdate);
                await _languageRepository.Update(language);
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

