using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlantAPI.Models;

namespace PlantAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController(PlantContext context, IConfiguration configuration) : ControllerBase
{
    [HttpGet("Temperature")]
    public async Task<IActionResult> GetTemperature()
    {
        try
        {
            var result = await context
                .Test.Select(t => new TestDTO
                {
                    Temperature = t.Temperature.ToString(new CultureInfo("sv-SE")) + " \u00b0C",
                    Timestamp = t.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Server not available");
        }
    }

    [Authorize]
    [HttpPost("post")]
    public async Task<IActionResult> Put(TestModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Incorrect data.");
        }
        try
        {
            var m = new Test { Temperature = model.Temperature, Timestamp = DateTime.UtcNow };

            await context.AddAsync(m);
            await context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // return StatusCode(500, "Error");
        }

        return StatusCode(401, "Unauthorized");
    }

    [HttpGet("User")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var res = await context.Users.ToListAsync();
            return Ok(res);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return BadRequest();
    }
    
    [HttpPost("User")]
    public async Task<IActionResult> Put(User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        try
        {
            var database = await context.Users.ToListAsync();
            if (database.Any(u => u.UserChatId == user.UserChatId))
            {
                var u = database.Find(u => u.UserChatId == user.UserChatId);
                u.NumberOfMessagesSent += 1;
                await context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                var newUser = new User
                {
                    FirstName = user.FirstName,
                    UserChatId = user.UserChatId,
                    NumberOfMessagesSent = 0
                };
                await context.AddAsync(newUser);
                await context.SaveChangesAsync();
                return Ok();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return StatusCode(401, "Unauthorized");
    }

    [HttpPost("token")]
    public string Token(string user, string password)
    {
        var credentials = configuration.GetSection("UserCredentials");
        if (user == credentials["Username"] && password == credentials["Password"])
        {
            return GenerateJwtToken();
        }

        return "";
    }

    private string GenerateJwtToken()
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!)
        );
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
