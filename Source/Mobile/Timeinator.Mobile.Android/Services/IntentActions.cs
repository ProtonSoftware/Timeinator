﻿namespace Timeinator.Mobile.Droid
{
    public class IntentActions
    {
        public const string ACTION_NOTHING = "com.gummybearstudio.NOTHING";
        public const string ACTION_SHOW = "com.gummybearstudio.SHOW";
        public const string ACTION_GOSESSION = "com.gummybearstudio.GOSESSION";
        public const string ACTION_NEXTTASK = "com.gummybearstudio.NEXTTASK";
        public const string ACTION_PAUSETASK = "com.gummybearstudio.PAUSETASK";
        public const string ACTION_RESUMETASK = "com.gummybearstudio.RESUMETASK";
        public const string ACTION_STOP = "com.gummybearstudio.STOP";

        public static string FromEnum(AppAction e)
        {
            switch (e)
            {
                case AppAction.DoNothing:
                    return ACTION_SHOW;
                case AppAction.GoToSession:
                    return ACTION_GOSESSION;
                case AppAction.NextSessionTask:
                    return ACTION_NEXTTASK;
                case AppAction.PauseSession:
                    return ACTION_PAUSETASK;
                case AppAction.ResumeSession:
                    return ACTION_RESUMETASK;
                case AppAction.Stop:
                    return ACTION_STOP;
            }
            return ACTION_NOTHING;
        }
    }
}