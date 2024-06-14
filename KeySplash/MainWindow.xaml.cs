using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KeySplash.SplashScreens;

namespace KeySplash;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string ResourceLocation = "/resources/bongo_idle.jpg";
    private bool _isStarted;
    private bool _isTimerReset;
    private CustomSplashScreen _currentCustomSplashScreen;
    private bool _isKeyDown;

    public MainWindow()
    {
        InitializeComponent();
    }
    
    public void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (_isKeyDown || !_isStarted) return;
        _isKeyDown = true;
        if (Application.Current.Windows.OfType<CustomSplashScreen>().FirstOrDefault() is CustomSplashScreen
            OpenSplashScreen)
        {
            OpenSplashScreen.ResetClosing();
            if(OpenSplashScreen is BongoCat bongoCat) bongoCat.Tap();
            return;
        }
        _currentCustomSplashScreen = ShowBongo();
        HideSplash(_currentCustomSplashScreen);
    }

    private CustomSplashScreen ShowSplash(string resource)
    {
        CustomSplashScreen splash = new CustomSplashScreen(this,resource);
        splash.Show();
        return splash;
    }
    private CustomSplashScreen ShowBongo()
    {
        CustomSplashScreen splash = new BongoCat(this);
        splash.Show();
        return splash;
    }
    private void HideSplash(CustomSplashScreen? splash, bool isInstant = false)
    {
        if (splash is null) return;
        if (isInstant)
        {
            splash.Close();
            return;
        }
        var timeSpanForFade = new TimeSpan(0, 0, 0, 0,500 );
        splash.CloseDelayed(timeSpanForFade);
    }

    public void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
    {
        if (!_isStarted) return;
        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            if (key == Key.None) continue;
            if (Keyboard.IsKeyDown(key)) return;
        }
        _isKeyDown = false;
    }

    private void BtnStart_OnClick(object sender, RoutedEventArgs e)
    {
        _isStarted = !_isStarted;
        btnStart.Content = _isStarted ? "Stop" : "Start";
    }
}