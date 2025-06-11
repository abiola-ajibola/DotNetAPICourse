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
[Route("usersJobInfoEF")] // defines what the name of the route is. Does not depend on the name of the controller
// see: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-9.0
// see: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-9.0
public class UserJobInfoControllerEF(IConfiguration config) : ControllerBase // http://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase?view=aspnetcore-9.0
{

    private readonly DataContextEF _context = new(config);

    [HttpGet("")]
    public IEnumerable<UserJobInfo> GetMany()
    {
        return _context.UserJobInfo;
    }

    [HttpGet("{userId}")] // using url parameters e.g http://localhost:3000/GetUsers/sample

    public IActionResult GetOne(int userId)
    {
        UserJobInfo? data = _context.UserJobInfo.Where(d => d.UserId == userId).FirstOrDefault();
        if (data == null)
        {
            return NotFound($"Job info with id:{userId} not found");
        }
        return Ok(data);
    }

    [HttpPut("edit")]
    public IActionResult Edit(UserJobInfo userJobInfo)
    {
        Console.WriteLine(userJobInfo);
        UserJobInfo? _userJobInfo = _context.UserJobInfo.Where(ji => ji.UserId == userJobInfo.UserId).FirstOrDefault();
        if (_userJobInfo == null)
        {
            return NotFound($"Job info with id:{userJobInfo.UserId} not found");
        }
        _userJobInfo.Department = userJobInfo.Department;
        _userJobInfo.JobTitle = userJobInfo.JobTitle;
        _userJobInfo.UserId = userJobInfo.UserId;
        if (_context.SaveChanges() > 0)
        {
            return Ok(userJobInfo);
        }
        return BadRequest(new { message = "User job info not updated" });

    }


    [HttpPost("new")]
    public IActionResult AddNew(UserJobInfo userJobInfo)
    {
        _context.UserJobInfo.Add(userJobInfo);
        if (_context.SaveChanges() > 0)
        {
            return Created();
        }
        else
        {
            return BadRequest("Failed to Add Job Info");
        }
    }

    [HttpDelete("delete/{userId}")]
    public IActionResult Delete(int userId)
    {
        UserJobInfo? jobInfo = _context.UserJobInfo.Where(ji => ji.UserId == userId).FirstOrDefault();
        if (jobInfo == null)
        {
            return NotFound($"Job info with id:{userId} not found");
        }
        _context.Remove(jobInfo);
        if (_context.SaveChanges() > 0)
        {
            return NoContent();
        }
        return BadRequest("Could not delete job info");

    }

}

