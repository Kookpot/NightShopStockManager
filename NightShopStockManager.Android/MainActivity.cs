﻿using Android.App;
using Android.OS;
using Android.Content.PM;

namespace NightShopStockManager
{
	[Activity(Label = "NightShopStockManager", Icon = "@drawable/icon", MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity :
	global::Xamarin.Forms.Platform.Android.FormsApplicationActivity // superclass new in 1.3
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            ZXing.Net.Mobile.Forms.Android.ZXingScannerViewRenderer.Init();

            LoadApplication(new App());
		}
	}
}