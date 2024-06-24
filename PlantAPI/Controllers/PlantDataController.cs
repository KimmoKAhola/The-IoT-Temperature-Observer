using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlantAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PlantAPI.Controllers;

/// <summary>
/// PlantDataController handles the retrieval and modification of plant data in the plant database.
/// </summary>
[ApiController]
[Route("[controller]")]
public class PlantDataController(PlantContext context, IConfiguration configuration)
    : ControllerBase
{
    /// <summary>
    /// Retrieves the temperature data from the plant database.
    /// </summary>
    /// <param name="date">Optional. The date for which to retrieve the temperature data.</param>
    /// <returns>A collection of PlantViewModel objects containing the temperature data.</returns>
    [HttpGet("Temperature")]
    [SwaggerOperation(Summary = "Test", Description = "Test des")]
    public async Task<IActionResult> GetTemperature(
        [FromQuery, SwaggerParameter(Description = "Test", Required = false)] DateTime? date
    )
    {
        try
        {
            var res = await context.SensorData.ToListAsync();

            if (date.HasValue)
            {
                res = res.Where(x => x.Timestamp.Date == date.Value.Date).ToList();
            }

            var result = res.Select(x => new PlantViewModel
            {
                Temperature = x.Temperature,
                DHT_Temperature = x.DHT_Temperature,
                DHT_Humidity = x.DHT_Humidity,
                Timestamp = x.Timestamp
            });
            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Server not available");
        }
    }

    /// <summary>
    /// Test method description.
    /// </summary>
    /// <param name="id">The identifier parameter.</param>
    /// <returns>The HTTP response result.</returns>
    [HttpGet]
    public async Task<IActionResult> Test(int? id)
    {
        return null;
    }

    /// <summary>
    /// Updates the user information in the plant database.
    /// </summary>
    /// <param name="id">The identifier of the user to be updated.</param>
    /// <param name="patch">The JsonPatchDocument object containing the changes to be applied to the user.</param>
    /// <returns>The HTTP response result. Returns 400 if the patch is null, the user with the given id does not exist, or the applied patch is not valid. Returns 200 if the user is successfully updated.</returns>
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateUser(
        string id,
        [FromBody] JsonPatchDocument<UpdateUserModel> patch
    )
    {
        if (patch != null)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest();
            }

            var model = new UpdateUserModel { IsSubscriber = user.IsSubscriber };
            patch.ApplyTo(model, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            user.IsSubscriber = model.IsSubscriber;

            context.Update(user);
            await context.SaveChangesAsync();
            return Ok();
        }

        return BadRequest();
    }

    [Authorize]
    [HttpPost("post")]
    public async Task<IActionResult> Put(PlantDataDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Incorrect data.");
        }
        try
        {
            var sensor = await context.Sensors.FindAsync(model.Sensor_Id);

            //TODO add null check, for now it is OK.
            var m = new SensorData
            {
                Temperature = Convert.ToDouble(model.Temperature),
                DHT_Temperature = Convert.ToInt32(model.DHT_Temperature),
                DHT_Humidity = Convert.ToInt32(model.DHT_Humidity),
                Sensor = sensor
            };
            m.Timestamp = m.Timestamp.AddHours(2);

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
            var users = res.Select(x => "Name: " + x.FirstName);
            return Ok(users);
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
                var message = new UserBotMessage
                {
                    User = user,
                    Content = userMessage.Content,
                    UserChatId = userMessage.UserChatId
                };
                user.Messages.Add(message);
            }
            else
            {
                var message = new UserBotMessage
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
