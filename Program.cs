using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using TaskTrackerApi.Data;
using TaskTrackerApi.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "_myAllowSpecificOrigins",
		policy =>
		{
			policy.AllowAnyOrigin()
				  .AllowAnyHeader()
				  .AllowAnyMethod();
		});
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter your Bearer token"
	});

	options.AddSecurityRequirement(document =>
		new OpenApiSecurityRequirement
		{
			[new OpenApiSecuritySchemeReference("Bearer", document)] = []
		});
});

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
//	builder.Configuration.GetConnectionString("DefaultConnection")
//	));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(
		builder.Configuration.GetConnectionString("DefaultConnection")
	));

builder.Services
	.AddIdentityApiEndpoints<ApplicationUser>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskTracker Api v1");
	options.RoutePrefix = string.Empty; // Access at root `/`
});

app.UseCors("_myAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<ApplicationUser>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
