// see https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-9.0
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc?view=aspnetcore-9.0

using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using HelloWorld.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
// [Route("[controller]")] // makes the endpoint of the controller to be /Users, from the name of the controller i.e WeatherForecastController.
[Route("usersSalaryEF")] // defines what the name of the route is. Does not depend on the name of the controller
// see: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0
// see: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-9.0
public class UserJobSalaryControllerEF(IConfiguration config) : ControllerBase // http://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase?view=aspnetcore-9.0
{

    private readonly DataContextEF _context = new(config);

    [HttpGet("")]
    public IEnumerable<UserSalary> GetMany()
    {
        return _context.UserSalary;
    }

    [HttpGet("{userId}")] // using url parameters e.g http://localhost:3000/GetUsers/sample

    public IActionResult GetOne(int userId)
    {
        UserSalary? data = _context.UserSalary.Where(d => d.UserId == userId).FirstOrDefault();
        if (data == null)
        {
            return NotFound($"Salary with id:{userId} not found");
        }
        return Ok(data);
    }

    [HttpPut("edit")]
    public IActionResult Edit(UserSalary userSalary)
    {
        Console.WriteLine(userSalary);
        UserSalary? _userSalary = _context.UserSalary.Where(ji => ji.UserId == userSalary.UserId).FirstOrDefault();
        if (_userSalary == null)
        {
            return NotFound($"Salary with id:{userSalary.UserId} not found");
        }
        _userSalary.UserId = userSalary.UserId;
        _userSalary.Salary = userSalary.Salary;
        if (_context.SaveChanges() > 0)
        {
            return Ok(userSalary);
        }
        return BadRequest(new { message = "User Salary not updated" });

    }


    [HttpPost("new")]
    public IActionResult AddNew(UserSalary userSalary)
    {
        _context.UserSalary.Add(userSalary);
        if (_context.SaveChanges() > 0)
        {
            return Created();
        }
        else
        {
            return BadRequest("Failed to Add Salary");
        }
    }

    [HttpDelete("delete/{userId}")]
    public IActionResult Delete(int userId)
    {
        UserSalary? jobInfo = _context.UserSalary.Where(ji => ji.UserId == userId).FirstOrDefault();
        if (jobInfo == null)
        {
            return NotFound($"Salary with id:{userId} not found");
        }
        _context.Remove(jobInfo);
        if (_context.SaveChanges() > 0)
        {
            return NoContent();
        }
        return BadRequest("Could not delete Salary");

    }

}

