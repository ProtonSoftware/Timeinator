using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The interface for setting provider to manage all the settings in this app
    /// </summary>
    public interface ISettingsProvider
    {
        bool SessionTimeAsFinishTime { get; }
        int TimerTickRate { get; }
        double MinimumTaskTime { get; }

        List<string> Languages { get; }
        string LanguageValue { get; }
        bool IsDarkModeOn { get; }
        bool HighestPrioritySetAsFirst { get; }
        bool RecalculateTasksAfterBreak { get; }

        void SetSetting(SettingsPropertyInfo setting, bool shouldSaveToDatabase = true);
    }
}
