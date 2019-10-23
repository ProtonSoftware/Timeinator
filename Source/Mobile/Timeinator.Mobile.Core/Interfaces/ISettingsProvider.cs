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

        bool SessionWasFinishTime { get; set; }
        int TimerTickRate { get; set; }
        double MinimumTaskTime { get; set; }

        #endregion

        List<string> Languages { get; }
        string LanguageValue { get; set; }
        bool IsDarkModeOn { get; set; }
        bool HighestPrioritySetAsFirst { get; set; }
        bool RecalculateTasksAfterBreak { get; set; }

        void SetSetting(SettingsPropertyInfo setting);
    }
}
