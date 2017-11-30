using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class HistoryPageViewModel : BaseViewModel
    {
        #region Members

        private PlotModel _model;
        public PlotModel Model
        {
            get { return _model; }
            set { SetProperty(ref _model, value); }
        }

        private Item _item;
        public Item Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }
        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }
        public DateTime CurrentDate { get { return DateTime.Now; } }

        #endregion

        #region Command ShowData

        private Command _showData;
        public Command ShowData
        {
            get { return _showData ?? (_showData = new Command(async () => await OnShowData())); }
        }

        private async Task OnShowData()
        {
            if (Item != null)
            {
                var sold = await App.Database.GetSoldDataAsync(Item, StartDate, EndDate);
                var total = sold.Values.Sum();
                var plot = new PlotModel { Title = $"{total} {Item.Name} Sold between {StartDate.ToString("dd/MM/yy")}-{EndDate.ToString("dd/MM/yy")} per day", TitleFontSize=8 };
                var lineSeries = new LineSeries { StrokeThickness = 2.0 };
                plot.Axes.Add(new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = DateTimeAxis.ToDouble(StartDate),
                    Maximum = DateTimeAxis.ToDouble(EndDate),
                    IntervalType = DateTimeIntervalType.Days,
                    StringFormat = "d/M"
                });
                foreach (var soldItem in sold)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(soldItem.Key), soldItem.Value));
                }
                plot.Series.Add(lineSeries);
                Model = plot;
            }
        }

        #endregion

        #region Item Search

        private Command _itemSearch;
        public Command ItemSearch
        {
            get { return _itemSearch ?? (_itemSearch = new Command(async () => await OnItemSearch())); }
        }

        private async Task OnItemSearch()
        {
            var parameters = new NavigationParameters
            {
                { "Select", true }
            };
            await _navigationService.NavigateAsync("ItemManagementPage", parameters);
        }

        #endregion

        #region Constructor

        public HistoryPageViewModel(INavigationService navigationService) : base(navigationService) {
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            StartDate = EndDate.AddDays(-6);
        }

        #endregion        

        #region Methods

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("Item"))
            {
                Item = (Item)parameters["Item"];
            }
        }

        #endregion
    }
}