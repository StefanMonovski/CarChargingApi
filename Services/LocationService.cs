using CarChargingApi.Common.Enums;
using CarChargingApi.Common.Exceptions;
using CarChargingApi.Common.Resources;
using CarChargingApi.Data;
using CarChargingApi.Models.Data;
using CarChargingApi.Models.Requests;
using CarChargingApi.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static CarChargingApi.Common.Constants.Constants;

namespace CarChargingApi.Services
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<LocationService> _logger;

        public LocationService(ApplicationDbContext dbContext, ILogger<LocationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Location> GetLocationAsync(string id)
        {
            _logger.LogInformation("Fetching location with id {id}.", id);

            var location = await _dbContext.Locations
                .Include(l => l.ChargePoints)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LocationId == id);

            if (location == null)
            {
                var errorMessage = string.Format(StringResources.LocationNotFoundErrorMessage, id);
                throw new NotFoundException(errorMessage);
            }

            return location;
        }

        public async Task CreateLocationAsync(LocationRequestModel requestModel)
        {
            _logger.LogInformation("Saving location to the database.");

            var location = new Location()
            {
                LocationId = requestModel.LocationId,
                Type = requestModel.Type,
                Name = requestModel.Name,
                Address = requestModel.Address,
                City = requestModel.City,
                PostalCode = requestModel.PostalCode,
                Country = requestModel.Country,
                LastUpdated = DateTime.Now
            };

            try
            {
                _dbContext.Locations.Add(location);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                _logger.LogError(ex, ex.Message);

                if (sqlEx.Number == ErrorCode.UniquePrimaryKeyViolation)
                {
                    var errorMessage = string.Format(StringResources.DuplicateLocationErrorMessage, location.LocationId);
                    throw new DuplicateEntityException(errorMessage);
                }

                throw;
            }

            return;
        }

        public async Task UpdateLocationAsync(string id, PatchLocationRequestModel requestModel)
        {
            _logger.LogInformation("Fetching location with id {id}.", id);

            var location = await _dbContext.Locations
                .FirstOrDefaultAsync(l => l.LocationId == id);

            if (location == null)
            {
                var errorMessage = string.Format(StringResources.LocationNotFoundErrorMessage, id);
                throw new NotFoundException(errorMessage);
            }
            else
            {
                _logger.LogInformation("Updating location with id {id}.", id);

                if (requestModel.Type.HasValue) location.Type = (LocationType)requestModel.Type;
                if (!string.IsNullOrEmpty(requestModel.Name)) location.Name = requestModel.Name;
                if (!string.IsNullOrEmpty(requestModel.Address)) location.Address = requestModel.Address;
                if (!string.IsNullOrEmpty(requestModel.City)) location.City = requestModel.City;
                if (!string.IsNullOrEmpty(requestModel.PostalCode)) location.PostalCode = requestModel.PostalCode;
                if (!string.IsNullOrEmpty(requestModel.Country)) location.Country = requestModel.Country;
                location.LastUpdated = DateTime.Now;

                await _dbContext.SaveChangesAsync();
            }

            return;
        }

        public async Task UpdateLocationChargePointsAsync(string id, ChargePointRequestModel requestModel)
        {
            _logger.LogInformation("Fetching location with id {id}.", id);

            var location = await _dbContext.Locations
                .Include(l => l.ChargePoints)
                .FirstOrDefaultAsync(l => l.LocationId == id);

            var currentChargePoints = await _dbContext.ChargePoints.ToListAsync();

            if (location == null)
            {
                var errorMessage = string.Format(StringResources.LocationNotFoundErrorMessage, id);
                throw new NotFoundException(errorMessage);
            }
            else
            {
                _logger.LogInformation("Updating charge points for location with id {id}.", id);

                foreach (var chargePoint in requestModel.ChargePoints)
                {
                    var updateChargePoint = currentChargePoints.Where(x => x.ChargePointId == chargePoint.ChargePointId).FirstOrDefault();

                    //Charge points that already exist will have values updated and non-existing will be added to the location
                    if (updateChargePoint != null)
                    {
                        if (chargePoint.LocationId != id)
                        {
                            updateChargePoint.LocationId = id;
                        }

                        updateChargePoint.Status = chargePoint.Status;
                        updateChargePoint.FloorLevel = chargePoint.FloorLevel;
                        updateChargePoint.LastUpdated = DateTime.Now;
                    }
                    else
                    {
                        location.ChargePoints.Add(chargePoint);
                    }
                }

                foreach (var chargePoint in location.ChargePoints)
                {
                    //Charge points that are not included in the update will be marked as removed
                    if (!requestModel.ChargePoints.Select(x => x.ChargePointId).Contains(chargePoint.ChargePointId))
                    {
                        chargePoint.Status = ChargePointStatus.Removed;
                    }
                }

                location.LastUpdated = DateTime.Now;

                await _dbContext.SaveChangesAsync();
            }

            return;
        }
    }
}