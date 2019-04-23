using NightShopStockManager.Views;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using Xamarin.Forms;

namespace NightShopStockManager
{
    public partial class App : PrismApplication
    {
        static ItemDatabase database;

        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();
            NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<ItemManagementPage>();
            containerRegistry.RegisterForNavigation<StockManagementPage>();
            containerRegistry.RegisterForNavigation<BuyItemsPage>();
            containerRegistry.RegisterForNavigation<ItemPage>();
            containerRegistry.RegisterForNavigation<ManualEnteringPage>();
            containerRegistry.RegisterForNavigation<StockItemPage>();
            containerRegistry.RegisterForNavigation<BarcodePage>();
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

        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}