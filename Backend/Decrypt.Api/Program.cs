using Decrypt.Api.Extensions;
using Decrypt.Api.Middleware;
using Decrypt.DataAccess.DataSources;
using Decrypt.DataAccess.DataSources.Interfaces;
using Decrypt.DataAccess.Extensions;
using Decrypt.Logic.Abstractions;
using Decrypt.Logic.Services;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



//Swagger config 
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();



//Configure DB connection
builder.Services.ConfigureDbContext(builder.Configuration.GetConnectionString("DecryptDatabase"));

//Register dependencies
builder.Services.AddScoped(typeof(IGenericDataSource<>), typeof(GenericDataSource<>));
builder.Services.AddScoped<IDashboardService, DashboardService>();

//Logic

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseMiddleware<ErrorsInterceptor>();

app.UseHttpsRedirection();

app.UseCors("DefaultCors");

app.UseAuthorization();

app.MapControllers();

//Automatic apply of migrations
app.DoMigrate(args).Run();
