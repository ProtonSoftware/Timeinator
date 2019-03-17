using System;
using System.Collections.Generic;
using System.Linq;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Basic implementation of session handling Service used by <see cref="UserTimeHandler"/> which can be extended or replaced with platform-specific implementation
    /// </summary>
    public class DrySessionService : ISessionService
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksManager mTimeTasksManager;
        private readonly IUserTimeHandler mUserTimeHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DrySessionService()
        {
        }

        #endregion

        #region Interface Implementation
        #endregion

        #region Private Helpers
        #endregion
    }
}
