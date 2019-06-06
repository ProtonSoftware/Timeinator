using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using MvvmCross.Droid.Support.V7.AppCompat;
using System.Linq;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    [Activity(
        Label = "Timeinator", 
        MainLauncher = true,
        NoHistory = true,
        Theme = "@style/AppTheme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenAppCompatActivity<MvxAppCompatSetup<App>, App>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SplashScreen()
        {
            // If there is no DI setup yet, i.e. IUIManager is not injected
            if (!Dna.Framework.Construction.Services.Any(x => x.ServiceType == typeof(IUIManager) && x.ImplementationType == typeof(UIManager)))
            {
                // Add Android-specific dependency injection implementations
                Dna.Framework.Construction.Services.AddSingleton<IUIManager, UIManager>();
                Dna.Framework.Construction.Services.AddSingleton<ISessionNotificationService, SessionNotificationService>();

                // Build new DI
                Dna.Framework.Construction.Build();
            }
        }

        #endregion
    }
}