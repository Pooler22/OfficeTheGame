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

namespace OfficeTheGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OptionsPage : Page
    {
        public OptionsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infoTextBlock.Text = e.Parameter as string;
        }

        private void Button_Click_Back_To_MainPage(object sender, RoutedEventArgs e)
        {
            if (infoTextBlock.Text != "Wrong name!")
            {
                this.Frame.Navigate(typeof(MainPage), infoTextBlock.Text);
            }
        }

        private void checkName()
        {
            if (changeNameTextBox.Text == "")
            {
                infoTextBlock.Text = "Wrong name!";
            }
            else
            {
                infoTextBlock.Text = changeNameTextBox.Text;
            }
        }

        private void changeNameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                checkName();
            }
        }

        private void changeNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkName();
        }
    }
}