﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile
{
    public interface ITimeTasksManager
    {
        void UploadTasksList(List<TimeTaskContext> contexts, TimeSpan userTime);

        List<TimeTaskContext> GetCalculatedTasksListForSpecifiedTime();
    }
}
