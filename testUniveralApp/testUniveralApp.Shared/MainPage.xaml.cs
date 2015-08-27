using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace testUniveralApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

		private void Button_Click_Start(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(PlayPage));
		}

		private void Button_Click_Options(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(OptionsPage));
		}

		private void Button_Click_Exit(object sender, RoutedEventArgs e)
		{
			Application.Current.Exit();
		}
    }
}
