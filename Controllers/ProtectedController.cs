using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lek8LarBackend.Controllers
{
    [ApiController]
    [Route("api/protected")]
    public class ProtectedController : ControllerBase
    {
        [Authorize] 
        [HttpGet]
        public IActionResult GetSecretData()
        {
            return Ok(new { Message = "Grattis! Du är inloggad och kan se detta." });
        }
    }
}

