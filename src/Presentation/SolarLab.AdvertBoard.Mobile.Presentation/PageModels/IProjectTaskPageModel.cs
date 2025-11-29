using CommunityToolkit.Mvvm.Input;
using SolarLab.AdvertBoard.Mobile.Presentation.Models;

namespace SolarLab.AdvertBoard.Mobile.Presentation.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}