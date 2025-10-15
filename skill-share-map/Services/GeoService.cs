using System.Text.Json;

namespace SkillShareMap.Services;

public class GeoService : IGeoService
{
    private readonly HttpClient _httpClient;
    private const string NominatimBaseUrl = "https://nominatim.openstreetmap.org";

    public GeoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Set User-Agent as required by Nominatim usage policy
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SkillShareMap/1.0");
    }

    /// <summary>
    /// Calculate the distance between two geographic coordinates using the Haversine formula
    /// </summary>
    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371.0;

        // Convert degrees to radians
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var lat1Rad = DegreesToRadians(lat1);
        var lat2Rad = DegreesToRadians(lat2);

        // Haversine formula
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) *
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    /// <summary>
    /// Geocode an address using Nominatim API
    /// </summary>
    public async Task<(double lat, double lon)?> GeocodeAddressAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return null;

        try
        {
            var encodedAddress = Uri.EscapeDataString(address);
            var url = $"{NominatimBaseUrl}/search?q={encodedAddress}&format=json&limit=1";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<List<NominatimResult>>(json);

            if (results == null || results.Count == 0)
                return null;

            var result = results[0];
            if (double.TryParse(result.lat, out var lat) && double.TryParse(result.lon, out var lon))
            {
                return (lat, lon);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    // DTO for Nominatim API response
    private class NominatimResult
    {
        public string lat { get; set; } = string.Empty;
        public string lon { get; set; } = string.Empty;
    }
}
