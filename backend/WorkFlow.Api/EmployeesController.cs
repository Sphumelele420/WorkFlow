
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkFlow.Api.Data;
using WorkFlow.Api.DTOs;
using WorkFlow.Api.Models;

namespace WorkFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/employees
    // Return active employees only.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        var employees = await _context.Employees
            .AsNoTracking()
            .Where(e => e.IsActive)
            .ToListAsync();

        return Ok(employees);
    }

    // GET: api/employees/5
    // Retrieve an employee by ID, including inactive records.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    // POST: api/employees
    // Create a new employee.
    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(
        EmployeeRequest request)
    {
        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            JobTitle = request.JobTitle,
            Department = request.Department,
            DateHired = request.DateHired ?? DateTime.UtcNow,
            IsActive = true
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = employee.Id },
            employee);
    }

    // PUT: api/employees/5
    // Update an active employee.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(
        int id, EmployeeRequest request)
    {
        var existingEmployee = await _context.Employees.FindAsync(id);

        if (existingEmployee == null)
        {
            return NotFound();
        }

        if (!existingEmployee.IsActive)
        {
            return Conflict(
                "Inactive employees cannot be updated.");
        }

        existingEmployee.FirstName = request.FirstName;
        existingEmployee.LastName = request.LastName;
        existingEmployee.Email = request.Email;
        existingEmployee.JobTitle = request.JobTitle;
        existingEmployee.Department = request.Department;

        if (request.DateHired.HasValue)
        {
            existingEmployee.DateHired = request.DateHired.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/employees/5
    // Soft-delete by marking the employee inactive.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeactivateEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        if (!employee.IsActive)
        {
            return NoContent();
        }

        employee.IsActive = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
