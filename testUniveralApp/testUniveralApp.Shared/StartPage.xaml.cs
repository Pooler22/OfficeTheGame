﻿using System;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
	public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

		private void Button_Click_Set_Name(object sender, RoutedEventArgs e)
		{
			if(nameInput.Text == "")
			{
				greetingOutput.Text = "You don't have name?";
			}
			else
			{
				greetingOutput.Text = "Hello, " + nameInput.Text + "!";
				this.Frame.Navigate(typeof(MainPage));
			}
		}
    }
}
