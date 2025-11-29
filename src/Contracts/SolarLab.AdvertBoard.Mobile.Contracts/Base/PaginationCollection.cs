namespace SolarLab.AdvertBoard.Mobile.Contracts.Base
{
    /// <summary>
    /// DTO для пагинированной коллекции элементов.
    /// </summary>
    /// <typeparam name="TItem">Тип элементов в коллекции.</typeparam>
    /// <param name="Items">Коллекция элементов текущей страницы.</param>
    /// <param name="Page">Текущий номер страницы.</param>
    /// <param name="PageSize">Размер страницы.</param>
    /// <param name="TotalCount">Общее количество элементов.</param>
    /// <param name="TotalPages">Общее количество страниц.</param>
    public record PaginationCollection<TItem>(
        IReadOnlyCollection<TItem> Items, 
        int Page, 
        int PageSize, 
        int TotalCount, 
        int TotalPages);
}
