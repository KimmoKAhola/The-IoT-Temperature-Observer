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
public class PlantDataController(PlantContext context, IConfiguration configuration)
    : ControllerBase
{
    [HttpGet("Temperature")]
    public async Task<IActionResult> GetTemperature()
    {
        try
        {
            var result = await context
                .PlantData.Select(t => new PlantDataDTO
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
    public async Task<IActionResult> Put(PlantDataModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Incorrect data.");
        }
        try
        {
            var m = new PlantData { Temperature = model.Temperature, Timestamp = DateTime.UtcNow };

            await context.AddAsync(m);
            await context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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

    [HttpPost("PostMessage")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] MessageDTO userMessage)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        try
        {
            var user = await context.Users.FindAsync(userMessage.UserChatId);

            if (user != null)
            {
                var message = new Message
                {
                    User = user,
                    Content = userMessage.Content,
                    UserChatId = userMessage.UserChatId
                };
                user.Messages.Add(message);
            }
            else
            {
                var message = new Message
                {
                    UserChatId = userMessage.UserChatId,
                    Content = userMessage.Content,
                    User = new User
                    {
                        FirstName = userMessage.FirstName,
                        UserChatId = userMessage.UserChatId
                    }
                };
                await context.AddAsync(message);
            }
            await context.SaveChangesAsync();
            return Ok("Post successful!");
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
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
