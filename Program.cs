using Homework7.DiExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services
    .AddFluentMigrator()
    .AddEf()
    .AddControllers();

var app = builder.Build();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//app.MigrateUp(); - команда для FluentMigrator

app.Run();



