using SkillShareMap.Models;
using SkillShareMap.Services;

namespace SkillShareMap.Services;

public interface IGeoService
{
    /// <summary>
    /// Calculate the distance between two geographic coordinates using the Haversine formula
    /// </summary>
    /// <param name="lat1">Latitude of the first point</param>
    /// <param name="lon1">Longitude of the first point</param>
    /// <param name="lat2">Latitude of the second point</param>
    /// <param name="lon2">Longitude of the second point</param>
    /// <returns>Distance in kilometers</returns>
    double CalculateDistance(double lat1, double lon1, double lat2, double lon2);

    /// <summary>
    /// Geocode an address using Nominatim API
    /// </summary>
    /// <param name="address">The address to geocode</param>
    /// <returns>Tuple of (latitude, longitude) or null if not found</returns>
    Task<(double lat, double lon)?> GeocodeAddressAsync(string address);

    /// <summary>
    /// Search for addresses (Multiple results for Autocomplete)
    /// </summary>
    Task<List<LocationResult>> SearchAddressAsync(string address);
}
