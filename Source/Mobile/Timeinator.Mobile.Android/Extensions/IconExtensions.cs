using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The extensions for everything that can be converted to icons
    /// </summary>
    public static class IconExtensions
    {
        /// <summary>
        /// Converts task's type to Android icon
        /// </summary>
        /// <param name="taskType">The task's type to get icon for</param>
        /// <returns>Icon as Android Resource Id</returns>
        public static int ToIcon(this TimeTaskType taskType)
        {
            // Return associated icon based on provided type
            switch (taskType)
            {
                case TimeTaskType.Generic:
                    return Resource.Drawable.icon_type_generic;

                case TimeTaskType.Reading:
                    return Resource.Drawable.icon_type_reading;

                default:
                    return -1;
            }
        }

        /// <summary>
        /// Converts task's priority to Android icon
        /// </summary>
        /// <param name="taskType">The task's priority to get icon for</param>
        /// <returns>Icon as Android Resource Id</returns>
        public static int ToIcon(this Priority taskType)
        {
            // Return associated icon based on provided priority value
            switch (taskType)
            {
                case Priority.One:
                    return Resource.Drawable.icon_priority_one;

                case Priority.Two:
                    return Resource.Drawable.icon_priority_two;

                case Priority.Three:
                    return Resource.Drawable.icon_priority_three;

                case Priority.Four:
                    return Resource.Drawable.icon_priority_four;

                case Priority.Five:
                    return Resource.Drawable.icon_priority_five;

                default:
                    return -1;
            }
        }
    }
}