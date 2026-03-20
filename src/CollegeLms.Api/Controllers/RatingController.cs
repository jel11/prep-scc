using System.Security.Claims;
using CollegeLms.Domain.Entities;
using CollegeLms.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeLms.Api.Controllers;

[ApiController]
[Route("api/v1/rating")]
[Authorize]
public class RatingController : ControllerBase
{
    private readonly AppDbContext _db;

    public RatingController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetRating([FromQuery] Guid groupId, [FromQuery] Guid disciplineId)
    {
        var students = await _db.StudentGroups
            .Where(sg => sg.GroupId == groupId && sg.RemovedAt == null)
            .Select(sg => sg.User)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        var entries = await _db.RatingEntries
            .Where(e => e.GroupId == groupId && e.DisciplineId == disciplineId)
            .ToListAsync();

        var dates = entries.Select(e => e.Date).Distinct().OrderBy(d => d).ToList();

        var result = students.Select(s =>
        {
            var studentEntries = entries.Where(e => e.UserId == s.Id).ToList();
            var totalClasses = dates.Count;
            var attended = studentEntries.Count(e => e.IsPresent);

            return new
            {
                StudentId = s.Id,
                s.FullName,
                Grades = dates.Select(d =>
                {
                    var entry = studentEntries.FirstOrDefault(e => e.Date == d);
                    return new
                    {
                        Date = d,
                        Grade = entry?.Grade ?? 0,
                        IsPresent = entry?.IsPresent ?? false
                    };
                }),
                AverageGrade = studentEntries.Any()
                    ? Math.Round(studentEntries.Average(e => (double)e.Grade), 2)
                    : 0.0,
                AttendancePercent = totalClasses > 0
                    ? Math.Round(100.0 * attended / totalClasses, 1)
                    : 0.0
            };
        });

        return Ok(new { Dates = dates, Students = result });
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost]
    public async Task<IActionResult> SetRating([FromBody] SetRatingRequest request)
    {
        var userId = GetCurrentUserId();

        var existing = await _db.RatingEntries
            .FirstOrDefaultAsync(e =>
                e.UserId == request.StudentId &&
                e.DisciplineId == request.DisciplineId &&
                e.GroupId == request.GroupId &&
                e.Date == request.Date);

        if (existing != null)
        {
            existing.Grade = request.Grade;
            existing.IsPresent = request.IsPresent;
            existing.Comment = request.Comment;
        }
        else
        {
            _db.RatingEntries.Add(new RatingEntry
            {
                Id = Guid.NewGuid(),
                UserId = request.StudentId,
                DisciplineId = request.DisciplineId,
                GroupId = request.GroupId,
                Date = request.Date,
                Grade = request.Grade,
                IsPresent = request.IsPresent,
                Comment = request.Comment,
                CreatedByUserId = userId
            });
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Рейтинг обновлён" });
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}

public class SetRatingRequest
{
    public Guid StudentId { get; set; }
    public Guid DisciplineId { get; set; }
    public Guid GroupId { get; set; }
    public DateOnly Date { get; set; }
    public int Grade { get; set; }
    public bool IsPresent { get; set; }
    public string? Comment { get; set; }
}
