using System.Windows;
using System.Windows.Media.Imaging;

namespace KeySplash.SplashScreens;

public class BongoCat:CustomSplashScreen
{
    private const string BongoCatIdle = "/resources/bongo_idle.jpg"; 
    private const string BongoCatLeft = "/resources/bongo_left.jpg"; 
    private const string BongoCatRight = "/resources/bongo_right.jpg";
    private TimeSpan BongoDelay = new (0, 0, 0, 0, 200);
    private Random random = new();
    public BongoCat(Window window) : base(window, BongoCatIdle)
    {
    }

    public void Tap()
    {
        string resource= BongoCatIdle;
        switch (Resource)
        {
            case BongoCatIdle:
                string[] bongos = [BongoCatLeft,BongoCatRight];
                resource = bongos[random.Next(2)];
                break;
            case BongoCatLeft:
                resource = BongoCatRight;
                break;
            case BongoCatRight:
                resource = BongoCatLeft;
                break;
        }

        ChangeImage(resource);
        ImageBackToIdle();
    }

    public async void ImageBackToIdle(TimeSpan? timeSpan = null)
    {
        if (timeSpan == null) timeSpan = BongoDelay;
        DateTime changeTime = (DateTime)(DateTime.Now + timeSpan);
        while (DateTime.Compare(DateTime.Now, changeTime) < 0)
        {
            await Task.Delay(1);
        }
        ChangeImage(BongoCatIdle);
    }

    private void ChangeImage(string resource)
    {
        Resource = resource;
        Uri uri = new Uri(resource,UriKind.Relative);
        Image.Source = new BitmapImage(uri);
    }
}