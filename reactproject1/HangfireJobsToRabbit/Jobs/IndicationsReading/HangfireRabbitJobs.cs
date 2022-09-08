using Hangfire;
using HangfireJobsToRabbitLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangfireJobsToRabbitLibrary.Jobs.IndicationsReading
{
    public class HangfireRabbitJobs : IHangfireRabbitJobs
    {
        public Task<bool> ReccuringJobAddOrUpdate(JobCreateSettings settings, List<BrokerTaskMessage> message_list)
        {
            try
            {
                if (string.IsNullOrEmpty(settings.last_datetime_request) ||
                    string.IsNullOrEmpty(settings.rabbit_server_address) || message_list.Count() == 0)
                {
                    return Task.FromResult(false);
                }
                if (_AddOrUpdate(settings, message_list))
                {
                    return Task.FromResult(true);
                };

                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public virtual bool _AddOrUpdate(JobCreateSettings settings, List<BrokerTaskMessage> message_list)
        {
            string json_message = JsonConvert.SerializeObject(message_list);
            var _json_mes_bytes = Encoding.UTF8.GetBytes(json_message);
            RecurringJob.AddOrUpdate(recurringJobId: settings.job_id,
                methodCall: () => ReccJobAddOrUpdateMethodCall(_json_mes_bytes, settings.rabbit_server_address, settings.rabbit_user_name, settings.rabbit_user_password, settings.rabbit_exchange_name, settings.communic_point_name, settings.last_datetime_request),
                cronExpression: settings.periodicity,
                timeZone: TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

            return true;
        }

        public void ReccJobAddOrUpdateMethodCall(byte[] json_message, string rabbit_server_address, string rabbit_user_name, string rabbit_user_password, string rabbit_exchange_name, string communic_point_name, string request_str)
        {
            try
            {
                json_message = ReProcessingRabbitMessage(json_message, request_str).Result;
                var _connection_factory = new ConnectionFactory()
                {
                    HostName = rabbit_server_address,
                    UserName = rabbit_user_name,
                    Password = rabbit_user_password
                };
                var _connection = _connection_factory.CreateConnection();
                var _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: rabbit_exchange_name,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueDeclare(queue: communic_point_name,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueBind(queue: communic_point_name,
                    exchange: rabbit_exchange_name,
                    routingKey: communic_point_name,
                    arguments: null);

                IBasicProperties _basic_props = _channel.CreateBasicProperties();
                _basic_props.Persistent = true;

                _channel.BasicPublish(exchange: rabbit_exchange_name,
                    routingKey: communic_point_name,
                    basicProperties: _basic_props,
                    body: json_message);

                _connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Task<byte[]> ReProcessingRabbitMessage(byte[] prev_json_message, string request_str)
        {
            if (string.IsNullOrEmpty(request_str))
                throw new Exception("Error: empty request_str");

            var _message_list = ConvertMessageToObject(prev_json_message);
            if (_message_list == null)
                throw new Exception("Error: couldn`t convert prev_json_message to List<BrokerTaskMessage> object");

            HttpClient _http_client = new HttpClient(new SocketsHttpHandler());
            foreach (var _message in _message_list)
            {
                string? _last_indication_dateTime = GetLastIndicationDateTime(_http_client, request_str + _message.meter_address).Result;

                if (_last_indication_dateTime == null)
                {
                    _message.last_indication_datetime = "";
                }
                else
                {
                    _message.last_indication_datetime = _last_indication_dateTime;
                }
            }
            _http_client.Dispose();

            return Task.FromResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_message_list)));
        }
        public virtual List<BrokerTaskMessage>? ConvertMessageToObject(byte[] message)
        {
            try
            {
                string _prev_json_mes_str = Encoding.UTF8.GetString(message);
                List<BrokerTaskMessage> result = JsonConvert.DeserializeObject<List<BrokerTaskMessage>>(_prev_json_mes_str);
                return result;
            }
            catch
            {
                return null;
            }

        }

        public virtual Task<string?> GetLastIndicationDateTime(HttpClient httpClient, string request_str)
        {
            try
            {
                var response = httpClient.GetAsync(request_str).Result;
                if (response.IsSuccessStatusCode)
                {
                    var response_obj = response.Content.ReadAsStringAsync().Result;
                    var _last_date_time_obj = JsonConvert.DeserializeObject<LastIndicationHttpResponse>(response_obj);
                    return Task.FromResult($"{_last_date_time_obj.last_date} {_last_date_time_obj.last_time}");
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return Task.FromResult("");
                    }
                    else
                    {
                        return null;
                    };
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
