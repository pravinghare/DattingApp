using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {

        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            _config = config;
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            register.username = register.username.ToLower();

            if (await _repository.UserExists(register.username))
            {
                return BadRequest("Username allready registered");
            }

            var CreateUser = new User
            {
                UserName = register.username
            };

            var UserCreated = await _repository.Register(CreateUser, register.password);

            return StatusCode(201);
        }

        [HttpPost("Login")]

        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repository.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userForLoginDto.Username),
            };
            var co=_config.GetSection("AppSettings:Token").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var cred= new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor=new SecurityTokenDescriptor{
                    Subject=new ClaimsIdentity(claims),
                    Expires=DateTime.Now.AddDays(1),
                    SigningCredentials=cred
            };

            var tokenHandler=new JwtSecurityTokenHandler();

            var token=tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new{
                token=tokenHandler.WriteToken(token)
            });



    }

    }
}