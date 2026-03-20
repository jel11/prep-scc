using System.Security.Claims;
using CollegeLms.Domain.Entities;
using CollegeLms.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeLms.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class TestsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TestsController(AppDbContext db) => _db = db;

    [Authorize]
    [HttpGet("disciplines/{disciplineId:guid}/tests")]
    public async Task<IActionResult> GetTestsForDiscipline(Guid disciplineId)
    {
        var tests = await _db.Tests
            .Where(t => t.Topic.DisciplineId == disciplineId && t.IsActive)
            .Select(t => new
            {
                t.Id,
                t.Code,
                t.Title,
                t.TimeLimitMinutes,
                TopicTitle = t.Topic.Title,
                QuestionCount = t.Questions.Count
            })
            .ToListAsync();

        return Ok(tests);
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpGet("tests/{id:guid}")]
    public async Task<IActionResult> GetTestFull(Guid id)
    {
        var test = await _db.Tests
            .Where(t => t.Id == id)
            .Select(t => new
            {
                t.Id,
                t.Code,
                t.Title,
                t.TimeLimitMinutes,
                t.ThresholdExcellent,
                t.ThresholdGood,
                t.ThresholdSatisfactory,
                t.IsActive,
                DisciplineId = t.Topic.DisciplineId,
                Questions = t.Questions.OrderBy(q => q.OrderIndex).Select(q => new
                {
                    q.Id,
                    q.Text,
                    q.ImagePath,
                    q.OrderIndex,
                    Answers = q.Answers.OrderBy(a => a.OrderIndex).Select(a => new
                    {
                        a.Id,
                        a.Text,
                        a.IsCorrect,
                        a.OrderIndex
                    })
                })
            })
            .FirstOrDefaultAsync();

        if (test == null)
            return NotFound();

        return Ok(test);
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost("disciplines/{disciplineId:guid}/tests")]
    public async Task<IActionResult> CreateTest(Guid disciplineId, [FromBody] CreateTestRequest request)
    {
        var topic = await _db.Topics.FindAsync(request.TopicId);
        if (topic == null || topic.DisciplineId != disciplineId)
            return BadRequest(new { message = "Тема не принадлежит данной дисциплине" });

        var test = new Test
        {
            Id = Guid.NewGuid(),
            TopicId = request.TopicId,
            Code = request.Code,
            Title = request.Title,
            TimeLimitMinutes = request.TimeLimitMinutes,
            ThresholdExcellent = request.ThresholdExcellent,
            ThresholdGood = request.ThresholdGood,
            ThresholdSatisfactory = request.ThresholdSatisfactory,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tests.Add(test);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTestFull), new { id = test.Id }, new { test.Id, test.Title });
    }

    [Authorize(Roles = "Student")]
    [HttpPost("tests/{testId:guid}/attempts")]
    public async Task<IActionResult> StartAttempt(Guid testId)
    {
        var userId = GetCurrentUserId();

        // Validate student can take this test (group -> discipline -> topic -> test chain)
        var test = await _db.Tests
            .Include(t => t.Topic)
            .FirstOrDefaultAsync(t => t.Id == testId && t.IsActive);

        if (test == null)
            return NotFound();

        var hasAccess = await _db.StudentGroups
            .Where(sg => sg.UserId == userId && sg.RemovedAt == null)
            .SelectMany(sg => sg.Group.GroupDisciplines)
            .AnyAsync(gd => gd.DisciplineId == test.Topic.DisciplineId);

        if (!hasAccess)
            return Forbid();

        // Check no active attempt
        var activeAttempt = await _db.TestAttempts
            .AnyAsync(ta => ta.UserId == userId && ta.TestId == testId && ta.CompletedAt == null);

        if (activeAttempt)
            return Conflict(new { message = "У вас уже есть активная попытка прохождения этого теста" });

        var attempt = new TestAttempt
        {
            Id = Guid.NewGuid(),
            TestId = testId,
            UserId = userId,
            StartedAt = DateTime.UtcNow,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        _db.TestAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        // Return questions without correct answers, shuffled
        var random = new Random();
        var questions = await _db.TestQuestions
            .Where(q => q.TestId == testId)
            .Select(q => new
            {
                q.Id,
                q.Text,
                q.ImagePath,
                Answers = q.Answers.Select(a => new
                {
                    a.Id,
                    a.Text
                }).ToList()
            })
            .ToListAsync();

        var shuffled = questions.OrderBy(_ => random.Next()).Select(q => new
        {
            q.Id,
            q.Text,
            q.ImagePath,
            Answers = q.Answers.OrderBy(_ => random.Next())
        });

        return Ok(new
        {
            AttemptId = attempt.Id,
            test.TimeLimitMinutes,
            attempt.StartedAt,
            Questions = shuffled
        });
    }

    [Authorize(Roles = "Student")]
    [HttpPost("tests/{testId:guid}/attempts/{attemptId:guid}/submit")]
    public async Task<IActionResult> SubmitAttempt(Guid testId, Guid attemptId, [FromBody] SubmitAttemptRequest request)
    {
        var userId = GetCurrentUserId();

        var attempt = await _db.TestAttempts
            .Include(a => a.Test)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId && a.TestId == testId);

        if (attempt == null)
            return NotFound();

        if (attempt.CompletedAt != null)
            return Conflict(new { message = "Попытка уже завершена" });

        // Save answers
        foreach (var answer in request.Answers)
        {
            _db.TestAttemptAnswers.Add(new TestAttemptAnswer
            {
                Id = Guid.NewGuid(),
                AttemptId = attemptId,
                QuestionId = answer.QuestionId,
                SelectedAnswerId = answer.SelectedAnswerId
            });
        }

        // Calculate score
        var totalQuestions = await _db.TestQuestions.CountAsync(q => q.TestId == testId);
        var correctCount = 0;

        foreach (var answer in request.Answers)
        {
            if (answer.SelectedAnswerId == null) continue;
            var isCorrect = await _db.TestAnswers
                .AnyAsync(a => a.Id == answer.SelectedAnswerId && a.IsCorrect);
            if (isCorrect) correctCount++;
        }

        var score = totalQuestions > 0 ? (int)Math.Round(100.0 * correctCount / totalQuestions) : 0;

        // Determine grade
        int grade;
        if (score >= attempt.Test.ThresholdExcellent) grade = 5;
        else if (score >= attempt.Test.ThresholdGood) grade = 4;
        else if (score >= attempt.Test.ThresholdSatisfactory) grade = 3;
        else grade = 2;

        attempt.CompletedAt = DateTime.UtcNow;
        attempt.Score = score;
        attempt.Grade = grade;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            attempt.Score,
            attempt.Grade,
            CorrectCount = correctCount,
            TotalQuestions = totalQuestions,
            attempt.CompletedAt
        });
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}

public class CreateTestRequest
{
    public Guid TopicId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int? TimeLimitMinutes { get; set; }
    public int ThresholdExcellent { get; set; } = 90;
    public int ThresholdGood { get; set; } = 75;
    public int ThresholdSatisfactory { get; set; } = 60;
}

public class SubmitAttemptRequest
{
    public List<SubmitAnswerItem> Answers { get; set; } = new();
}

public class SubmitAnswerItem
{
    public Guid QuestionId { get; set; }
    public Guid? SelectedAnswerId { get; set; }
}
