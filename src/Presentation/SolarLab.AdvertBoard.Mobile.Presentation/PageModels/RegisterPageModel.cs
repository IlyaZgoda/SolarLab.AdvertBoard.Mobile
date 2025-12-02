using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public partial class RegisterPageModel : ObservableObject
    {

        public IRelayCommand BackCommand { get; }

        public RegisterPageModel()
        {
            BackCommand = new RelayCommand(async () =>
            {
                // Переходим на главную страницу, очищая стек
                await Shell.Current.GoToAsync("///main");
            });
        }

    }
}
