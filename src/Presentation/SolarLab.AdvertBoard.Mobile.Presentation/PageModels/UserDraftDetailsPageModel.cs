using CommunityToolkit.Mvvm.ComponentModel;
using SolarLab.AdvertBoard.Mobile.Contracts.Adverts;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public partial class UserDraftDetailsPageModel : ObservableObject, IQueryAttributable
    {
        private readonly IAdvertApiClient _client;
        private readonly IAuthService _auth;

        public UserDraftDetailsPageModel(IAdvertApiClient client, IAuthService auth)
        {
            _client = client;
            _auth = auth;
            AdvertImages = new ObservableCollection<Guid>();
        }

        [ObservableProperty]
        private AdvertDraftDetailsResponse? _advert;

        public ObservableCollection<Guid> AdvertImages { get; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("AdvertId", out var idObj) && Guid.TryParse(idObj?.ToString(), out var advertId))
            {
                _ = LoadDraftDetailsAsync(advertId);
            }
        }


        private async Task LoadDraftDetailsAsync(Guid advertId)
        {
            try
            {
                var response = await _client.GetDraftDetailsAsync(advertId, _auth.Jwt!);
                Advert = response;

                AdvertImages.Clear();
                if (response.ImageIds != null)
                {
                    foreach (var img in response.ImageIds)
                        AdvertImages.Add(img);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки черновика: {ex}");
            }
        }
    }
}
