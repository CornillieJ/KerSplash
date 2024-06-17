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
using KeySplash.Data;
using KeySplash.HelperWindows;
// using KeySplash.SplashScreens;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace KeySplash;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<SplashOption> _splashOptions = new List<SplashOption>()
    {
        new SplashOption("BongoCat", 498, 306
            , "/resources/bongo_idle.jpg", ["/resources/bongo_left.jpg", "/resources/bongo_right.jpg"])
    };
    private IKeyboardMouseEvents _globalHook;
    private NotifyIcon _notifyIcon;
    private const string IconLocation = "resources/splash.ico";// "https://www.flaticon.com/free-icons/splash" title="Splash icons">Splash icons created by Freepik - Flaticon
    private bool _isStarted;
    private int _keysPressedCount;
    private int _splashWidth;
    private int _splashHeight;
    private double _imageRatio;
    private bool _isRandomPosition;
    private int _splashX;
    private int _splashY;
    private int? _rangeMinLeft = null;
    private int? _rangeMaxLeft = null;
    private int? _rangeMinTop = null;
    private int? _rangeMaxTop = null;
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
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Restore", null, (s, e) => NotifyIcon_DoubleClick(null,null));
        contextMenu.Items.Add("Exit", null, (s, e) => Close());

        _notifyIcon.ContextMenuStrip = contextMenu;
    }

    private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
    {
        Show();
        WindowState = WindowState.Normal;
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _splashWidth = 251;
        _splashHeight = 154;
        txtWidth.Text = _splashWidth.ToString();
        txtHeight.Text = _splashHeight.ToString();
        _imageRatio = _splashOptions[0].Width / (double)_splashOptions[0].Height;
        cmbOptions.ItemsSource = _splashOptions;
        cmbOptions.SelectedIndex = 0;
    } 
    public void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && (Keyboard.Modifiers & ModifierKeys.Shift) != 0 && _isStarted)
        {
            BtnStart_OnClick(null,null);
            _isStarted = false;
            Show();
            WindowState = WindowState.Normal;
            return;
        }
        if (_keysPressedCount > 0 || !_isStarted) return;
        _keysPressedCount++;
        if (Application.Current.Windows.OfType<CustomSplashScreen>().FirstOrDefault() is CustomSplashScreen
            OpenSplashScreen && !_isMultiple)
        {
            OpenSplashScreen.ResetClosing();
            OpenSplashScreen.PositionSplashScreen(_isRandomPosition, _splashX, _splashY);
            OpenSplashScreen.Tap();
            return;
        }

        try
        {
            CustomSplashScreen customSplashScreen = ShowSplash();
            HideSplash(customSplashScreen);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
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
        chkRandom.IsEnabled = !chkRandom.IsEnabled;
        int.TryParse(txtHeight.Text.Trim(), out _splashHeight);
        int.TryParse(txtWidth.Text.Trim(), out _splashWidth);
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
        {
            chkRandom.IsChecked = true;
            chkRandom.IsEnabled = false;
            _isRandomPosition = true;
        }
        else
        {
            chkRandom.IsEnabled = true;
            _isRandomPosition = chkRandom.IsChecked ?? false;
        }
    }

    private void MainWindow_OnStateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            Hide();
            _notifyIcon.ShowBalloonTip(600, "Minimized", "Kersplash is minimized to the tray.", ToolTipIcon.Info);
        }
    }
    private void BtnPlacement_OnClick(object sender, RoutedEventArgs e)
    {
        PlacementWindow placementWindow = new(_splashWidth,_splashHeight,_imageRatio);
        if (_splashX != 0 && _splashY != 0)
        {
            placementWindow.Left = _splashX;
            placementWindow.Top = _splashY;
        }
        placementWindow.ShowDialog();
        _splashX = (int)placementWindow.Left;
        _splashY = (int)placementWindow.Top;
        txtHeight.Text = ((int)placementWindow.Height).ToString();
        txtWidth.Text = ((int)placementWindow.Width).ToString();
    }
    private void BtnRange_OnClick(object sender, RoutedEventArgs e)
    {
        PlacementWindow placementWindow = new(_splashWidth,_splashHeight,null);
        if (_splashX != 0 && _splashY != 0)
        {
            placementWindow.Left = _splashX;
            placementWindow.Top = _splashY;
        }
        placementWindow.ShowDialog();
        _rangeMinLeft = (int)placementWindow.Left;
        _rangeMinTop = (int)placementWindow.Top;
        _rangeMaxLeft = (int)(_rangeMinLeft + placementWindow.Width);
        _rangeMaxTop = (int)(_rangeMinTop + placementWindow.Height);
    }
    private void ChkRandom_OnChecked(object sender, RoutedEventArgs e)
    {
        _isRandomPosition = chkRandom.IsChecked??false;
        if (stkPositions is null) return;
        stkPositions.Visibility = _isRandomPosition? Visibility.Collapsed: Visibility.Visible;
        stkRange.Visibility = _isRandomPosition? Visibility.Visible: Visibility.Collapsed;
    }
    private CustomSplashScreen ShowSplash()
    {
        if (cmbOptions.SelectedItem is not SplashOption selectedSplash) throw new Exception("Incorrecte selectie");
        string resource = selectedSplash.IdleResource;
        string[] tapResources = selectedSplash.TapResources;
        CustomSplashScreen splash = new CustomSplashScreen(this,resource,tapResources, _splashWidth,_splashHeight, _rangeMinLeft,_rangeMinTop,_rangeMaxLeft,_rangeMaxTop);
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


}