using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo) => _repo = repo;

        [HttpPost("register")]
        public async Task<IActionResult> Register(USerForRegisterDTO userForRegisterDTO)
        {

            //before the registration check if the username already exist

              userForRegisterDTO.username=userForRegisterDTO.username.ToLower();

              if(await _repo.UserExistes(userForRegisterDTO.username))
             return BadRequest("This user is already exist!");

            var userToCreate=new User
            {
                username=userForRegisterDTO.username
            };


            var createdUser=await _repo.Register(userToCreate,userForRegisterDTO.password);
            return StatusCode(201);
        }
    }
}