using CollegeLms.Domain.Enums;
using CollegeLms.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeLms.Api.Controllers;

[ApiController]
[Route("api/v1/teachers")]
public class TeachersController : ControllerBase
{
    private readonly AppDbContext _db;

    public TeachersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teachers = await _db.Users
            .Where(u => u.Role == UserRole.Teacher && u.IsActive)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.PhotoPath,
                u.Bio,
                Disciplines = u.TeacherDisciplines.Select(td => new
                {
                    td.Discipline.Id,
                    td.Discipline.Name,
                    td.Discipline.Code
                })
            })
            .ToListAsync();

        return Ok(teachers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var teacher = await _db.Users
            .Where(u => u.Id == id && u.Role == UserRole.Teacher)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.PhotoPath,
                u.Bio,
                Disciplines = u.TeacherDisciplines.Select(td => new
                {
                    td.Discipline.Id,
                    td.Discipline.Name,
                    td.Discipline.Code,
                    td.Discipline.Description
                })
            })
            .FirstOrDefaultAsync();

        if (teacher == null)
            return NotFound();

        return Ok(teacher);
    }
}
