using CollegeLms.Domain.Entities;
using CollegeLms.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeLms.Api.Controllers;

[ApiController]
[Route("api/v1/groups")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _db;

    public GroupsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var groups = await _db.Groups
            .Where(g => g.IsActive)
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.EnrollmentYear,
                StudentCount = g.StudentGroups.Count(sg => sg.RemovedAt == null)
            })
            .ToListAsync();

        return Ok(groups);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var group = await _db.Groups
            .Where(g => g.Id == id)
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.EnrollmentYear,
                g.IsActive,
                Students = g.StudentGroups
                    .Where(sg => sg.RemovedAt == null)
                    .Select(sg => new
                    {
                        sg.User.Id,
                        sg.User.FullName,
                        sg.User.Email,
                        sg.AssignedAt
                    }),
                Disciplines = g.GroupDisciplines.Select(gd => new
                {
                    gd.Discipline.Id,
                    gd.Discipline.Name,
                    gd.Discipline.Code
                })
            })
            .FirstOrDefaultAsync();

        if (group == null)
            return NotFound();

        return Ok(group);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
    {
        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            EnrollmentYear = request.EnrollmentYear
        };

        _db.Groups.Add(group);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = group.Id }, new { group.Id, group.Name });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/students")]
    public async Task<IActionResult> AddStudent(Guid id, [FromBody] AddStudentRequest request)
    {
        var group = await _db.Groups.FindAsync(id);
        if (group == null)
            return NotFound();

        var user = await _db.Users.FindAsync(request.UserId);
        if (user == null)
            return NotFound(new { message = "Студент не найден" });

        var existing = await _db.StudentGroups
            .AnyAsync(sg => sg.UserId == request.UserId && sg.GroupId == id && sg.RemovedAt == null);
        if (existing)
            return Conflict(new { message = "Студент уже в этой группе" });

        _db.StudentGroups.Add(new StudentGroup
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            GroupId = id,
            AssignedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        return Ok(new { message = "Студент добавлен в группу" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{groupId:guid}/disciplines")]
    public async Task<IActionResult> AddDiscipline(Guid groupId, [FromBody] AddDisciplineToGroupRequest request)
    {
        var exists = await _db.GroupDisciplines
            .AnyAsync(gd => gd.GroupId == groupId && gd.DisciplineId == request.DisciplineId);
        if (exists)
            return Conflict(new { message = "Дисциплина уже привязана к группе" });

        _db.GroupDisciplines.Add(new GroupDiscipline
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            DisciplineId = request.DisciplineId
        });
        await _db.SaveChangesAsync();

        return Ok(new { message = "Дисциплина добавлена к группе" });
    }
}

public class CreateGroupRequest
{
    public string Name { get; set; } = string.Empty;
    public int EnrollmentYear { get; set; }
}

public class AddStudentRequest
{
    public Guid UserId { get; set; }
}

public class AddDisciplineToGroupRequest
{
    public Guid DisciplineId { get; set; }
}
