using System.Security.Claims;
using CollegeLms.Domain.Entities;
using CollegeLms.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeLms.Api.Controllers;

[ApiController]
[Route("api/v1/journal")]
[Authorize]
public class JournalController : ControllerBase
{
    private readonly AppDbContext _db;

    public JournalController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetJournal([FromQuery] Guid groupId, [FromQuery] Guid disciplineId)
    {
        var students = await _db.StudentGroups
            .Where(sg => sg.GroupId == groupId && sg.RemovedAt == null)
            .Select(sg => sg.User)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        var entries = await _db.JournalEntries
            .Where(e => e.GroupId == groupId && e.DisciplineId == disciplineId)
            .ToListAsync();

        var dates = entries.Select(e => e.Date).Distinct().OrderBy(d => d).ToList();

        var result = students.Select(s => new
        {
            StudentId = s.Id,
            s.FullName,
            Grades = dates.Select(d => new
            {
                Date = d,
                Grade = entries.FirstOrDefault(e => e.UserId == s.Id && e.Date == d)?.Grade
            }),
            Average = entries
                .Where(e => e.UserId == s.Id && e.Grade.HasValue && e.Grade > 0)
                .Select(e => e.Grade!.Value)
                .DefaultIfEmpty()
                .Average()
        });

        return Ok(new { Dates = dates, Students = result });
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost]
    public async Task<IActionResult> SetGrade([FromBody] SetGradeRequest request)
    {
        var userId = GetCurrentUserId();

        var existing = await _db.JournalEntries
            .FirstOrDefaultAsync(e =>
                e.UserId == request.StudentId &&
                e.DisciplineId == request.DisciplineId &&
                e.GroupId == request.GroupId &&
                e.Date == request.Date);

        if (existing != null)
        {
            existing.Grade = request.Grade;
            existing.Comment = request.Comment;
        }
        else
        {
            _db.JournalEntries.Add(new JournalEntry
            {
                Id = Guid.NewGuid(),
                UserId = request.StudentId,
                DisciplineId = request.DisciplineId,
                GroupId = request.GroupId,
                Date = request.Date,
                Grade = request.Grade,
                Comment = request.Comment,
                CreatedByUserId = userId
            });
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Оценка сохранена" });
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}

public class SetGradeRequest
{
    public Guid StudentId { get; set; }
    public Guid DisciplineId { get; set; }
    public Guid GroupId { get; set; }
    public DateOnly Date { get; set; }
    public int? Grade { get; set; }
    public string? Comment { get; set; }
}
