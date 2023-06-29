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
        
        var lista = await _userServices.GetAllAsync();
        
        var response = new
        {
            UserLogged = userId,
            Lista = lista
        };
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy= "Admin")]
    public async Task<OkObjectResult> GetUserById([FromRoute] int id)
    {
        var userId = HttpContext.User.Identity?.Name;

        var userResponse = await _userServices.GetByIdAsync(id);

        userResponse.Password = "";
        
        var response = new
        {
            UserLogged = userId,
            user = userResponse
        };
        return Ok(response);
    }
    
    [HttpPost("/auth")]
    public async Task<IActionResult> AuthUser([FromBody] CreateUserDTO createUserRequest)
    {

        var user = await _userServices.GetByUsernameAndPasswordAsync(createUserRequest.Username, createUserRequest.Pass);

        var token = TokenService.GenerateToken(user);
        user.Password = "";
        
        return Ok( new
        {
            user = user,
            token = token
        });
    }
    
    [HttpPost("/createAdmin")]
    public async Task<IActionResult> CreateUserAdmin([FromBody] CreateUserDTO createUserRequest)
    {
        var user = await _userServices.CreateAsync(new User()
        {
            Username = createUserRequest.Username,
            Password = createUserRequest.Pass,
            Role = "manager"
        });

        user.Password = "";
        
        return Created( string.Empty, new
        {
            user = user
        });
    }
    
    [HttpPost("/createEmployee")]
    public async Task<IActionResult> CreateUserEmployee([FromBody] CreateUserDTO createUserRequest)
    {
        var user = await _userServices.CreateAsync(new User()
        {
            Username = createUserRequest.Username,
            Password = createUserRequest.Pass,
            Role = "employee"
        });

        user.Password = "";
        
        return Created( string.Empty, new
        {
            user = user
        });
    }
    
    [HttpPut("{id}")]
    public async Task<OkObjectResult> UpdateUser([FromBody] UpdateUserDTO updateUserDto, [FromRoute] int id)
    {
        var user = await _userServices.UpdateAsync(new User()
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
        await _userServices.DeleteAsync(id);
        return NoContent();
    }
}