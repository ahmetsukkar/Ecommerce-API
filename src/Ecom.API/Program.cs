using Ecom.API.Extensions;
using Ecom.API.Middleware;
using Ecom.Core.Services;
using Ecom.Infrastructure;
using Ecom.Infrastructure.Data;
using Ecom.Infrastructure.Repositories;
using Ecom.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//ConfigureAPIBehaviorOptions (Catch ValidationError)
builder.Services.AddAPIRegistration();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
	var securitySchema = new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "JWT Auth Bearer",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		Reference = new OpenApiReference
		{
			Id = "Bearer",
			Type = ReferenceType.SecurityScheme
		}
	};
	s.AddSecurityDefinition("Bearer", securitySchema);
	var securityRequirement = new OpenApiSecurityRequirement { { securitySchema, new[] { "Bearer" } } };
	s.AddSecurityRequirement(securityRequirement);
});

//Services Register
builder.Services.InfrastructureConfiguration(builder.Configuration);

//Configure Order Services
builder.Services.AddScoped<IOrderServices, OrderServices>();

//Configure IFileProvider
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
	Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors/{0}"); //This middleware to redirect any error inside the app to ErrorsController

app.UseHttpsRedirection();

app.UseStaticFiles(); //Configures the application to serve static files to clients.

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//// To Create the Database Tabels __ Depending on ConnectionString that exist in appsettings.json file
//using (var scope = app.Services.CreateScope())
//{
//	var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//	db.Database.Migrate();
//}

InfrastructureRegistration.InfrastructreConfigurationMiddleware(app);

app.Run();
