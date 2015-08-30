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
    public sealed partial class SelectPage : Page
    {
		public string name { get; set; }

        public SelectPage()
        {
            this.InitializeComponent();
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			name = e.Parameter as string;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.GoBack();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(PlayPage), 's' + name);
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(PlayPage), 'c' + name);
		}
    }
}
