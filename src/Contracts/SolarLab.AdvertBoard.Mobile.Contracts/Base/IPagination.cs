namespace SolarLab.AdvertBoard.Mobile.Contracts.Base
{
    /// <summary>
    /// Интерфейс для стандартизации параметров пагинации в запросах.
    /// </summary>
    public interface IPagination
    {
        /// <summary>
        /// Номер страницы (начинается с 1).
        /// </summary>
        int Page { get; init; }

        /// <summary>
        /// Размер страницы (количество элементов на странице).
        /// </summary>
        int PageSize { get; init; }
    }
}
