using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    [QueryProperty(nameof(FirstName), "FirstName")]
    [QueryProperty(nameof(LastName), "LastName")]
    [QueryProperty(nameof(MiddleName), "MiddleName")]
    [QueryProperty(nameof(ContactEmail), "ContactEmail")]
    [QueryProperty(nameof(Phone), "Phone")]
    public partial class EditProfilePageModel : ObservableObject
    {
        [ObservableProperty] private string _firstName;
        [ObservableProperty] private string _lastName;
        [ObservableProperty] private string _middleName;
        [ObservableProperty] private string _contactEmail;
        [ObservableProperty] private string _phone;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditProfilePageModel()
        {
            SaveCommand = new AsyncRelayCommand(OnSaveAsync);
            CancelCommand = new AsyncRelayCommand(OnCancelAsync);
        }

        private Task OnSaveAsync() => Task.CompletedTask; // сюда будет сохранение
        private Task OnCancelAsync() => Shell.Current.GoToAsync("..");
    }

}
