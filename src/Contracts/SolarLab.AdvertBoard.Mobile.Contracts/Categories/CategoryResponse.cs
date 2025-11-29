namespace SolarLab.AdvertBoard.Mobile.Contracts.Categories
{
    /// <summary>
    /// DTO для ответа с информацией о категории.
    /// </summary>
    /// <param name="Id">Идентификатор категории.</param>
    /// <param name="ParentId">Идентификатор родительской категории (null для корневых).</param>
    /// <param name="Title">Название категории.</param>
    public record CategoryResponse(Guid Id, Guid? ParentId, string Title);
}
