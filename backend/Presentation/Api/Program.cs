using Api.ExceptionHandlers;
using Application.Registration;
using Microsoft.AspNetCore.Mvc;
using Persistance.Registration;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration["AutoMapper:LicenseKey"]);
builder.Services.AddPersistanceServices(builder.Configuration);

builder.Services.AddExceptionHandler<DeleteRestrictedExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "The request body is invalid.",
                Detail = "The request body is not valid JSON or contains values of the wrong type."
            };
            problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            return new BadRequestObjectResult(problem);
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("CORS");
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.Run();
