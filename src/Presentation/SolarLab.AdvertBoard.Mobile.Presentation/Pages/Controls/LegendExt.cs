using Syncfusion.Maui.Toolkit.Charts;

namespace SolarLab.AdvertBoard.Mobile.Presentation.Pages.Controls
{
    public class LegendExt : ChartLegend
    {
        protected override double GetMaximumSizeCoefficient()
        {
            return 0.5;
        }
    }
}
