
using Microsoft.EntityFrameworkCore;
using WorkFlow.Api.Models;

namespace WorkFlow.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
}
