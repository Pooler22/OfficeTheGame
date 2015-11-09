using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace OfficeTheGame
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

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
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
                Frame.Navigate(typeof(MainPage), nameInput.Text);
            }
        }
    }
}
