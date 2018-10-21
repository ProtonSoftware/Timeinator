using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile
{
    public interface ITimeTasksManager
    {
        void AddTask(TimeTaskContext timeTask);
        void RemoveTask(TimeTaskContext timeTask);
        void UserReady(TimeSpan freetime);
        void UpdateTaskList(List<TimeTaskContext> timecontexts);
        void CalcAssignedTimes(List<TimeTaskContext> target);
        void RefreshContexts();
        List<TimeTaskContext> GetImportant(List<TimeTaskContext> contexts);
        List<TimeTaskContext> GetEnabled(List<TimeTaskContext> contexts);
        double GetRealPriority(TimeTaskContext tc);
    }
}
