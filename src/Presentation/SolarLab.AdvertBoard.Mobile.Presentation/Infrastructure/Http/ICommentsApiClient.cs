using SolarLab.AdvertBoard.Mobile.Contracts.Base;
using SolarLab.AdvertBoard.Mobile.Contracts.Comments;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http
{
    public interface ICommentsApiClient
    {
        Task<PaginationCollection<CommentItem>> GetCommentsByAdvertIdAsync(Guid advertId, GetCommentsByAdvertIdRequest request);
    }
}