using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Bastille
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private UrlData UrlData;

        public MainPage()
        {
            InitializeComponent();
            UrlData = new UrlData();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UrlData.SaveUrl(UserTokenTextBox.Text, UrlTextBox.Text);
            }
            catch (Exception exception)
            {
                OutputTextBox.Text = exception.Message;
            }
        }

        private void UnitTestButton_Click(object sender, RoutedEventArgs e)
        {
            var urlDataUnitTests = new UrlDataUnitTests();
            var success = true;

            try
            {
                urlDataUnitTests.RunAllUnitTests();
            }
            catch (Exception exception)
            {
                OutputTextBox.Text = exception.Message;
                success = false;
            }

            if (success)
            {
                OutputTextBox.Text = "All unit tests passed.";
            }
        }
    }
}
