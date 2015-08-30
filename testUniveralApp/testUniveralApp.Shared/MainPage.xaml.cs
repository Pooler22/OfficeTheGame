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

namespace testUniveralApp
{
    public sealed partial class MainPage : Page
    {
		string name = System.String.Empty;
		public MainPage()
        {
            this.InitializeComponent();
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.name = e.Parameter as string;
			nameTextBlock.Text = "Welcome " + this.name;
		}

		private void Button_Click_Start(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(SelectPage), name);
		}

		private void Button_Click_Options(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(OptionsPage), name);
		}

		private void Button_Click_Exit(object sender, RoutedEventArgs e)
		{
			Application.Current.Exit();
		}
    }
}
