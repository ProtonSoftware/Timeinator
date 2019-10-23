using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// Interface for managing app settings at runtime
    /// </summary>
    public interface ISettingsProvider
    {
        #region Config

        bool SessionWasFinishTime { get; }
        int TimerTickRate { get; }
        double MinimumTaskTime { get; }

        #endregion

        List<string> Languages { get; }
        string LanguageValue { get; set; }
        bool IsDarkModeOn { get; set; }
        bool HighestPrioritySetAsFirst { get; set; }
        bool RecalculateTasksAfterBreak { get; set; }

        void SaveSetting(SettingsPropertyInfo setting);
    }
}
