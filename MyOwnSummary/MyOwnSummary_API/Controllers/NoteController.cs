using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyOwnSummary_API.Models.Dtos.LanguageDtos;
using MyOwnSummary_API.Models.Manager;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Repositories.IRepository;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using MyOwnSummary_API.Models.Dtos.NoteDtos;

namespace MyOwnSummary_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NoteController : Controller
    {
        private readonly INoteRepository _noteRepository;
        private readonly IMapper _mapper;
        private readonly ISessionManager _sessionManager;
        protected APIResponse _apiResponse;
        public NoteController(INoteRepository noteRepository, IMapper mapper, ISessionManager sessionManager)
        {
            _noteRepository = noteRepository;
            _mapper = mapper;
            _apiResponse = new();
            _sessionManager = sessionManager;
        }
        [HttpGet("{id:int}", Name = "GetNote")]
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
                var note = await _noteRepository.Get(x => x.Id == id);
                if (note == null)
                {
                    _apiResponse.Errors.Add($"La note con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                _apiResponse.Result = _mapper.Map<NoteDto>(note);
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
                var t = _sessionManager.IsAuthenticate(HttpContext);
                if (!t.IsSuccess) return Unauthorized(t);
                if (!await _sessionManager.IsAdmin(t.Result.ToString()))
                {
                    _apiResponse.Errors.Add("No tienes permisos para esto");
                    _apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    return Unauthorized(_apiResponse);
                }
                _apiResponse.Result = _mapper.Map<IEnumerable<NoteDto>>(await _noteRepository.GetAll());
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
                var note = await _noteRepository.Get(x => x.Id == id);
                if (note == null)
                {
                    _apiResponse.Errors.Add($"La nota con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _noteRepository.Remove(note);
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
        public async Task<ActionResult<APIResponse>> Create([FromBody] CreateNoteDto createNote)
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
                if (await _noteRepository.Get(x => x.UserId == createNote.UserId && x.Title == createNote.Title && x.LanguageId == createNote.LanguageId) != null)
                {
                    _apiResponse.Errors.Add("Ya tienes una nota con este título");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var note = _mapper.Map<Note>(createNote);
                await _noteRepository.Create(note);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = _mapper.Map<NoteDto>(note);
                return CreatedAtRoute("GetNote", new { id = note.Id }, _apiResponse);
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
        public async Task<IActionResult> Update([FromBody] UpdateNoteDto noteUpdate, int id)
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
                if (id != noteUpdate.Id)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Errors.Add("El id por parametro no coincide con el id de la entidad a editar");
                    return BadRequest(_apiResponse);
                }
                if (await _noteRepository.Get(x => x.UserId == Convert.ToInt32(t.Result) && x.Title == noteUpdate.Title && x.LanguageId == noteUpdate.LanguageId) != null)
                {
                    _apiResponse.Errors.Add("Ya tienes una nota con este título");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var note = await _noteRepository.Get(x => x.Id == id, false);
                if (note == null)
                {
                    _apiResponse.Errors.Add($"La nota con id {id} no existe");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                note = _mapper.Map<Note>(noteUpdate);
                await _noteRepository.Update(note);
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
