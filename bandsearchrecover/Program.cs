using BandSearch.Database;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using BandSearch.Web.Configuration;
using BandSearch.Web.Options;
using BandSearch.Web.Database;

var builder = WebApplication.CreateBuilder(args);

var conf = builder.Configuration;

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddControllers();
builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(conf.GetConnectionString("DefaultConnection")));

//builder.Services.AddHttpClient("band_search", c =>
//{
//    c.BaseAddress = new Uri("https://localhost:7149/");
//}).AddHeaderPropagation();

builder.Services.AddSingleton<BandSearchMemoryCache>();

ServicesConfiguration.ConfigureLoggingService(builder);
ServicesConfiguration.ConfigureUserServices(builder);
ServicesConfiguration.ConfigureBandServices(builder);
ServicesConfiguration.ConfigureOpenPositionServices(builder);
ServicesConfiguration.ConfigureAuthServices(builder);
ServicesConfiguration.ConfigureInstrumentLevelServices(builder);

builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseHeaderPropagation();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Test1 Api v1");
});

if (!builder.Environment.IsDevelopment())
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // using static System.Net.Mime.MediaTypeNames;
            context.Response.ContentType = Text.Plain;

            await context.Response.WriteAsync("An exception was thrown.");

            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is KeyNotFoundException)
            {
                await context.Response.WriteAsync(" The user was not found.");
            }
        });
    });
}



// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
