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
            UrlData.SaveUrl(UserTokenTextBox.Text, UrlTextBox.Text);
        }
    }
}
