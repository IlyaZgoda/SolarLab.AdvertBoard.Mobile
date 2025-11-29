namespace SolarLab.AdvertBoard.Mobile.Contracts.Adverts
{
    /// <summary>
    /// DTO для запроса на создание черновика объявления.
    /// </summary>
    /// <param name="CategoryId">Идентификатор категории.</param>
    /// <param name="Title">Заголовок объявления.</param>
    /// <param name="Description">Описание объявления.</param>
    /// <param name="Price">Цена.</param>
    public record CreateAdvertDraftRequest(Guid CategoryId, string Title, string Description, decimal Price);
}
