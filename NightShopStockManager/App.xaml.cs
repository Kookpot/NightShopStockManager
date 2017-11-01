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
            Container.RegisterTypeForNavigation<ItemManagementPage>();
            Container.RegisterTypeForNavigation<StockManagementPage>();
            Container.RegisterTypeForNavigation<BuyItemsPage>();
            Container.RegisterTypeForNavigation<SellItemsPage>();
            Container.RegisterTypeForNavigation<SupplierManagementPage>();
            Container.RegisterTypeForNavigation<SupplierPage>();
            Container.RegisterTypeForNavigation<ItemPage>();
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
                    var path = DependencyService.Get<IFileHelper>().GetLocalFilePath("NightShopStockManagerSQLite.db3");
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