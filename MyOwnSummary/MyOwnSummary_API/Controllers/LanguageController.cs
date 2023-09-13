using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Models;

namespace MyOwnSummary_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly DbSet<Language> _languages;
        private readonly ILogger<LanguageController> _logger;
        public LanguageController(ApplicationDbContext context, ILogger<LanguageController> logger)
        {
            _languages = context.Languages;
            _logger = logger;
        }
        [HttpGet("{id:int}", Name = "GetLanguage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Language>> Get(int id)
        {
            if(id == 0) 
            {
                _logger.LogError("El id por parametro no puede ser 0", id);
                return BadRequest(); 
            }
            var l = await _languages.FirstOrDefaultAsync(x => x.Id == id);
            if(l == null) 
            {
                _logger.LogError("El id por parámetro no se encuentra en la BD", id);
                return NotFound();
            }

            return Ok(l);
        }
    }
}
