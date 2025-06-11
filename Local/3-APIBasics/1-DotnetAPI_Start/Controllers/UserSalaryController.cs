// see https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-9.0
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc?view=aspnetcore-9.0

using DotnetAPI.Dtos;
using DotnetAPI.Models;
using HelloWorld.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
// [Route("[controller]")] // makes the endpoint of the controller to be /Users, from the name of the controller i.e WeatherForecastController.
[Route("usersSalary")] // defines what the name of the route is. Does not depend on the name of the controller
// see: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0
// see: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-9.0
public class UserSalaryController(IConfiguration config) : ControllerBase // http://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase?view=aspnetcore-9.0
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
    public IEnumerable<UserSalary> GetMany()
    {
        string sqlQuery = @"
            SELECT [UserId],
                [Salary],
                [AvgSalary]
            FROM TutorialAppSchema.UserSalary";
        Console.WriteLine(sqlQuery);
        return _context.LoadData<UserSalary>(sqlQuery);
    }

    [HttpGet("{userId}")] // using url parameters e.g http://localhost:3000/GetUsers/sample

    public UserSalary GetOne(int userId)
    {
        string sqlQuery = @"
            SELECT [UserId],
                [Salary],
                [AvgSalary]
            FROM TutorialAppSchema.UserSalary
            WHERE UserId = @UserId";

        Console.WriteLine(sqlQuery);

        return _context.LoadSingle<UserSalary>(sqlQuery, new { UserId = userId });
    }

    [HttpPut("edit")]
    public IActionResult Edit(UserSalary userSalary)
    {
        Console.WriteLine(userSalary);
        // to learn more about parameters, see: https://www.learndapper.com/parameters#dapper-anonymous-parameters
        string sql = @"
        UPDATE TutorialAppSchema.UserSalary
            SET [UserId] = @UserId,
                [Salary] = @Salary
             WHERE UserId = @UserId";

        Console.WriteLine(sql);

        if (_context.ExecuteSql(sql, userSalary))
        {
            return Ok();
        }

        throw new Exception("Failed to Update Job Info");
    }


    [HttpPost("new")]
    public IActionResult AddNew(UserSalary userSalary)
    {
        string sql = @"
        INSERT INTO TutorialAppSchema.UserSalary(
                [UserId],
                [Salary]
            ) VALUES (
                @UserId,
                @Salary
            )";

        Console.WriteLine(sql);
        Console.WriteLine(userSalary);

        if (_context.ExecuteSql(sql, userSalary))
        {
            return Ok();
        }

        throw new Exception("Failed to Add Job Info");
    }

    [HttpDelete("delete/{userId}")]
    public IActionResult Delete(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.UserSalary 
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

