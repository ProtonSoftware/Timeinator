
using Android.App;
using Android.OS;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using Timeinator.Mobile.Core;

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

            var tp = (TimePicker)FindViewById(Resource.Id.pickerSession);
            tp.SetIs24HourView((Java.Lang.Boolean)true);
            mOnTimeChanged = new TimeChanged();
            tp.SetOnTimeChangedListener(mOnTimeChanged);

            var set = this.CreateBindingSet<TasksTimePage, TasksTimePageViewModel>();
            set.Bind(mOnTimeChanged).For(t => t.SessionTime).To(vm => vm.UserTime);
            set.Apply();
        }

        private TimeChanged mOnTimeChanged;

        [PropertyChanged.AddINotifyPropertyChangedInterface]
        private class TimeChanged : Java.Lang.Object, TimePicker.IOnTimeChangedListener
        {
            public System.TimeSpan SessionTime { get; set; }

            public void OnTimeChanged(TimePicker view, int hour, int minute)
            {
                SessionTime = new System.TimeSpan(hour, minute, 0);
            }
        }
    }
}