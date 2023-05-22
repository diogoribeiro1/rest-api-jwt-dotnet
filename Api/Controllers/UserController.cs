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
    public async Task<OkObjectResult> GetUsers()
    {
        var userId = HttpContext.User.Identity?.Name;
        
        var lista = await _userServices.GetAll();
        
        var response = new
        {
            UserLogged = userId,
            Lista = lista
        };
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy= "Admin")]
    public async Task<OkObjectResult> GetAnUser([FromRoute] int id)
    {
        var userId = HttpContext.User.Identity?.Name;

        var userResponse = await _userServices.GetById(id);
        
        var response = new
        {
            UserLogged = userId,
            user = userResponse
        };
        return Ok(response);
    }
    
    [HttpPost("/auth")]
    public IActionResult AuthUser([FromBody] CreateUserDTO createUserRequest)
    {

        var user = _userServices.GetByUsernameAndPassword(createUserRequest.Username, createUserRequest.Pass);
       
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
    public IActionResult CreateUserAdmin([FromBody] CreateUserDTO createUserRequest)
    {
        var user = _userServices.Create(new User()
        {
            Username = createUserRequest.Username,
            Password = createUserRequest.Pass,
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
    public IActionResult CreateUserEmployee([FromBody] CreateUserDTO createUserRequest)
    {
        var user = _userServices.Create(new User()
        {
            Username = createUserRequest.Username,
            Password = createUserRequest.Pass,
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
    
    [HttpPut("{id}")]
    public async Task<OkObjectResult> UpdateUser([FromBody] UpdateUserDTO updateUserDto, [FromRoute] int id)
    {
        var user = await _userServices.Update(new User()
        {
            Id = id,
            Username = updateUserDto.Username
        });
        
        return Ok( new
        {
            user = user
        });
    }
    
    [HttpDelete("{id}")]
    public async Task<NoContentResult> DeleteUser([FromRoute] int id)
    {
        await _userServices.Delete(id);
        return NoContent();
    }
}