using CRUDforAngular.BusinessLayer.CommonService;
using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.BusinessLayer.Repos;
using CRUDforAngular.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
 
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:8080");

// Add services to the container.
 
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


builder.Services.AddOpenApi();
//add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CRUDforAngular API", Version = "v1" });

    // Add JWT Bearer definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//builder.Services.AddDbContext<MyDBContext>( 
//    options => options.UseSqlServer(
//        builder.Configuration.GetConnectionString("LocalConStr") 
//    )
//);

builder.Services.AddDbContext<MyDBContext>(
    options=> options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConStr")
    ));
 
builder.Services.AddScoped<IuserRegistrationRepo, userRegistrationRepo>();
builder.Services.AddScoped<IuserProfileRepo, userProfileRepo>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name:"AngularSite", builder =>
    {
        builder.WithOrigins(["http://localhost:4200", "http://localhost:8080","https://ruthik-first-project-silk.vercel.app"]).AllowAnyHeader().AllowAnyMethod();
    });

});

builder.Services.Configure<smtpOptions>(builder.Configuration.GetSection(nameof(smtpOptions)));

// implement seri Log for logging
Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Information()
   .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
   .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
   .CreateLogger(); 
builder.Host.UseSerilog();


//add jwt authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["jwt:issuer"],
            ValidAudience = builder.Configuration["jwt:audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["jwt:key"]))
        };

    });

 
var app = builder.Build();


// Global Exception Handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = new
        {
            Message = "An unexpected error occurred. Please try again later."
        };

        await context.Response.WriteAsJsonAsync(error);
    });
});


// Configure the HTTP request pipeline.

//app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
 

app.UseHttpsRedirection();
app.UseCors("AngularSite");
app.UseRouting();


app.UseAuthorization();
 

app.MapControllers();

app.Run();
