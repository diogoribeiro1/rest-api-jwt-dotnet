using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize(Policy= "Admin")]
    public IActionResult GetUsers()
    {
        var userId = HttpContext.User.Identity?.Name;
        
        var lista = _userRepository.GetAll();
        
        var response = new
        {
            UserLogged = userId,
            Lista = lista.Result
        };
        
        return Ok(response);
    }
    
    [HttpPost("/auth")]
    public IActionResult AuthUser([FromBody] UserPostDTO userRequest)
    {

        var user = _userRepository.GetByUsermaeAndPassword(userRequest.Username, userRequest.Pass);
       
        if (user.Result.First() == null)
            return NotFound("Invalid Credentials");
        
        var token = TokenService.GenerateToken(user.Result.First());
        user.Result.First().Password = "";
        
        return Ok( new
        {
            user = user.Result.First(),
            token = token
        });
    }
    
    [HttpPost("/createAdmin")]
    public IActionResult CreateUserAdmin([FromBody] UserPostDTO userRequest)
    {
        var user = _userRepository.Create(new User()
        {
            Username = userRequest.Username,
            Password = userRequest.Pass,
            Role = "manager"
        });
        
        user.Result.Password = "";
        
        return Created( string.Empty, new
        {
            user = user.Result
        });
    }
}