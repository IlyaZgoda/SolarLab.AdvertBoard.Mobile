using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Contracts.Base;
using SolarLab.AdvertBoard.Mobile.Contracts.Images;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public interface IAdvertApiClient
    {
        Task<PaginationCollection<PublishedAdvertItem>> GetPublishedAsync(AdvertFilterRequest filter);
        Task<PublishedAdvertDetailsResponse> GetAdvertDetailsAsync(Guid id);
        Task<PaginationCollection<PublishedAdvertItem>> GetUserPublishedAdvertsAsync(GetUserPublishedAdvertsRequest request, string jwt);
        Task<PaginationCollection<AdvertDraftItem>> GetUserDraftsAsync(GetUserAdvertDraftsRequest request, string jwt);
        Task<AdvertDraftDetailsResponse> GetDraftDetailsAsync(Guid id, string jwt);
        Task UpdateDraftAsync(Guid advertId, UpdateAdvertDraftRequest request, string jwt);
    }
}