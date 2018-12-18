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
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // Add NotificationHandler implementation to DI
            Dna.Framework.Construction.Services.AddSingleton<INotificationHandler, NotificationHandler>();
            Dna.Framework.Construction.Build();

            // Read app start parameters and execute them (origin Notification)
            var aid = Intent.GetIntExtra("AID", -1);
            var nid = Intent.GetIntExtra("NID", -1);
            if (nid >= 0 && aid >= 0)
            {
                switch (aid)
                {
                    case (int)NotificationAction.GoToSession:
                        DI.Application.GoToPage(ApplicationPage.TasksSession);
                        break;
                }
            }
        }
    }
}