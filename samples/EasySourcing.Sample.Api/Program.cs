using System.Reflection;
using EasySourcing.DependencyInjection;
using EasySourcing.EntityFrameworkCore.DependencyInjection;
using EasySourcing.Sample.Core.Data;
using EasySourcing.Sample.Core.Interfaces;
using EasySourcing.Sample.Core.ReadModels;
using EasySourcing.Sample.Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<EventSourcingOptions>(options => { options.TakeEachSnapshotVersion = 5; });

builder.Services.AddDbContext<SampleDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddEasySourcing(typeof(PostReadModelGenerator).Assembly,
    eventSourcingBuilder => { eventSourcingBuilder.UseEfCoreStore<SampleDbContext>(); });

builder.Services.AddScoped<IPostQueries, PostQueries>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;
    try
    {
        var dbContext = scopedProvider.GetRequiredService<SampleDbContext>();
        if (dbContext.Database.IsSqlServer())
        {
            dbContext.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred seeding the database.");
    }
}

app.Run();