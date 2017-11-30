using Prism.Navigation;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NightShopStockManager.ViewModels
{
    public class ReportsPageViewModel : BaseViewModel
    {
        #region Command PerformanceReview

        private Command _performanceReview;
        public Command PerformanceReview
        {
            get { return _performanceReview ?? (_performanceReview = new Command(async () => await OnPerformanceReview())); }
        }

        private async Task OnPerformanceReview()
        {
            await _navigationService.NavigateAsync("PerformancePage");
        }

        #endregion

        #region Command History

        private Command _history;
        public Command History
        {
            get { return _history ?? (_history = new Command(async () => await OnHistory())); }
        }

        private async Task OnHistory()
        {
            await _navigationService.NavigateAsync("HistoryPage");
        }

        #endregion

        #region Command Export Data

        private Command _exportData;
        public Command ExportData
        {
            get { return _exportData ?? (_exportData = new Command(async () => await OnExportData())); }
        }

        private async Task OnExportData()
        {
            var stringBuilder = new StringBuilder();
            var recordItems = await App.Database.GetRecordItemsAsync();
            stringBuilder.AppendLine("ItemId;Name;DateTime;Amount;SellPrice;TotalPrice");
            foreach (var itm in recordItems)
                stringBuilder.AppendLine($"{itm.Item};{itm.Name};{itm.DateTime.ToString("dd/MM/yyyy HH:mm:ss")};{itm.Amount};{itm.SellPrice};{itm.TotalPrice}");

#if UWP
            await DependencyService.Get<IFileHelper_WinApp>().SaveFileAsync($"export_sales_{DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")}.csv", stringBuilder.ToString());
#else
            DependencyService.Get<IFileHelper>().SaveFile($"export_sales_{DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")}.csv", stringBuilder.ToString());
#endif

            var stringBuilder2 = new StringBuilder();
            var buyRecords = await App.Database.GetBuyRecordsAsync();
            stringBuilder2.AppendLine("ItemId;Name;SupplierId;SupplierName;DateTime;Amount;BuyPrice/p.Item;TotalPrice");
            foreach (var itm in buyRecords)
                stringBuilder2.AppendLine($"{itm.Item};{itm.Name};{itm.Supplier};{itm.SupplierName};{itm.DateTime.ToString("dd/MM/yyyy HH:mm:ss")};{itm.Amount};{itm.BuyPrice};{itm.TotalPrice}");

#if UWP
            await DependencyService.Get<IFileHelper_WinApp>().SaveFileAsync($"export_buy_{DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")}.csv", stringBuilder2.ToString());
#else
            DependencyService.Get<IFileHelper>().SaveFile($"export_buy_{DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")}.csv", stringBuilder2.ToString());
#endif

            var stringBuilder3 = new StringBuilder();
            var throwAwayRecords = await App.Database.GetThrowAwayRecordAsync();
            stringBuilder3.AppendLine("ItemId;Name;DateTime;Amount;BuyPrice/p.Item;TotalPrice");
            foreach (var itm in throwAwayRecords)
                stringBuilder3.AppendLine($"{itm.Item};{itm.Name};{itm.DateTime.ToString("dd/MM/yyyy HH:mm:ss")};{itm.Amount};{itm.BuyPrice};{itm.TotalPrice}");

#if UWP
            await DependencyService.Get<IFileHelper_WinApp>().SaveFileAsync($"export_throwaway_{DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")}.csv", stringBuilder3.ToString());
#else
            DependencyService.Get<IFileHelper>().SaveFile($"export_throwaway_{DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")}.csv", stringBuilder3.ToString());
#endif
        }

        #endregion

        #region Constructors

        public ReportsPageViewModel(INavigationService navigationService) : base(navigationService) { }

        #endregion
    }
}