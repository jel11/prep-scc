using CollegeLms.Domain.Entities;
using CollegeLms.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeLms.Api.Controllers;

[ApiController]
[Route("api/v1/schedule")]
public class ScheduleController : ControllerBase
{
    private readonly AppDbContext _db;

    public ScheduleController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetSchedule([FromQuery] Guid groupId)
    {
        var entries = await _db.ScheduleEntries
            .Where(se => se.GroupId == groupId)
            .OrderBy(se => se.DayOfWeek)
            .ThenBy(se => se.TimeSlot)
            .Select(se => new
            {
                se.Id,
                se.DayOfWeek,
                se.TimeSlot,
                se.Room,
                WeekType = se.WeekType.ToString(),
                Discipline = se.Discipline.Name,
                DisciplineCode = se.Discipline.Code,
                Teacher = se.Teacher.FullName
            })
            .ToListAsync();

        return Ok(entries);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
    {
        var entry = new ScheduleEntry
        {
            Id = Guid.NewGuid(),
            GroupId = request.GroupId,
            DisciplineId = request.DisciplineId,
            TeacherUserId = request.TeacherUserId,
            DayOfWeek = request.DayOfWeek,
            TimeSlot = request.TimeSlot,
            Room = request.Room,
            WeekType = request.WeekType
        };

        _db.ScheduleEntries.Add(entry);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSchedule), new { groupId = entry.GroupId }, new { entry.Id });
    }
}

public class CreateScheduleRequest
{
    public Guid GroupId { get; set; }
    public Guid DisciplineId { get; set; }
    public Guid TeacherUserId { get; set; }
    public int DayOfWeek { get; set; }
    public int TimeSlot { get; set; }
    public string? Room { get; set; }
    public CollegeLms.Domain.Enums.WeekType WeekType { get; set; }
}
