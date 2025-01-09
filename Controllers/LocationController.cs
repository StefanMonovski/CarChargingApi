using CarChargingApi.Common.Exceptions;
using CarChargingApi.Common.Resources;
using CarChargingApi.Models.Data;
using CarChargingApi.Models.Requests;
using CarChargingApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CarChargingApi.Controllers
{
    [ApiController]
    [Route("locations")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILocationService locationService, ILogger<LocationController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        [HttpGet("{locationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Location>> GetLocation([StringLength(39)] string locationId)
        {
            try
            {
                var location = await _locationService.GetLocationAsync(locationId);
                return Ok(location);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, StringResources.InternalServerErrorMessage);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateLocation([FromBody] LocationRequestModel location)
        {
            try
            {
                await _locationService.CreateLocationAsync(location);
                return Created();
            }
            catch (DuplicateEntityException ex)
            {
                _logger.LogError(ex, ex.Message);
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, StringResources.InternalServerErrorMessage);
            }
        }

        [HttpPatch("{locationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLocation([StringLength(39)] string locationId, [FromBody] PatchLocationRequestModel location)
        {
            try
            {
                await _locationService.UpdateLocationAsync(locationId, location);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, StringResources.InternalServerErrorMessage);
            }
        }

        [HttpPut("{locationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLocationChargePoints([StringLength(39)] string locationId, [FromBody] ChargePointRequestModel chargePoints)
        {
            try
            {
                await _locationService.UpdateLocationChargePointsAsync(locationId, chargePoints);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, StringResources.InternalServerErrorMessage);
            }
        }
    }
}
