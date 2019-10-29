using System;
using System.Collections.Generic;

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

        public static ICollection<string> SplitTagsString(this string tagsString) => string.IsNullOrWhiteSpace(tagsString) ? null : tagsString.Split(' ');
        public static string CreateTagsString(this ICollection<string> tagsList) => string.Join(" ", tagsList);
    }
}
