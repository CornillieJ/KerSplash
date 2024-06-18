using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KeySplash;

public class Animation
{
    public string[] Resources { get; set; }
    private int _frameIndex = 0;
    public Image Image { get; set; }
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isAnimating;

    public Animation(string[] resources, Image image)
    {
        Resources = resources;
        Image = image;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void StopAnimation()
    {
        _cancellationTokenSource.Cancel();
    }
    public void StopAnimation(int delayInMs)
    {
        _cancellationTokenSource.CancelAfter(delayInMs);
    }
    public void StopAnimation(TimeSpan delayTimeSpan)
    {
        _cancellationTokenSource.CancelAfter(delayTimeSpan);
    }
    public async void Animate(int frameDelay)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        if (_isAnimating) return;
        _isAnimating = true;
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            Uri nextUri = new Uri(Resources[_frameIndex], UriKind.Relative);
            Image.Source = new BitmapImage(nextUri);
            await Task.Delay(frameDelay);
            _frameIndex++;
            if (_frameIndex == Resources.Length) _frameIndex = 0;
        }
        _frameIndex = 0;
        _isAnimating = false;
    }
}