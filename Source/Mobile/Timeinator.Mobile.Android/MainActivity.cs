using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Timeinator.Mobile.Droid
{
    [Activity(Label = "Timeinator", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Exit activity since no action was requested
            if (Intent.Action == IntentActions.ACTION_NOTHING)
                return;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // If there is no DI setup yet, i.e. NotificationHandler is not injected
            if (!Dna.Framework.Construction.Services.Any(x => x.ServiceType == typeof(IUserTimeHandler) && x.ImplementationType == typeof(AndroidTimeHandler)))
            {
                // Add Android-specific DI implementations
                Dna.Framework.Construction.Services.AddScoped<IUserTimeHandler, AndroidTimeHandler>();

                // Build new DI
                Dna.Framework.Construction.Build();
            }

            // If we get there by session intent from notification
            if (Intent.Action == IntentActions.ACTION_GOSESSION)
            {
                // Change application's page to continue session
                DI.Application.GoToPage(ApplicationPage.TasksSession);
            }
        }
    }
}