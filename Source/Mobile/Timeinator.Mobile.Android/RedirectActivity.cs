using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Timeinator.Mobile.Droid
{
    [Activity(Label = "Redirecting")]
    public class RedirectActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            DI.Application.GoToPage(ApplicationPage.TasksSession);
            //base.OnCreate(savedInstanceState);
        }
    }
}