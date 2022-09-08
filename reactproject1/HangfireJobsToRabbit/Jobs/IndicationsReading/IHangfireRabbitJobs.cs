using HangfireJobsToRabbitLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangfireJobsToRabbitLibrary.Jobs.IndicationsReading
{
    public interface IHangfireRabbitJobs
    {
        /// <summary>
        /// Функция создаёт или обновляет повторяющееся расписание Hangfire
        /// </summary>
        /// <param name="message_list"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public Task<bool> ReccuringJobAddOrUpdate(JobCreateSettings settings, List<BrokerTaskMessage> message_list);

        /// <summary>
        /// Функция Hangfire повторяющегося расписания для выполнения
        /// </summary>
        /// <param name="communic_point_name"></param>
        /// <param name="rabbit_exchange_name"></param>
        /// <param name="rabbit_server_address"></param>
        /// <param name="rabbit_user_name"></param>
        /// <param name="json_message"></param>
        /// <param name="rabbit_user_password"></param>
        ///<param name="request_str"></param>
        public void ReccJobAddOrUpdateMethodCall(byte[] json_message, string rabbit_server_address, string rabbit_user_name, string rabbit_user_password, string rabbit_exchange_name, string communic_point_name, string request_str);
        /// <summary>
        /// Функция, которая пересчитывает поле даты последнего измерения при вызове метода ReccJobAddOrUpdateMethodCall
        /// </summary>
        /// <param name="prev_message"></param>
        /// <param name="request_str"></param>
        /// <returns></returns>
        public Task<byte[]> ReProcessingRabbitMessage(byte[] prev_message, string request_str);
        /// <summary>
        /// Функция для получения даты последнего измерения счётчика 
        /// </summary>
        /// <param name="http_client"></param>
        ///<param name="request_str"></param>
        /// <returns></returns>
        public Task<string?> GetLastIndicationDateTime(HttpClient http_client, string request_str);
    }
}
