using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Hangfire.MySql;
using RabbitMQ.Client;
using WebApplication2_client.rabbit;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace WebApplication2_client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleController : ControllerBase
    {
        // IModel channel;
        ILogger<SimpleController> _logger;
        IConfiguration _conf;
        public SimpleController(ILogger<SimpleController> logger,IConfiguration conf)
        {
            //this.channel = channel;
            _logger = logger;
            _conf = conf;
        }

        [HttpPost("postmessage")]
        public async void PostMessage(string? mes)
        {

            var s = _conf["emptt"];
           /* JobStorage.Current = new MySqlStorage("Server=192.168.0.64;port=3307;user=root;pwd=Otohof96;Database=HangfireTesting;Allow User Variables=true", new MySqlStorageOptions());

            //BackgroundJob.Enqueue(() => Console.WriteLine(mes));
            RecurringJob.AddOrUpdate<rabbitEmitter>(recurringJobId: mes,

                r => r.Write(mes),

                Cron.Daily);*/

            //BackgroundJob.Enqueue(() =>Console.WriteLine(DateTime.Now.ToString()));
            //RecurringJob.AddOrUpdate(recurringJobId: "1", () => Console.WriteLine(DateTime.Now.ToString()), Cron.Minutely);

            //BackgroundJob.Enqueue<rabbitEmitter>(r => r.EmitMessage(mes));

            //BackgroundJob.Enqueue<rabbitEmitter>(r => r.EmitMessage(mes));



            /*HttpClient client = new HttpClient(new SocketsHttpHandler());

            try
            {
                string request = "http://localhost:80/api/indications/PowerIndications/get_last_indic_datetime?meter_address=1";

                var response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string s = await response.Content.ReadAsStringAsync();
                var f = JsonConvert.DeserializeObject<response>(s);

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
            finally
            {
                client.Dispose();
            }*/
        }
    }

    public class response
    {
        public string last_date;
        public string last_time;
    }
}
