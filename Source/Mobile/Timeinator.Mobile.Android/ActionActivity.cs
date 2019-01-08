﻿using System;
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
    [Activity(Label = "Timeinator")]
    public class ActionActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Read intent parameters and execute them
            if (Intent.Action == IntentActions.ACTION_GOSESSION)
            {
                var intent = new Intent(Application.Context, typeof(MainActivity));
                intent.SetAction(IntentActions.ACTION_GOSESSION);
                StartActivity(intent);
            }
            else if (Intent.Action == IntentActions.ACTION_SHOW)
            {
                var intent = new Intent(Application.Context, typeof(MainActivity));
                StartActivity(intent);
            }
            else if (Intent.Action == IntentActions.ACTION_NEXTTASK)
            {
                // Controlled by Service, but can be implemented here as well
                // could pass intent to Service
            }
            else if (Intent.Action == IntentActions.ACTION_PAUSETASK)
            {
                // Controlled by Service, but can be implemented here as well
            }
            else if (Intent.Action == IntentActions.ACTION_RESUMETASK)
            {
                // Controlled by Service, but can be implemented here as well
            }
        }
    }
}