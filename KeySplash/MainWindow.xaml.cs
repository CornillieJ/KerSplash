using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gma.System.MouseKeyHook;
using KeySplash.SplashScreens;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace KeySplash;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private NotifyIcon _notifyIcon;
    private const string ResourceLocation = "resources/bongo_idle.jpg";
    private const string IconLocation = "resources/splash.ico";// "https://www.flaticon.com/free-icons/splash" title="Splash icons">Splash icons created by Freepik - Flaticon
    private readonly string[] Options = ["BongoCat"];
    private bool _isStarted;
    private int _keysPressedCount;
    private int _splashWidth;
    private int _splashHeight;
    private double _imageRatio;
    private bool _isRandomPosition;
    private int _splashX;
    private int _splashY;
    private IKeyboardMouseEvents _globalHook;
    private bool _isMultiple;

    public MainWindow()
    {
        InitializeComponent();
        HookKeyboard();
        InitializeNotifyIcon();
        this.Icon = new BitmapImage(new Uri(IconLocation,UriKind.Relative));
    }

    private void InitializeNotifyIcon()
    {
        _notifyIcon = new NotifyIcon();
        _notifyIcon.Icon = new Icon(IconLocation);
        _notifyIcon.Visible = true;
        _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

        // TODO: Add a context menu to the tray icon
        // _notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
        // {
        //     new MenuItem("Restore", (s, e) => ShowMainWindow()),
        //     new MenuItem("Exit", (s, e) => CloseApplication())
        // };
    }

    private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
    {
        Show();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        txtHeight.Text = "251";
        txtWidth.Text = "154";
        _imageRatio = BongoCat.ImageWidth / (double)BongoCat.ImageHeight;
        cmbOptions.ItemsSource = Options;
        cmbOptions.SelectedIndex = 0;
    } 
    public void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && (Keyboard.Modifiers & ModifierKeys.Shift) != 0 && _isStarted)
        {
            BtnStart_OnClick(null,null);
            _isStarted = false;
            return;
        }
        if (_keysPressedCount > 0 || !_isStarted) return;
        _keysPressedCount++;
        if (Application.Current.Windows.OfType<CustomSplashScreen>().FirstOrDefault() is CustomSplashScreen
            OpenSplashScreen && !_isMultiple)
        {
            OpenSplashScreen.ResetClosing();
            OpenSplashScreen.PositionSplashScreen(_isRandomPosition, _splashX, _splashY);
            if(OpenSplashScreen is BongoCat bongoCat) bongoCat.Tap();
            return;
        }
        CustomSplashScreen _currentCustomSplashScreen = ShowBongo();
        HideSplash(_currentCustomSplashScreen);
    }
    public void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
    {
        _keysPressedCount--;
        if (_keysPressedCount < 0) _keysPressedCount = 0;
        if (!_isStarted) return;
        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            if (key == Key.None) continue;
            if (Keyboard.IsKeyDown(key)) return;
        }
    }

    private void BtnStart_OnClick(object sender, RoutedEventArgs e)
    {
        _isStarted = !_isStarted;
        btnStart.Content = _isStarted ? "Stop" : "Start";
        txtHeight.IsEnabled = !txtHeight.IsEnabled;
        txtWidth.IsEnabled = !txtWidth.IsEnabled;
        txtX.IsEnabled = !txtX.IsEnabled;
        txtY.IsEnabled = !txtY.IsEnabled;
        chkRandom.IsEnabled = !chkRandom.IsEnabled;
        int.TryParse(txtHeight.Text.Trim(), out _splashHeight);
        int.TryParse(txtWidth.Text.Trim(), out _splashWidth);
        int.TryParse(txtX.Text.Trim(), out _splashX);
        int.TryParse(txtY.Text.Trim(), out _splashY);
    }
    
    private void TxtWidth_OnLostFocus(object sender, RoutedEventArgs e)
    {
        int.TryParse(txtWidth.Text.Trim(), out int width);
        txtHeight.Text = Math.Round(width / _imageRatio).ToString();
    }

    private void TxtHeight_OnLostFocus(object sender, RoutedEventArgs e)
    {
        int.TryParse(txtHeight.Text.Trim(), out int height);
        txtWidth.Text = Math.Round(height * _imageRatio).ToString();
    }

    private void ChkRandom_OnChecked(object sender, RoutedEventArgs e)
    {
        _isRandomPosition = chkRandom.IsChecked??false;
        if (stkPositions is null) return;
        stkPositions.Visibility = _isRandomPosition? Visibility.Hidden: Visibility.Visible;
    }
    private CustomSplashScreen ShowSplash(string resource)
    {
        CustomSplashScreen splash = new CustomSplashScreen(this,resource,_splashWidth,_splashHeight);
        splash.PositionSplashScreen(_isRandomPosition, _splashX, _splashY);
        splash.Show();
        splash.Topmost = true;
        return splash;
    }
    private CustomSplashScreen ShowBongo()
    {
        CustomSplashScreen splash = new BongoCat(this, _splashWidth,_splashHeight);
        splash.PositionSplashScreen(_isRandomPosition, _splashX, _splashY);
        splash.Show();
        splash.Topmost = true;
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

    private void HookKeyboard()
    {
        _globalHook = Hook.GlobalEvents();
        _globalHook.KeyDown += GlobalHook_KeyDown;
        _globalHook.KeyUp += GlobalHook_KeyUp;
    }
    private void GlobalHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {
        Key key = KeyInterop.KeyFromVirtualKey(e.KeyValue);
        KeyEventArgs newE = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, key);
        MainWindow_OnKeyDown(sender,newE);
    }

    private void GlobalHook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
    {
        Key key = KeyInterop.KeyFromVirtualKey(e.KeyValue);
        KeyEventArgs newE = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, key);
        MainWindow_OnKeyUp(sender,newE);
    }

    protected override void OnClosed(EventArgs e)
    {
        _globalHook.KeyDown -= GlobalHook_KeyDown;
        _globalHook.KeyUp -= GlobalHook_KeyUp;
        _globalHook.Dispose();
        base.OnClosed(e);
    }

    private void ChkMultiple_OnChecked(object sender, RoutedEventArgs e)
    {
        _isMultiple = chkMultiple.IsChecked ?? false;
        if (_isMultiple)
            _isRandomPosition = true;
        else _isRandomPosition = chkRandom.IsChecked ?? false;
    }

    private void MainWindow_OnStateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            Hide();
            _notifyIcon.ShowBalloonTip(600, "Minimized", "Kersplash is minimized to the tray.", ToolTipIcon.Info);
        }
    }
}