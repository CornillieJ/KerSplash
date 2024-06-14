using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KeySplash;

public partial class CustomSplashScreen : Window
{
    protected string Resource { get; set; }
    private TimeSpan _fadeDuration = new (0, 0, 0,1);
    private TimeSpan _closeDuration;
    private DateTime _endFadeTime;
    private DateTime _endCloseTime;
    private bool _stopClosing;

    public CustomSplashScreen(Window window,string resource)
    {
        InitializeComponent();
        Owner = window;
        Uri imageUri = new Uri(resource, UriKind.Relative);
        Image.Source = new BitmapImage(imageUri);
        Resource = resource;
    }

    private void CustomSplashScreen_OnLoaded(object sender, RoutedEventArgs e)
    {
        this.Width = Image.Width;
        this.Height = Image.Height;
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
}