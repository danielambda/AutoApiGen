using TestConsumer;
using TestConsumer.Features.Students;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .AddControllers();

builder.Services
    .AddSingleton<StudentsRepo>();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<IApiMarker>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();