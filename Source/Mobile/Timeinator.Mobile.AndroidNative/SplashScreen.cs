
using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Core;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.AndroidNative
{
    [Activity(
        Label = "$rootnamespace$"
        , MainLauncher = true
        , NoHistory = true
        , Theme = "@style/AppTheme"
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenAppCompatActivity<MvxAppCompatSetup<App>, App>
    {
        public SplashScreen()
             : base(Resource.Layout.SplashScreen)
        {
        }
    }
}