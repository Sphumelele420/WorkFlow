
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkFlow.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Register the database context.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register API controllers.
builder.Services.AddControllers();

// Register standardized error responses.
builder.Services.AddProblemDetails();

// OpenAPI support.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure centralized exception handling.
app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var exceptionFeature =
            context.Features.Get<IExceptionHandlerFeature>();

        var logger = context.RequestServices
            .GetRequiredService<ILogger<Program>>();

        if (exceptionFeature?.Error != null)
        {
            logger.LogError(
                exceptionFeature.Error,
                "An unexpected error occurred while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }

        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An unexpected error occurred.",
            detail: "The server encountered an error while processing your request."
        ).ExecuteAsync(context);
    });
});

app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map controller routes.
app.MapControllers();

app.Run();

public partial class Program { }


