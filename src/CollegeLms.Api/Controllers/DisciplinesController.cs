using System.Security.Claims;
using CollegeLms.Domain.Entities;
using CollegeLms.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeLms.Api.Controllers;

[ApiController]
[Route("api/v1/disciplines")]
public class DisciplinesController : ControllerBase
{
    private readonly AppDbContext _db;

    public DisciplinesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var disciplines = await _db.Disciplines
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Code,
                d.Description,
                d.Semester,
                Teachers = d.TeacherDisciplines.Select(td => new
                {
                    td.User.Id,
                    td.User.FullName
                }),
                Groups = d.GroupDisciplines.Select(gd => new
                {
                    gd.Group.Id,
                    gd.Group.Name
                })
            })
            .ToListAsync();

        return Ok(disciplines);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var discipline = await _db.Disciplines
            .Where(d => d.Id == id)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Code,
                d.Description,
                d.Semester,
                Teachers = d.TeacherDisciplines.Select(td => new
                {
                    td.User.Id,
                    td.User.FullName
                }),
                Groups = d.GroupDisciplines.Select(gd => new
                {
                    gd.Group.Id,
                    gd.Group.Name
                }),
                Topics = d.Topics.OrderBy(t => t.OrderIndex).Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.OrderIndex,
                    t.PlannedDate,
                    t.Hours,
                    Materials = t.Materials.OrderBy(m => m.OrderIndex).Select(m => new
                    {
                        m.Id,
                        m.Title,
                        Type = m.Type.ToString(),
                        m.FilePath,
                        m.ExternalUrl
                    }),
                    Tests = t.Tests.Where(test => test.IsActive).Select(test => new
                    {
                        test.Id,
                        test.Code,
                        test.Title
                    })
                })
            })
            .FirstOrDefaultAsync();

        if (discipline == null)
            return NotFound();

        return Ok(discipline);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDisciplineRequest request)
    {
        var discipline = new Discipline
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Semester = request.Semester
        };

        _db.Disciplines.Add(discipline);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = discipline.Id }, new { discipline.Id, discipline.Name });
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost("{id:guid}/topics")]
    public async Task<IActionResult> AddTopic(Guid id, [FromBody] CreateTopicRequest request)
    {
        var discipline = await _db.Disciplines.FindAsync(id);
        if (discipline == null)
            return NotFound();

        if (!await IsTeacherOfDiscipline(id))
            return Forbid();

        var topic = new Topic
        {
            Id = Guid.NewGuid(),
            DisciplineId = id,
            Title = request.Title,
            Description = request.Description,
            OrderIndex = request.OrderIndex,
            PlannedDate = request.PlannedDate,
            Hours = request.Hours
        };

        _db.Topics.Add(topic);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id }, new { topic.Id, topic.Title });
    }

    private async Task<bool> IsTeacherOfDiscipline(Guid disciplineId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role == "Admin") return true;
        return await _db.TeacherDisciplines
            .AnyAsync(td => td.UserId == userId && td.DisciplineId == disciplineId);
    }
}

public class CreateDisciplineRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? Semester { get; set; }
}

public class CreateTopicRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public int? Hours { get; set; }
}
