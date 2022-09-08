using Hangfire;
using Hangfire.MySql;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(conf => conf
                                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                        .UseSimpleAssemblyNameTypeSerializer()
                                        .UseRecommendedSerializerSettings()
                                        .UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlStorageOptions())));

/*builder.Services.AddSingleton<IModel>((IServiceProvider service) =>
{
    string _exchangeName = "cloudlab_energy_indications_ex";
    string _routingKey = "cloudlab_energy_indications_queue";

    var connectionFactory = new ConnectionFactory()
    {
        HostName = "85.193.83.154",
        UserName = "MeasuringPointUser",
        Password = "Otohof96"
    };
    var connection = connectionFactory.CreateConnection();
    var channel = connection.CreateModel();

    channel.ExchangeDeclare(exchange: _exchangeName,
        type: ExchangeType.Direct,
        durable: true,
        autoDelete: false,
        arguments: null);

    return channel;
});*/

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
