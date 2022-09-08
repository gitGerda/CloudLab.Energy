namespace kzmpCloudAPI.Components.HangfireSheduler.UpdateShedulePart
{
    public interface IUpdateShedule
    {
        /// <summary>
        /// Функция обновления таблицы сопоставления расписания с счётчиками (SheduleMeters)
        /// </summary>
        /// <param name="shedule_id"></param>
        /// <param name="selected_meters_id"></param>
        public void SheduleMetersTableUpdate(int shedule_id, List<int> selected_meters_id);
        /// <summary>
        /// Функция обработки смены статуса и точки опроса расписания
        /// </summary>
        /// <param name="shedule_id"></param>
        /// <param name="communic_point_id"></param>
        public void ProcessCommunicPointChange(int shedule_id, int communic_point_id);
        /// <summary>
        /// Функция обновления даты создания (изменения) расписания
        /// </summary>
        /// <param name="shedule_id"></param>
        public void UpdateSheduleCreatingDate(int shedule_id);
        /// <summary>
        /// Обновление периодичности в таблице SheduleMeters 
        /// </summary>
        /// <param name="shedule_id"></param>
        /// <param name="periodicity"></param>
        public void UpdatePeriodicity(int shedule_id, string periodicity);



    }
}
