using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using OrderService.Api.Application.Behaviors;
using OrderService.Api.Application.Orders.CancelOrder;
using OrderService.Api.Application.Orders.CreateOrder;
using OrderService.Api.Domain.Common;
using OrderService.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
// Registro del repositorio requerido por los Handlers
builder.Services.AddScoped<IOrderRepository, InMemoryOrderRepository>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OrderService API",
        Version = "v1"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/orders", async (
    CreateOrderCommand command,
    ISender sender,
    CancellationToken ct) =>
{
    var result = await sender.Send(command, ct);

    return result.IsSuccess
        ? Results.Created($"/api/orders/{result.Value}", new { id = result.Value })
        : result.ToHttpProblem();
});

app.MapPost("/api/orders/{orderId:guid}/cancel", async (
    Guid orderId,
    ISender sender,
    CancellationToken ct) =>
{
    var result = await sender.Send(new CancelOrderCommand(orderId), ct);

    return result.IsSuccess
        ? Results.NoContent()
        : result.ToHttpProblem();
});

app.Run();