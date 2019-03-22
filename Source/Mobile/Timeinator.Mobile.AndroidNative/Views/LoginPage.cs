
using Android.App;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.AndroidNative
{
    [MvxActivityPresentation]
    [Activity(Label = "View for LoginPageViewModel")]
    public class LoginPage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LoginPage);

            // Add Android-specific dependency injection implementations
            Dna.Framework.Construction.Services.AddSingleton<IUIManager, UIManager>();
            Dna.Framework.Construction.Build();
        }
    }
}