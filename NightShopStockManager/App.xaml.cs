using NightShopStockManager.Views;
using Prism.Autofac;
using Prism.Autofac.Forms;
using Xamarin.Forms;

namespace NightShopStockManager
{
    public partial class App : PrismApplication
    {
        static ItemDatabase database;

        public App(IPlatformInitializer initializer = null) : base(initializer)
        { }

        protected override void OnInitialized()
        {
            InitializeComponent();
            NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<SummaryPage>();
            Container.RegisterTypeForNavigation<ItemManagementPage>();
            Container.RegisterTypeForNavigation<StockManagementPage>();
            Container.RegisterTypeForNavigation<HistoryPage>();
            Container.RegisterTypeForNavigation<BuyItemsPage>();
            Container.RegisterTypeForNavigation<SettingsPage>();
            Container.RegisterTypeForNavigation<SellItemsPage>();
            Container.RegisterTypeForNavigation<ReportsPage>();
            Container.RegisterTypeForNavigation<SupplierManagementPage>();
            Container.RegisterTypeForNavigation<SupplierPage>();
            Container.RegisterTypeForNavigation<ItemPage>();
            Container.RegisterTypeForNavigation<PerformancePage>();
            Container.RegisterTypeForNavigation<ManualEnteringPage>();
            Container.RegisterTypeForNavigation<StockItemPage>();
            Container.RegisterTypeForNavigation<BarcodePage>();
        }

        public static ItemDatabase Database
        {
            get
            {
                if (database == null)
                {
#if UWP
                    var path = DependencyService.Get<IFileHelper_WinApp>().GetLocalFilePath("NightShopStockManagerSQLite.db3");
#else
                    var path = DependencyService.Get<IFileHelper>().GetLocalFilePath("NightShopStockManagerSQLite.db3");
#endif
                    database = new ItemDatabase(path);
                }
                return database;
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}