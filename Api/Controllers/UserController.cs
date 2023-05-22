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
    
    private readonly IUserServices _userServices;

    public UserController(IUserServices userServices)
    {
        _userServices = userServices;
    }

    [HttpGet]
    [Authorize(Policy= "Admin")]
    public IActionResult GetUsers()
    {
        var userId = HttpContext.User.Identity?.Name;
        
        var lista = _userServices.GetAll();
        
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

        var user = _userServices.GetByUsernameAndPassword(userRequest.Username, userRequest.Pass);
       
        if (user == null)
            return NotFound("Invalid Credentials");
        
        var token = TokenService.GenerateToken(user);
        user.Password = "";
        
        return Ok( new
        {
            user = user,
            token = token
        });
    }
    
    [HttpPost("/createAdmin")]
    public IActionResult CreateUserAdmin([FromBody] UserPostDTO userRequest)
    {
        var user = _userServices.Create(new User()
        {
            Username = userRequest.Username,
            Password = userRequest.Pass,
            Role = "manager"
        });
        
        if (user == null)
        {
            return BadRequest("Username Already exists");
        }
        
        user.Password = "";
        
        return Created( string.Empty, new
        {
            user = user
        });
    }
    
    [HttpPost("/createEmployee")]
    public IActionResult CreateUserEmployee([FromBody] UserPostDTO userRequest)
    {
        var user = _userServices.Create(new User()
        {
            Username = userRequest.Username,
            Password = userRequest.Pass,
            Role = "employee"
        });
        
        if (user == null)
        {
            return BadRequest("Username Already exists");
        }
        
        user.Password = "";
        
        return Created( string.Empty, new
        {
            user = user
        });
    }
}