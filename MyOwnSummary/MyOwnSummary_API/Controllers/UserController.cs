using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Models.Dtos.UserDtos;

namespace MyOwnSummary_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;
        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet("{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> Get(int id)
        {
            if (id == 0)
            {
                _logger.LogError("El id por parametro no puede ser 0", id);
                return BadRequest();
            }
            var l = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (l == null)
            {
                _logger.LogError("El id por parámetro no se encuentra en la BD", id);
                return NotFound();
            }

            return Ok(l);
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]

        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            return Ok( await _context.Users.ToListAsync());
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
            {
                _logger.LogError("El id por parametro no puede ser 0", id);
                return BadRequest();
            }
            var l = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (l == null)
            {
                _logger.LogError("El id por parámetro no se encuentra en la BD", id);
                return NotFound();
            }
            _context.Users.Remove(l);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create([FromBody] CreateUserDto user)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }
            if(await _context.Users.AnyAsync(x => x.UserName == user.UserName))
            {
                ModelState.AddModelError("UserNameDuplicated","Este nombre de usuario ya existe");
                return BadRequest(ModelState);
            }

            User u = new()
            {
                UserName = user.UserName,
                Password = user.Password
            };
            await _context.Users.AddAsync(u);
            _context.SaveChanges();
            return CreatedAtRoute("GetUser", new { id = _context.Users.OrderByDescending(x => x.Id).First().Id }, u);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update([FromBody] UserDto user, int id)
        {
            if(id != user.Id) {
                _logger.LogError("El id por parametro no coincide con el id de la entidad a editar", id);
                return BadRequest("El id por parametro no coincide con el id de la entidad a editar");
            }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            if (await _context.Users.AnyAsync(x => x.UserName == user.UserName))
            {
                ModelState.AddModelError("UserNameDuplicated", "Este nombre de usuario ya existe");
                return BadRequest(ModelState);
            }

            var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (u == null)
            {
                _logger.LogError("El id por parámetro no se encuentra en la BD", id);
                return NotFound();
            }
            u.UserName = user.UserName;
            u.Password = user.Password;
            _context.Users.Update(u);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
