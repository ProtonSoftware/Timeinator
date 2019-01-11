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
    public class TaskServiceBinder : Binder
    {
        public TaskServiceBinder(TaskService service)
        {
            Service = service;
        }

        public TaskService Service { get; private set; }
    }
}