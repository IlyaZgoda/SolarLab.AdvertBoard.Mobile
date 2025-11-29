namespace SolarLab.AdvertBoard.Mobile.Contracts.Images
{
    /// <summary>
    /// DTO для получения информации об изображении.
    /// </summary>
    /// <param name="Content">Бинарное сожержание изображения.</param>
    /// <param name="ContentType">MIME-тип содержимого объявления.</param>
    /// <param name="FileName">Имя файла.</param>
    public record ImageResponse(byte[] Content, string ContentType, string FileName);
}
