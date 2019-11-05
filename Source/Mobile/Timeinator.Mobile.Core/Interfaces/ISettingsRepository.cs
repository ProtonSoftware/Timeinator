using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The interface for repository that handles application's settings
    /// </summary>
    public interface ISettingsRepository : IRepository<Setting, int>
    {
        IEnumerable<SettingsPropertyInfo> GetAllSettings();
        void SaveSetting(SettingsPropertyInfo setting);
    }
}
