
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
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        ApplicationDbContext context,
        ILogger<EmployeesController> logger)
    {
        _context = context;
        _logger = logger;
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

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlException
            && (sqlException.Number == 2601
                || sqlException.Number == 2627))
        {
            _logger.LogWarning(
                "Employee creation failed because the email address already exists.");

            return Conflict(new
            {
                message = "An employee with this email address already exists."
            });
        }

        _logger.LogInformation(
            "Employee {EmployeeId} was created successfully.",
            employee.Id);

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
            _logger.LogWarning(
                "Update rejected because employee {EmployeeId} is inactive.",
                id);

          return Problem(
    statusCode: StatusCodes.Status409Conflict,
    title: "Employee is inactive",
    detail: "Inactive employees cannot be updated.");
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

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlException
            && (sqlException.Number == 2601
                || sqlException.Number == 2627))
        {
            _logger.LogWarning(
                "Employee update rejected because the email address already exists for employee {EmployeeId}.",
                id);

            return Problem(
    statusCode: StatusCodes.Status409Conflict,
    title: "Duplicate employee email",
    detail: "An employee with this email address already exists.");
        }

        _logger.LogInformation(
            "Employee {EmployeeId} was updated successfully.",
            id);

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

        _logger.LogInformation(
            "Employee {EmployeeId} was deactivated successfully.",
            id);

        return NoContent();
    }
}
