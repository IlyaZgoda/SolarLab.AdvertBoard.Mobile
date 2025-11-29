namespace SolarLab.AdvertBoard.Mobile.Contracts.Comments
{
    /// <summary>
    /// DTO для запроса на получение комментариев.
    /// </summary>
    /// <param name="Page">Номер страницы.</param>
    /// <param name="PageSize">Количество элементов на странице.</param>
    public record GetCommentsByAdvertIdRequest(int Page = 1, int PageSize = 20);
}
