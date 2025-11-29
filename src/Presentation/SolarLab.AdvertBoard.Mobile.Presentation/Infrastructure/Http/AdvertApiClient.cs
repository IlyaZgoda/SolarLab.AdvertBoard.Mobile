using Microsoft.AspNetCore.WebUtilities;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Base;
using System.Net.Http.Json;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public class AdvertApiClient(IHttpClientFactory factory) : IAdvertApiClient
    {
        private readonly HttpClient _httpClient = factory.CreateClient("BaseApi");

        public async Task<PaginationCollection<PublishedAdvertItem>> GetPublishedAsync(AdvertFilterRequest filter)
        {
            var parameters = new Dictionary<string, string?>
            {
                ["Page"] = filter.Page.ToString(),
                ["PageSize"] = filter.PageSize.ToString(),
            };

            var url = QueryHelpers.AddQueryString("/api/adverts", parameters);

            var response = await _httpClient.GetAsync(url);  

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaginationCollection<PublishedAdvertItem>>();
        }
    }
}