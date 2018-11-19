﻿using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

namespace Timeinator.Mobile.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FormsApplicationPage
    {
        public MainWindow()
        {
            InitializeComponent();

            Forms.Init();
            LoadApplication(new Timeinator.Mobile.App());
        }
    }
}