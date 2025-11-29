using SolarLab.AdvertBoard.Mobile.Contracts.Base;

namespace SolarLab.AdvertBoard.Mobile.Contracts.Adverts
{
    /// <summary>
    /// DTO для запроса фильтрации объявлений с поддержкой пагинации.
    /// </summary>
    /// <remarks>
    /// Реализует интерфейс IPagination для стандартизации пагинации.
    /// Поддерживает фильтрацию по категории, автору, цене и текстовый поиск.
    /// </remarks>
    public record AdvertFilterRequest : IPagination
    {
        /// <summary>
        /// Номер страницы (начинается с 1).
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Размер страницы (количество элементов на странице).
        /// </summary>
        public int PageSize { get; init; } = 20;

        /// <summary>
        /// Поле для сортировки (price, title, createdat).
        /// </summary>
        public string? SortBy { get; init; }

        /// <summary>
        /// Направление сортировки (true - по убыванию, false - по возрастанию).
        /// </summary>
        public bool SortDescending { get; init; } = true;

        /// <summary>
        /// Идентификатор категории для фильтрации.
        /// </summary>
        public Guid? CategoryId { get; init; }

        /// <summary>
        /// Идентификатор автора для фильтрации.
        /// </summary>
        public Guid? AuthorId { get; init; }

        /// <summary>
        /// Минимальная цена для фильтрации по диапазону.
        /// </summary>
        public decimal? MinPrice { get; init; }

        /// <summary>
        /// Максимальная цена для фильтрации по диапазону.
        /// </summary>
        public decimal? MaxPrice { get; init; }

        /// <summary>
        /// Текст для поиска в заголовке и описании объявлений.
        /// </summary>
        public string? SearchText { get; init; }
    }
}
