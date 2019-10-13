using Android.App;
using Android.OS;
using Android.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(Label = "View for AlarmPageViewModel")]
    public class AlarmPage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AlarmPage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            Window.AddFlags(
                WindowManagerFlags.ShowWhenLocked |
                WindowManagerFlags.DismissKeyguard |
                WindowManagerFlags.KeepScreenOn |
                WindowManagerFlags.TurnScreenOn |
                WindowManagerFlags.AllowLockWhileScreenOn
                );
        }
    }
}