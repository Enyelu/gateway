using gateway.api.Commands;
using gateway.api.Models.Login;
using gateway.api.Models.Token;
using gateway.api.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace gateway.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GenericResponse<string>>> Login([FromBody] LoginDto model)
        {
            _logger.LogInformation($"Login Attempt for {model.Email} at {DateTime.Now}");
            var result = await _mediator.Send(new HandleLogin.Command
            {
                Email = model.Email,
                Password = model.Password,
            });
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GenericResponse<string>>> RefreshToken([FromBody] RefreshTokenRequestDto model)
        {
            _logger.LogInformation($"Refresh token Attempt for {model.UserId} at {DateTime.Now}");
            var result = await _mediator.Send(new HandleRefreshToken.Command
            {
                UserId = model.UserId,
                RefreshToken = model.RefreshToken
            });
            return StatusCode(result.StatusCode, result);
        }
    }
}
