using KzmpEnergyIndicationsLibrary.Models.Indications;

namespace KzmpEnergyIndicationsLibrary.Interfaces.IDevices
{
    public interface ICommonIndicationsReader
    {
        Task<EnergyRecordResponse> GetEnergyRecordAsync(int month, int year);
        Task<PowerProfileRecordResponse> GetPowerProfileRecordAsync();
        Task<SessionInitializationResponse> SessionInitializationAsync();
    }
}