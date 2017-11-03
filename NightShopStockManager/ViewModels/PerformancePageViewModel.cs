using OxyPlot;
using OxyPlot.Series;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class PerformancePageViewModel : BaseViewModel
    {
        #region Members

        private PlotModel _model;
        public PlotModel Model
        {
            get { return _model; }
            set { SetProperty(ref _model, value); }
        }

        private DateTime _startDate;
        public DateTime StartDate {
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

        private decimal _totalIncome;
        public decimal TotalIncome
        {
            get { return _totalIncome; }
            set { SetProperty(ref _totalIncome, value); }
        }

        private decimal _totalExpenses;
        public decimal TotalExpenses
        {
            get { return _totalExpenses; }
            set { SetProperty(ref _totalExpenses, value); }
        }

        private decimal _expiredExpenses;
        public decimal ExpiredExpenses
        {
            get { return _expiredExpenses; }
            set { SetProperty(ref _expiredExpenses, value); }
        }

        public decimal BuyExpenses { get { return TotalExpenses - ExpiredExpenses; } }

        public decimal Profit { get { return TotalIncome - TotalExpenses; } }

        #endregion

        #region Command ShowData

        private Command _showData;
        public Command ShowData
        {
            get { return _showData ?? (_showData = new Command(async () => await OnShowData())); }
        }

        private async Task OnShowData()
        {
            TotalIncome = await App.Database.GetIncome(StartDate, EndDate);
            TotalExpenses = await App.Database.GetExpenses(StartDate, EndDate);
            ExpiredExpenses = await App.Database.GetExpiredExpenses(StartDate, EndDate);
            Model = new PlotModel { Title = $"Revenue {StartDate.ToString("dd/MM/yy")}-{EndDate.ToString("dd/MM/yy")} : €{TotalIncome}" };
            var pieSeries = new PieSeries { StrokeThickness = 2.0, InsideLabelPosition = 0.8, AngleSpan = 360, StartAngle = 0,  };
            pieSeries.Slices.Add(new PieSlice("Expenses", Convert.ToDouble(BuyExpenses)));
            pieSeries.Slices.Add(new PieSlice("Expired", Convert.ToDouble(ExpiredExpenses)));
            pieSeries.Slices.Add(new PieSlice("Profit", Convert.ToDouble(Profit)));
            Model.Series.Add(pieSeries);
            RaisePropertyChanged("Profit");
        }

        #endregion

        #region Constructor 

        public PerformancePageViewModel(INavigationService navigationService) : base(navigationService) {
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            StartDate = EndDate.AddDays(-6);
        }

        #endregion
    }
}