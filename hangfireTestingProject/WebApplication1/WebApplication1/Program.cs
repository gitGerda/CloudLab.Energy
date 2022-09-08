using Hangfire;
using Hangfire.MySql;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(conf => conf
                                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                        .UseSimpleAssemblyNameTypeSerializer()
                                        .UseRecommendedSerializerSettings()
                                        .UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlStorageOptions())));
builder.Services.AddHangfireServer();
builder.Services.AddMvc();


var app = builder.Build();

app.UseStaticFiles();
app.UseHangfireDashboard();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.MapHangfireDashboard();

app.Run();
