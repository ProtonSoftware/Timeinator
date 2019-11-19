using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The actions that can be used for Android intents to perform certain tasks
    /// </summary>
    public static class IntentActions
    {
        public const string ACTION_NOTHING = "com.gummybearstudio.NOTHING";
        public const string ACTION_TIMEOUT = "com.gummybearstudio.TIMEOUT";
        public const string ACTION_SHOW = "com.gummybearstudio.SHOW";
        public const string ACTION_GOSESSION = "com.gummybearstudio.GOSESSION";
        public const string ACTION_NEXTTASK = "com.gummybearstudio.NEXTTASK";
        public const string ACTION_PAUSETASK = "com.gummybearstudio.PAUSETASK";
        public const string ACTION_RESUMETASK = "com.gummybearstudio.RESUMETASK";
        public const string ACTION_STOP = "com.gummybearstudio.STOP";

        /// <summary>
        /// Returns certain action intent string based on provided enum
        /// </summary>
        /// <param name="action">The enum to base intent on</param>
        public static string ToIntentString(this AppAction action)
        {
            switch (action)
            {
                case AppAction.DoNothing:
                    return ACTION_SHOW;

                case AppAction.TimeOut:
                    return ACTION_TIMEOUT;

                case AppAction.GoToSession:
                    return ACTION_GOSESSION;

                case AppAction.NextSessionTask:
                    return ACTION_NEXTTASK;

                case AppAction.PauseSession:
                    return ACTION_PAUSETASK;

                case AppAction.ResumeSession:
                    return ACTION_RESUMETASK;

                case AppAction.StopSession:
                    return ACTION_STOP;

                default:
                    return ACTION_NOTHING;
            }
        }

        /// <summary>
        /// Returns app action enum based on provided string intent
        /// </summary>
        /// <param name="intentString">The intent string to convert</param>
        public static AppAction ToActionEnum(this string intentString)
        {
            switch (intentString)
            {
                case ACTION_SHOW:
                    return AppAction.DoNothing;

                case ACTION_TIMEOUT:
                    return AppAction.TimeOut;

                case ACTION_GOSESSION:
                    return AppAction.GoToSession;

                case ACTION_NEXTTASK:
                    return AppAction.NextSessionTask;

                case ACTION_PAUSETASK:
                    return AppAction.PauseSession;

                case ACTION_RESUMETASK:
                    return AppAction.ResumeSession;

                case ACTION_STOP:
                    return AppAction.StopSession;
                    
                default:
                    return default;
            }
        }
    }
}