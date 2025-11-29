using SolarLab.AdvertBoard.Mobile.Contracts.Users;

namespace SolarLab.AdvertBoard.Mobile.Contracts.Adverts
{
    /// <summary>
    /// DTO для получения детальной информации об опубликованном объявлении.
    /// </summary>
    /// <param name="Id">Идентификатор объявления.</param>
    /// <param name="Title">Заголовок объявления.</param>
    /// <param name="Description">Описание объявления.</param>
    /// <param name="Price">Цена.</param>
    /// <param name="CategoryId">Идентификатор категории.</param>
    /// <param name="CategoryTitle">Название категории.</param>
    /// <param name="PublishedAt">Дата и время публикации.</param>
    /// <param name="AuthorId">Идентификатор автора.</param>
    /// <param name="AuthorContacts">Контактная информация автора.</param>
    public record PublishedAdvertDetailsResponse(
        Guid Id,
        string Title, 
        string Description, 
        decimal Price, 
        Guid CategoryId, 
        string CategoryTitle, 
        DateTime PublishedAt, 
        Guid AuthorId, 
        UserContactInfoResponse AuthorContacts,
        List<Guid> ImageIds,
        int CommentsCount);
}
