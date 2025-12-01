using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Base;
using SolarLab.AdvertBoard.Mobile.Contracts.Images;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public interface IAdvertApiClient
    {
        Task<PaginationCollection<PublishedAdvertItem>> GetPublishedAsync(AdvertFilterRequest filter);
        Task<PublishedAdvertDetailsResponse> GetAdvertDetailsAsync(Guid id);
    }
}