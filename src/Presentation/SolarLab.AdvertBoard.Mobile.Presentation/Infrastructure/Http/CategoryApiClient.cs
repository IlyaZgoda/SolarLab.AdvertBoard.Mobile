using SolarLab.AdvertBoard.Mobile.Contracts.Categories;
using SolarLab.AdvertBoard.Mobile.Presentation.Models.Categories;
using System.Text.Json;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public class CategoryApiClient(IHttpClientFactory factory) : ICategoryApiClient
    {
        private readonly HttpClient _httpClient = factory.CreateClient("BaseApi");
        public async Task<CategoryTreeResponse> GetCategoryTreeAsync() =>
            JsonSerializer.Deserialize<CategoryTreeResponse>(
                await _httpClient.GetStringAsync("api/categories/tree"), 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
