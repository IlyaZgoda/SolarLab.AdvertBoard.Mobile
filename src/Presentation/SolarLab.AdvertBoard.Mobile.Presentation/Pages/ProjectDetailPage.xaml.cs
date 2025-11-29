using SolarLab.AdvertBoard.Mobile.Presentation.Models;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages
{
    public partial class ProjectDetailPage : ContentPage
    {
        public ProjectDetailPage(ProjectDetailPageModel model)
        {
            InitializeComponent();

            BindingContext = model;
        }
    }
}
