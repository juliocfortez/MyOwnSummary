using Microsoft.AspNetCore.Mvc;
using MyOwnSummary_API.Models;

namespace MyOwnSummary_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger)
        {

            _logger = logger;

        }
    }
}
