using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace testUniveralApp
{
	public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

		private void Button_Click_Set_Name(object sender, RoutedEventArgs e)
		{
			checkName();
		}

		private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				checkName();
			}
		}

		private void checkName()
		{
			if (nameInput.Text == "")
			{
				greetingOutput.Text = "You don't have name?";
			}
			else
			{
				greetingOutput.Text = "Hello, " + nameInput.Text + "!";
				this.Frame.Navigate(typeof(MainPage), nameInput.Text);
			}
		}
    }
}
