using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Microsoft.Extensions.DependencyInjection;

namespace Timeinator.Mobile.Droid
{
    [Activity(Label = "Timeinator", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Intent.Action == IntentActions.ACTION_NOTHING)
                return; // exit activity since no action was requested

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            // Is this correct if MainActivity is launched again over the old one???
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // Add NotificationHandler implementation to DI
            Dna.Framework.Construction.Services.AddSingleton<INotificationHandler, NotificationHandler>();
            // replace existing UserTimeHandler with Android specific version of it
            Dna.Framework.Construction.Services.AddScoped<IUserTimeHandler, AndroidTimeHandler>();
            Dna.Framework.Construction.Build();

            // Read intent parameters and execute them
            if (Intent.Action == IntentActions.ACTION_GOSESSION)
            {
                // Make sure that app correctly loads all data
                DI.Application.GoToPage(ApplicationPage.TasksSession);
            }
        }
    }
}