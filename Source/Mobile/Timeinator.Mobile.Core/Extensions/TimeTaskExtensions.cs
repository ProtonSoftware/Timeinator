using System.Collections.Generic;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The extensions for time task related stuff
    /// </summary>
    public static class TimeTaskExtensions
    {
        /// <summary>
        /// Converts max progress to proper value based on provided task's type
        /// </summary>
        /// <param name="maxProgress">The initial max progress provided by user</param>
        /// <param name="type">The task's type</param>
        /// <returns>Correct max progress value for specified type</returns>
        public static double ConvertBasedOnType(this double maxProgress, TimeTaskType type)
        {
            // Based on type...
            switch (type)
            {
                case TimeTaskType.Generic:
                    // Max progress is 100%
                    return 100;

                case TimeTaskType.Reading:
                    // Max progress is provided amount of pages
                    return maxProgress;

                default:
                    // Max progress is 100%
                    return 100;
            }
        }

        /// <summary>
        /// Splits provided tags string into collection of single tags
        /// </summary>
        /// <param name="tagsString">The tag string to split</param>
        /// <returns>Collection of tags as strings</returns>
        public static ICollection<string> SplitTagsString(this string tagsString) 
            => string.IsNullOrWhiteSpace(tagsString) ? null : tagsString.Split(' ');

        /// <summary>
        /// Creates tags string out of provided collection of tags
        /// </summary>
        /// <param name="tagsList">The collection of tags as strings</param>
        /// <returns>The tag string containing all the tags</returns>
        public static string CreateTagsString(this ICollection<string> tagsList) 
            => string.Join(" ", tagsList);
    }
}
