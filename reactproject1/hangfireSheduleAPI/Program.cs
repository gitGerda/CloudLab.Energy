using Hangfire;
using Hangfire.MySql;
using hangfireSheduleAPI.Filters.AuthFilter;
using HangfireJobsToRabbitLibrary.Jobs.IndicationsReading;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(configuration =>
{
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlStorageOptions()));
});
builder.Services.AddHangfireServer();
builder.Services.AddMvc();
var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    Authorization = new[] { new AuthFilter(app.Services) }
});

app.UseAuthorization();


app.MapControllers();
app.MapHangfireDashboard();

app.Run();
