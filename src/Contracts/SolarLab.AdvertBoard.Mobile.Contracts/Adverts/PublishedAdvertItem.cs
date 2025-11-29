namespace SolarLab.AdvertBoard.Mobile.Contracts.Adverts
{
    /// <summary>
    /// DTO для элемента списка опубликованных объявлений.
    /// </summary>
    /// <param name="Id">Идентификатор объявления.</param>
    /// <param name="Title">Заголовок объявления.</param>
    /// <param name="Description">Описание объявления.</param>
    /// <param name="Price">Цена.</param>
    /// <param name="CategoryId">Идентификатор категории.</param>
    /// <param name="CategoryName">Название категории.</param>
    /// <param name="AuthorId">Идентификатор автора.</param>
    /// <param name="AuthorName">Имя автора.</param>
    /// <param name="PublishedAt">Дата и время публикации.</param>
    public record PublishedAdvertItem(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        Guid CategoryId,
        string CategoryName,
        Guid AuthorId,
        string AuthorName,
        DateTime? PublishedAt,
        int CommentsCount);
}
