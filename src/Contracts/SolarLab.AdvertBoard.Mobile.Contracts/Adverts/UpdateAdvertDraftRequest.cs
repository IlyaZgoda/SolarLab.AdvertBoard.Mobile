namespace SolarLab.AdvertBoard.Mobile.Contracts.Adverts
{
    /// <summary>
    /// DTO для запроса на обновление черновика объявления.
    /// </summary>
    /// <param name="CategoryId">Новый идентификатор категории (опционально).</param>
    /// <param name="Title">Новый заголовок (опционально).</param>
    /// <param name="Description">Новое описание (опционально).</param>
    /// <param name="Price">Новая цена (опционально).</param>
    public record UpdateAdvertDraftRequest(Guid? CategoryId, string? Title, string? Description, decimal? Price);
}
