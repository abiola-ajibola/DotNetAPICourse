// see https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-9.0
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc?view=aspnetcore-9.0

using DotnetAPI.Dtos;
using DotnetAPI.Models;
using HelloWorld.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
// [Route("[controller]")] // makes the endpoint of the controller to be /Users, from the name of the controller i.e WeatherForecastController.
[Route("usersJobInfo")] // defines what the name of the route is. Does not depend on the name of the controller
// see: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0
// see: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-9.0
public class UserJobInfoController(IConfiguration config) : ControllerBase // http://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase?view=aspnetcore-9.0
{
    private readonly DataContextDapper _context = new(config);

    /*
    // The constructor can also be in this format 
    public UsersController(IConfiguration config)
    {
        context = new DataContextDapper(config);
    }
     */

    [HttpGet("")]
    public IEnumerable<UserJobInfo> GetMany()
    {
        string sqlQuery = @"
            SELECT [UserId],
                [JobTitle],
                [Department]
            FROM TutorialAppSchema.UserJobInfo";
        Console.WriteLine(sqlQuery);
        return _context.LoadData<UserJobInfo>(sqlQuery);
    }

    [HttpGet("{userId}")] // using url parameters e.g http://localhost:3000/GetUsers/sample

    public UserJobInfo GetOne(int userId)
    {
        string sqlQuery = @"
            SELECT [UserId],
                [JobTitle],
                [Department]
            FROM TutorialAppSchema.UserJobInfo
            WHERE UserId = @UserId";

        Console.WriteLine(sqlQuery);

        return _context.LoadSingle<UserJobInfo>(sqlQuery, new { UserId = userId });
    }

    [HttpPut("edit")]
    public IActionResult Edit(UserJobInfo userJobInfo)
    {
        // to learn more about parameters, see: https://www.learndapper.com/parameters#dapper-anonymous-parameters
        string sql = @"
        UPDATE TutorialAppSchema.UserJobInfo
            SET [UserId] = @UserId,
                [JobTitle] = @JobTitle,
                [Department] = @Department
             WHERE UserId = @UserId";

        Console.WriteLine(sql);
        Console.WriteLine(userJobInfo);

        if (_context.ExecuteSql(sql, userJobInfo))
        {
            return Ok();
        }

        throw new Exception("Failed to Update Job Info");
    }


    [HttpPost("new")]
    public IActionResult AddNew(UserJobInfo userJobInfo)
    {
        string sql = @"INSERT INTO TutorialAppSchema.UserJobInfo(
                [UserId],
                [JobTitle],
                [Department]
            ) VALUES (
                @UserId,
                @JobTitle,
                @Department
            )";

        Console.WriteLine(sql);
        Console.WriteLine(userJobInfo);

        if (_context.ExecuteSql(sql, userJobInfo))
        {
            return Ok();
        }

        throw new Exception("Failed to Add Job Info");
    }

    [HttpDelete("delete/{userId}")]
    public IActionResult Delete(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId = @UserId";

        Console.WriteLine(sql);
        Console.WriteLine(userId);
        try
        {
            if (_context.ExecuteSql(sql, new { UserId = userId }))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete Job Info");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(new { message = e.Message });
        }
    }

}

