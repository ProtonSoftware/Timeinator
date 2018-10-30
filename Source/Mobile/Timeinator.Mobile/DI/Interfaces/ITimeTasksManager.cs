using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile
{
    public interface ITimeTasksManager
    {
        List<TimeTaskContext> TaskContexts { get; set; }
        void AddTask(TimeTaskContext timeTask);
        void RemoveTask(TimeTaskContext timeTask);
        void UserReady(TimeSpan freetime);
        List<TimeTaskContext> CalcAssignedTimes(List<TimeTaskContext> target);
        void RefreshContexts();
        List<TimeTaskContext> GetImportant(List<TimeTaskContext> contexts, bool inverse = false);
        List<TimeTaskContext> GetEnabled(List<TimeTaskContext> contexts, bool inverse = false);
        List<TimeTaskContext> GetConstant(List<TimeTaskContext> contexts, bool inverse = false);
        List<TimeTaskContext> GetNotReady(List<TimeTaskContext> contexts, bool inverse = false);
        double GetRealPriority(TimeTaskContext tc);
    }
}
