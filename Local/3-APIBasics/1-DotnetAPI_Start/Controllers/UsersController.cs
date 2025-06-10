// see https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-9.0
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc?view=aspnetcore-9.0

using DotnetAPI.Dtos;
using DotnetAPI.Models;
using HelloWorld.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
// [Route("[controller]")] // makes the endpoint of the controller to be /Users, from the name of the controller i.e WeatherForecastController.
[Route("users")] // defines what the name of the route is. Does not depend on the name of the controller
// see: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0
// see: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-9.0
public class UsersController(IConfiguration config) : ControllerBase // http://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase?view=aspnetcore-9.0
{
    private readonly DataContextDapper context = new DataContextDapper(config);

    /*
    // The constructor can also be in this format 
    public UsersController(IConfiguration config)
    {
        context = new DataContextDapper(config);
    }
     */

    [HttpGet("test_connection")]
    public string TestConnection()
    {
        return context.LoadSingle<string>("SELECT GETDATE()");
    }

    [HttpGet("getUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sqlQuery = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            FROM TutorialAppSchema.Users";
        IEnumerable<User> users = context.LoadData<User>(sqlQuery);
        return users;
    }

    [HttpGet("getUsers/{userId}")] // using url parameters e.g http://localhost:3000/GetUsers/sample
    // public IActionResult Test()
    public User GetUser(int userId)
    {
        string sqlQuery = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            FROM TutorialAppSchema.Users
            WHERE UserId = " + userId.ToString();

        return context.LoadSingle<User>(sqlQuery);
    }

    [HttpPut("editUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName.Replace("'", "''") +
                "', [LastName] = '" + user.LastName.Replace("'", "''") +
                "', [Email] = '" + user.Email +
                "', [Gender] = '" + user.Gender +
                "', [Active] = '" + user.Active +
            "' WHERE UserId = " + user.UserId;

        Console.WriteLine(sql);

        if (context.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User");
    }


    [HttpPost("addUser")]
    public IActionResult AddUser(AddUserDto user)
    {
        string sql = @"INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES (" +
                "'" + user.FirstName +
                "', '" + user.LastName +
                "', '" + user.Email +
                "', '" + user.Gender +
                "', '" + user.Active +
            "')";

        Console.WriteLine(sql);

        if (context.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");
    }
    
    [HttpDelete("deleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.Users 
                WHERE UserId = " + userId.ToString();
        
        Console.WriteLine(sql);

        if (context.ExecuteSql(sql))
        {
            return Ok();
        } 

        throw new Exception("Failed to Delete User");
    }

}

