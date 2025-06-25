using CRUDforAngular.BusinessLayer.CommonService;
using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.BusinessLayer.Repos;
using CRUDforAngular.Services;
using Microsoft.EntityFrameworkCore;
 

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:8080");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();
//add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MyDBContext>( 
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("LocalConStr") 
    )
); 
builder.Services.AddScoped<IuserRegistrationRepo, userRegistrationRepo>();
builder.Services.AddScoped<IuserProfileRepo, userProfileRepo>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name:"AngularSite", builder =>
    {
        builder.WithOrigins(["http://localhost:4200", "http://localhost:8080"]).AllowAnyHeader().AllowAnyMethod();
    });

});

builder.Services.Configure<smtpOptions>(builder.Configuration.GetSection(nameof(smtpOptions)));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularSite");
app.UseRouting();


app.UseAuthorization();
 

app.MapControllers();

app.Run();
