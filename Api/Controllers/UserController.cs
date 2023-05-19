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

    [HttpGet]
    [Authorize(Policy= "Admin")]
    public IResult GetUsers()
    {
        var userId = HttpContext.User.Identity?.Name;
        
        var lista = UserRepository.GetAll();
        
        var response = new
        {
            UserLogged = userId,
            Lista = lista
        };
        
        return Results.Ok(response);
    }
    
    [HttpPost("/auth")]
    public IResult AuthUser([FromBody] UserPostDTO userRequest)
    {

        var user = UserRepository.Get(userRequest.Username, userRequest.Pass);
        
        if (user == null)
            return Results.NotFound("Invalid Credentials");
        
        var token = TokenService.GenerateToken(user);
        //user.Pass = "";
        return Results.Ok( new
        {
            user = user,
            token = token
        });
    }
    
    [HttpPost("/createAdmin")]
    public IResult CreateUserAdmin([FromBody] UserPostDTO userRequest)
    {
        var user = UserRepository.CreateAdmin(new User()
        {
            Username = userRequest.Username,
            Pass = userRequest.Pass
        });
        
        // user.Pass = "";
        
        return Results.Created( string.Empty, new
        {
            user = user
        });
    }
}