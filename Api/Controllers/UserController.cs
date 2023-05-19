using Api.Model;
using Api.Repository;
using Api.Services;
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
        var lista = UserRepository.GetAll();
        return Results.Ok(lista);
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
        var user = UserRepository.CreateAdmin(new User
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