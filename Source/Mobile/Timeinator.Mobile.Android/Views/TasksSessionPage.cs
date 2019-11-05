using Android.App;
using Android.Content;
using Android.OS;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(NoHistory = true)]
    public class TasksSessionPage : MvxAppCompatActivity
    {
        #region Private Members

        /// <summary>
        /// The android wake lock feature that allows to keep CPU running in the background
        /// </summary>
        private PowerManager.WakeLock mWakeLock;

        #endregion

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TasksSessionPage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Get android power manager
            var powerManager = Application.Context.GetSystemService(Context.PowerService) as PowerManager;

            // Setup new wake lock that allows to keep CPU running in the background
            mWakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "WakeLockTag");
        }

        protected override void OnStop()
        {
            base.OnStop();

            // Use prepared wake lock to prevent CPU pause when app is closed
            mWakeLock.Acquire();
        }
    }
}