using EDaemonWebServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EDaemonWebServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillController : ControllerBase
{
    private readonly ISkillService _service;
            
    public SkillController(ISkillService service)
    {
        _service = service;
    }

    [HttpGet("basic-skills")]
    public async Task<IActionResult> GetAllBasicSkills()
    {
        var skills = await _service.GetAllBasicSkillsAsync();
        return Ok(skills);
    }
    [HttpGet("basic-skills/{id}")]
    public async Task<IActionResult> GetBasicSkill(int id)
    {
        var skill = await _service.GetBasicSkillAsync(id);
        if (skill == null)
        {
            return NotFound();
        }
        return Ok(skill);
    }
}

