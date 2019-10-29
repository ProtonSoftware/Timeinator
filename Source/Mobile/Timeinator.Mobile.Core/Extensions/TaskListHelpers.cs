using System;
using System.Collections.Generic;
using System.Linq;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// TODO: Get rid of this class, this stuff is not needed here, so this shouldnt exist
    /// </summary>
    public static class TaskListHelpers
    {
        public static event Action RefreshUITasks;

        public static void CleanRefreshEvent()
        {
            RefreshUITasks = () => { };
        }
        public static void RaiseRefreshEvent() => RefreshUITasks.Invoke();

        public static List<string> SplitTagsString(this string tagsString) => string.IsNullOrWhiteSpace(tagsString) ? null : tagsString.Split(' ').ToList();
        public static string CreateTagsString(this List<string> tagsList) => string.Join(" ", tagsList);
    }
}
