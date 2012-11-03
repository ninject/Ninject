using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Xunit.Runner.Silverlight;

namespace Ninject.WindowsPhone7Tests
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            SystemTray.IsVisible = false;

            var testPage = new TestEngine(Assembly.GetExecutingAssembly());

            (Application.Current.RootVisual as PhoneApplicationFrame).Content = testPage;
        }

    }
}