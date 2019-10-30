namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The extensions for time task types
    /// The configuration of how every type should behave is there
    /// </summary>
    public static class TimeTaskTypeExtensions
    {
        public static double ConvertBasedOnType(this double maxProgress, TimeTaskType type)
        {
            switch (type)
            {
                case TimeTaskType.Generic:
                    return 100;

                case TimeTaskType.Reading:
                    return maxProgress;

                default:
                    return 100;
            }
        }
    }
}
