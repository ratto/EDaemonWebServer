using EDaemonWebServer.Domain.Skills;
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
    public async Task<IActionResult> GetAllBasicSkills([FromForm] BasicSkillsFilter filter)
    {
        var basicSkills = await _service.GetAllBasicSkillsAsync(filter);
        return Ok(basicSkills);
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

