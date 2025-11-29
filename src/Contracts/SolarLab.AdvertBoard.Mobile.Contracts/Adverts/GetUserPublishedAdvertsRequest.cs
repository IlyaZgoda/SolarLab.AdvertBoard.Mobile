namespace SolarLab.AdvertBoard.Mobile.Contracts.Adverts
{
    /// <summary>
    /// DTO для получения опубликованных объявлений текущего аутентифицированного пользователя.
    /// </summary>
    /// <param name="Page">Номер страницы.</param>
    /// <param name="PageSize">Размер страницы.</param>
    public record GetUserPublishedAdvertsRequest(int Page = 1, int PageSize = 20);
}
