using System.Threading.Tasks;
using authservice.Boundary.Request;
using authservice.Encryption;
using authservice.Factories;
using authservice.Infrastructure.Exceptions;
using authservice.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenService;

namespace authservice.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IDeleteUseCase _deleteUseCase;
        private readonly IPasswordHasher _hashService;

        private readonly ILoginUseCase _loginUseCase;
        private readonly IRegisterUseCase _registerUseCase;
        private readonly ITokenService _tokenService;

        public AuthController(
            ILoginUseCase loginUseCase,
            IRegisterUseCase registerUseCase,
            IDeleteUseCase deleteUseCase,
            ITokenService tokenService,
            IPasswordHasher hashService)
        {
            _loginUseCase = loginUseCase;
            _registerUseCase = registerUseCase;
            _deleteUseCase = deleteUseCase;

            _tokenService = tokenService;
            _hashService = hashService;
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestObject requestObject)
        {
            var user = await _loginUseCase.Execute(requestObject.Email);
            if (user == null) return NotFound(requestObject.Email);

            var verified = _hashService.Check(user.Hash, requestObject.Password);
            if (verified == false) return Unauthorized();

            var payload = user.ToPayload();
            var token = _tokenService.CreateToken(payload);
            Response.Headers.Add(Constants.AuthToken, token);

            return Ok();
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestObject requestObject)
        {
            try
            {
                var hash = _hashService.Hash(requestObject.Password);

                var response = await _registerUseCase.Execute(requestObject, hash);

                return Ok();
            }
            catch (UserWithEmailAlreadyExistsException)
            {
                return Conflict(requestObject.Email);
            }
        }

        [HttpDelete]
        [Route("delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAccount([FromHeader] string token)
        {
            var payload = _tokenService.ValidateToken(token);
            if (payload == null) return Unauthorized();

            try
            {
                await _deleteUseCase.Execute(payload.Email);

                return NoContent();
            }
            catch (UserNotFoundException)
            {
                return NotFound(payload.Email);
            }
        }

        [HttpGet]
        [Route("check")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckAuth([FromHeader] string token)
        {
            var payload = _tokenService.ValidateToken(token);
            if (payload == null) return Unauthorized();

            return Ok();
        }
    }
}