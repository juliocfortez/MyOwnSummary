using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Models.Dtos.CategoryDtos;
using MyOwnSummary_API.Models.Manager;
using MyOwnSummary_API.Repositories.IRepository;
using System.Net;

namespace MyOwnSummary_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ISessionManager _sessionManager;
        protected APIResponse _apiResponse;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper, ISessionManager sessionManager)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _apiResponse = new();
            _sessionManager = sessionManager;
        }
        [HttpGet("{id:int}", Name = "GetCategory")]
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
                var category = await _categoryRepository.Get(x => x.Id == id);
                if (category == null)
                {
                    _apiResponse.Errors.Add($"El usuario con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                _apiResponse.Result = _mapper.Map<CategoryDto>(category);
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
                if(!await _sessionManager.IsAdmin(t.Result.ToString()))
                {
                    _apiResponse.Errors.Add("No tienes permisos para esto");
                    _apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    return Unauthorized(_apiResponse);
                }
                _apiResponse.Result = _mapper.Map<IEnumerable<CategoryDto>>(await _categoryRepository.GetAll());
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
                var category = await _categoryRepository.Get(x => x.Id == id);
                if (category == null)
                {
                    _apiResponse.Errors.Add($"La categoria con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _categoryRepository.Remove(category);
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
        public async Task<ActionResult<APIResponse>> Create([FromBody] CreateCategoryDto createCategory)
        {
            try
            {
                var t = _sessionManager.IsAuthenticate(this.HttpContext);
                if (!t.IsSuccess) return Unauthorized(t);
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
                var category = await _categoryRepository.Get(x=>x.Name == createCategory.Name);
                if (category != null)
                {
                    _apiResponse.Errors.Add("Esta categoria ya existe");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                category = _mapper.Map<Category>(createCategory);
                await _categoryRepository.Create(category);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = _mapper.Map<CategoryDto>(category);
                return CreatedAtRoute("GetCategory", new { id = category.Id }, _apiResponse);
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
        public async Task<IActionResult> Update([FromBody] CategoryDto categoryUpdate, int id)
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
                if (id != categoryUpdate.Id)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
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
                var category = await _categoryRepository.Get(x => x.Name == categoryUpdate.Name);
                if (category != null)
                {
                    _apiResponse.Errors.Add("Esta categoria ya existe");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                if (await _categoryRepository.Get(x=>x.Id == id, false) == null)
                {
                    _apiResponse.Errors.Add($"La categoria con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                category = _mapper.Map<Category>(categoryUpdate);
                await _categoryRepository.Update(category);
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
