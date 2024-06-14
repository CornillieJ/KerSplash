using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace KeySplash;

public partial class CustomSplashScreen : Window
{
    protected string Resource { get; set; }
    private TimeSpan _fadeDuration = new (0, 0, 0,1);
    private TimeSpan _closeDuration;
    private DateTime _endFadeTime;
    private DateTime _endCloseTime;
    private bool _stopClosing;
    public int MaxLeft { get; set; }
    public int MaxTop { get; set; }
    private Random _random = new();

    public CustomSplashScreen(Window window,string resource, int width, int height)
    {
        InitializeComponent();
        Owner = window;
        Uri imageUri = new Uri(resource, UriKind.Relative);
        Image.Source = new BitmapImage(imageUri);
        Resource = resource;
        this.Width = width;
        this.Height = height;
        MaxLeft = (int)(System.Windows.SystemParameters.PrimaryScreenWidth - width);
        MaxTop = (int)(System.Windows.SystemParameters.PrimaryScreenHeight - height);
    }

    private void CustomSplashScreen_OnLoaded(object sender, RoutedEventArgs e)
    {
    }
    private async void FadeClose(TimeSpan timeSpan)
    {
        _endFadeTime = DateTime.Now + timeSpan;
        DateTime now = DateTime.Now;
        while (DateTime.Compare(now,_endFadeTime) < 0)
        {
            Opacity = (_endFadeTime - now).Ticks / (double)timeSpan.Ticks;
            await Task.Delay(1);
            now = DateTime.Now;
            //Meet bongocat, hij typt mee met mijn keystrokes
            // nu komt hij tevoorschijn op willekeurige locaties.
            // En dit is hoe ik vanaf nu ga programmeren.
            //Zodat ik me goed kan focussen!
            if (_stopClosing)
            {
                _stopClosing = false;
                Opacity = 1;
                CloseDelayed(_closeDuration);
                return;
            }
        }
        base.Close(); 
    }
    public async void CloseDelayed(TimeSpan timeSpan)
    {
        _closeDuration = timeSpan;
        _endCloseTime = DateTime.Now + timeSpan;
        while (DateTime.Compare(DateTime.Now,_endCloseTime) < 0)
        {
            await Task.Delay(1);
            if (_stopClosing)
            {
                _stopClosing = false;
                _endCloseTime = DateTime.Now + timeSpan;
            }
        }
        FadeClose(_fadeDuration);
    }

    public void ResetClosing()
    {
        _stopClosing = true;
    }

    private void CustomSplashScreen_OnKeyDown(object sender, KeyEventArgs e)
    {
        ((MainWindow)Owner).MainWindow_OnKeyDown(sender,e);
    }

    private void CustomSplashScreen_OnKeyUp(object sender, KeyEventArgs e)
    {
        ((MainWindow)Owner).MainWindow_OnKeyUp(sender,e);
    }
    
    public void PositionSplashScreen(bool isRandomPosition, int x, int y)
    {
        int minLeft = 0;
        int minTop = 0;
        if (Owner.Left < 0)
        {
            minLeft = (int)-System.Windows.SystemParameters.VirtualScreenWidth;
            x = minLeft + x;
            MaxLeft = (int)(0 - Width);
        }
        else if (Owner.Left > System.Windows.SystemParameters.PrimaryScreenWidth)
        {
            minLeft = (int)System.Windows.SystemParameters.PrimaryScreenWidth; 
            x = minLeft + x;
            MaxLeft = (int)(System.Windows.SystemParameters.PrimaryScreenWidth +
                                  System.Windows.SystemParameters.VirtualScreenWidth - Width);
        }
        else
        {
            MaxLeft = (int)(System.Windows.SystemParameters.PrimaryScreenWidth - Width);
            MaxTop = (int)(System.Windows.SystemParameters.PrimaryScreenHeight - Height); 
        }
        if (!isRandomPosition)
        {
            this.Left = Math.Min(x,this.MaxLeft);
            this.Top = Math.Min(y,this.MaxTop);
        }
        else
        {
            this.Left = _random.Next(minLeft,this.MaxLeft);
            this.Top = _random.Next(minTop,this.MaxTop);
        }
    }
}