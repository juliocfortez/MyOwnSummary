using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Models.Dtos.UserDtos;
using MyOwnSummary_API.Repositories.IRepository;

namespace MyOwnSummary_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IMapper mapper)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet("{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            if (id == 0)
            {
                _logger.LogError("El id por parametro no puede ser 0", id);
                return BadRequest();
            }
            var l = await _userRepository.Get(x => x.Id == id);
            if (l == null)
            {
                _logger.LogError("El id por parámetro no se encuentra en la BD", id);
                return NotFound();
            }

            return Ok(_mapper.Map<UserDto>(l));
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]

        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<UserDto>>(await _userRepository.GetAll()));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                _logger.LogError("El id por parametro no puede ser 0", id);
                return BadRequest();
            }
            var l = await _userRepository.Get(x => x.Id == id);
            if (l == null)
            {
                _logger.LogError("El id por parámetro no se encuentra en la BD", id);
                return NotFound();
            }
            await _userRepository.Remove(l);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto user)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }
            if(await _userRepository.Get(x => x.UserName == user.UserName) != null)
            {
                ModelState.AddModelError("UserNameDuplicated","Este nombre de usuario ya existe");
                return BadRequest(ModelState);
            }
            var u = _mapper.Map<User>(user);
            await _userRepository.Create(u);
            return CreatedAtRoute("GetUser", new {id = u.Id }, _mapper.Map<UserDto>(u));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UserDto user, int id)
        {
            if(id != user.Id) {
                _logger.LogError("El id por parametro no coincide con el id de la entidad a editar", id);
                return BadRequest("El id por parametro no coincide con el id de la entidad a editar");
            }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            if (await _userRepository.Get(x => x.UserName == user.UserName) != null)
            {
                ModelState.AddModelError("UserNameDuplicated", "Este nombre de usuario ya existe");
                return BadRequest(ModelState);
            }

            var u = await _userRepository.Get(x => x.Id == id, false);
            if (u == null)
            {
                _logger.LogError("El id por parámetro no se encuentra en la BD", id);
                return NotFound();
            }
            u = _mapper.Map<User>(user);
            await _userRepository.Update(u);
            return NoContent();
        }
    }
}
