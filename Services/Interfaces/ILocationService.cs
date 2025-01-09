using CarChargingApi.Models.Data;
using CarChargingApi.Models.Requests;

namespace CarChargingApi.Services.Interfaces
{
    public interface ILocationService
    {
        Task<Location> GetLocationAsync(string id);

        Task CreateLocationAsync(LocationRequestModel requestModel);

        Task UpdateLocationAsync(string id, PatchLocationRequestModel requestModel);

        Task UpdateLocationChargePointsAsync(string id, ChargePointRequestModel requestModel);
    }
}
