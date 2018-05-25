using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SuperGameSystemBasic
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private readonly BasicOne _game;

        public GamePage()
        {
            InitializeComponent();
            // Create the game.
            var launchArguments = string.Empty;
            _game = XamlGame<BasicOne>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
        }
    }
}