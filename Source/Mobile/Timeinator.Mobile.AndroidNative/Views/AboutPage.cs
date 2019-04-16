﻿using Android.App;
using Android.OS;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(Label = "View for AboutPageViewModel")]
    public class AboutPage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AboutPage);
        }
    }
}