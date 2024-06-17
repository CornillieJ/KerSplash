using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace KeySplash;

public partial class CustomSplashScreen : System.Windows.Window
{
    protected string Resource { get; set; }
    private TimeSpan _fadeDuration = new (0, 0, 0,1);
    private TimeSpan _closeDuration;
    private DateTime _endFadeTime;
    private DateTime _endCloseTime;
    private bool _stopClosing;
    public int MinLeft { get; set; }
    public int MaxLeft { get; set; }
    public int MinTop { get; set; }
    public int MaxTop { get; set; }
    private Random _random = new();

    public CustomSplashScreen(Window window,string resource, int width, int height, int? minLeft = null, int? minTop=null, int? maxLeft = null, int? maxTop=null)
    {
        InitializeComponent();
        Owner = window;
        Uri imageUri = new Uri(resource, UriKind.Relative);
        Image.Source = new BitmapImage(imageUri);
        Resource = resource;
        this.Width = width;
        this.Height = height;
        MinLeft = minLeft ?? 0;
        MinTop = minTop ?? 0;
        MaxLeft = maxLeft?? (int)(System.Windows.SystemParameters.PrimaryScreenWidth - width);
        MaxTop = maxTop?? (int)(System.Windows.SystemParameters.PrimaryScreenHeight - height);
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
        if (!isRandomPosition)
        {
            Left = Math.Min(x,this.MaxLeft);
            Top = Math.Min(y,this.MaxTop);
        }
        else
        {
            Left = _random.Next(MinLeft,(int)(MaxLeft-Width));
            Top = _random.Next(MinTop, (int)(MaxTop - Width));
        }
        if(System.Windows.Forms.Cursor.Position.X >= Left && System.Windows.Forms.Cursor.Position.X <= Left + Width)
            Left = MaxLeft - Left;
    }

    private void CustomSplashScreen_OnMouseEnter(object sender, MouseEventArgs e)
    {
        this.Left = MaxLeft - Left;
    }
}