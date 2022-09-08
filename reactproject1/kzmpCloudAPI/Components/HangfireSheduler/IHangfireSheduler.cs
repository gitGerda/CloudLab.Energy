namespace kzmpCloudAPI.Components.HangfireSheduler
{
    public interface IHangfireSheduler<T>
    {
        /// <summary>
        /// Создание расписания Hangfire
        /// </summary>
        /// <param name="shedule"></param>
        /// <returns></returns>
        Task<bool> CreateShedule(T shedule);
        /// <summary>
        /// Обновление расписания Hangfire
        /// </summary>
        /// <param name="shedule"></param>
        /// <returns></returns>
        Task<bool> UpdateShedule(T shedule);
        /// <summary>
        /// Удаление расписания Hangfire с сохранением информации о расписании в БД веб-сервиса
        /// </summary>
        /// <param name="shedule"></param>
        /// <returns></returns>
        bool DeleteSheduleFromSheduler(int shedule_id);
        /// <summary>
        /// Полное удаление расписание.
        /// </summary>
        /// <param name="shedule_id"></param>
        /// <returns></returns>
        bool DeleteSheduleFull(int shedule_id);
    }
}
