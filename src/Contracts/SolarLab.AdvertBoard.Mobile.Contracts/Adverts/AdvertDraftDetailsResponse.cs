namespace SolarLab.AdvertBoard.Mobile.Contracts.Adverts
{
    /// <summary>
    /// DTO для получения детальной информации о черновике объявления.
    /// </summary>
    /// <param name="Id">Идентификатор объявления.</param>
    /// <param name="Title">Заголовок объявления.</param>
    /// <param name="Description">Описание объявления.</param>
    /// <param name="Price">Цена.</param>
    /// <param name="CategoryId">Идентификатор категории.</param>
    /// <param name="CategoryTitle">Название категории.</param>
    /// <param name="Status">Статус объявления.</param>
    /// <param name="CreatedAt">Дата и время создания.</param>
    /// <param name="UpdatedAt">Дата и время последнего обновления.</param>
    /// <param name="AuthorId">Идентификатор автора.</param>
    public record AdvertDraftDetailsResponse(
        Guid Id,
        string Title, 
        string Description, 
        decimal Price, 
        Guid CategoryId, 
        string CategoryTitle, 
        string Status, 
        DateTime CreatedAt, 
        DateTime? UpdatedAt, 
        Guid AuthorId,
        List<Guid> ImageIds);
}
