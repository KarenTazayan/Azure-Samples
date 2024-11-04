using Microsoft.AspNetCore.Mvc;

namespace WebAppForBot.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(WebLogService webLogService) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        webLogService.AddLog("Hello from the Web API");

        return Ok("Hello from the Web API");
    }
}