using DocumentService.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Controllers
{


    [Route("api/auth")]
    [ApiController]
    public class TempAuthController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GetToken()
        {
            var tokenService = new TokenService("askdjhaskjdasjkldasjkd");

            var defaultPayload = new Payload
            {
                Id = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed"),
                FirstName = "Callum",
                LastName = "Macpherson",
                Email = "callummac@protonmail.com"
            };

            var token = tokenService.CreateToken(defaultPayload);

            return Ok(token);
        }
    }
}
