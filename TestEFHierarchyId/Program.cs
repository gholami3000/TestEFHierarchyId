using Microsoft.EntityFrameworkCore;
using TestEFHierarchyId.models;
using TestEFHierarchyId.Services;

var builder = WebApplication.CreateBuilder(args);
var configurations = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<HierarchyService>();


//builder.Services.AddDbContext<HierarchyDbContext>(opt =>
//{
//    opt.UseSqlServer(configurations.GetConnectionString("TestEFHierarchyIdConection").
   
//},);

builder.Services.AddDbContextFactory<HierarchyDbContext>(options =>
       options.UseSqlServer(configurations.GetConnectionString("TestEFHierarchyIdConection"), x => x.UseHierarchyId()));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
