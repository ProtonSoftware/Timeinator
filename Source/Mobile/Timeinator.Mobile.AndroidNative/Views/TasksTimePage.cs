using Android.App;
using Android.OS;
using Android.Widget;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Timeinator.Mobile.AndroidNative
{
    [MvxActivityPresentation]
    [Activity(Label = "View for TasksTimePageViewModel",
              NoHistory = true)]
    public class TasksTimePage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TasksTimePage);
        }
    }
}