﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace KeySplash;

class GifImage : Image
{
    private bool _isInitialized;
    private GifBitmapDecoder _gifDecoder;
    private Int32Animation _animation;
    public static readonly DependencyProperty GifSourceProperty =
        DependencyProperty.Register("GifSource", typeof(string), typeof(GifImage), new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));
    public static readonly DependencyProperty FrameIndexProperty =
        DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));
    public static readonly DependencyProperty AutoStartProperty =
        DependencyProperty.Register("AutoStart", typeof(bool), typeof(GifImage), new UIPropertyMetadata(false, AutoStartPropertyChanged));

    public int FrameIndex
    {
        get { return (int)GetValue(FrameIndexProperty); }
        set { SetValue(FrameIndexProperty, value); }
    }

    private void Initialize()
    {
        _gifDecoder = new GifBitmapDecoder(new Uri(this.GifSource,UriKind.Relative), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
        _animation = new Int32Animation(0, _gifDecoder.Frames.Count - 1, new Duration(new TimeSpan(0, 0, 0, _gifDecoder.Frames.Count / 10, (int)((_gifDecoder.Frames.Count / 10.0 - _gifDecoder.Frames.Count / 10) * 1000))));
        _animation.RepeatBehavior = RepeatBehavior.Forever;
        this.Source = _gifDecoder.Frames[0];

        _isInitialized = true;
    }

    static GifImage()
    {
        VisibilityProperty.OverrideMetadata(typeof (GifImage),
            new FrameworkPropertyMetadata(VisibilityPropertyChanged));
    }

    private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if ((Visibility)e.NewValue == Visibility.Visible)
        {
            ((GifImage)sender).StartAnimation();
        }
        else
        {
            ((GifImage)sender).StopAnimation();
        }
    }


    static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
    {
        var gifImage = obj as GifImage;
        gifImage.Source = gifImage._gifDecoder.Frames[(int)ev.NewValue];
    }

    /// <summary>
    /// Defines whether the animation starts on it's own
    /// </summary>
    public bool AutoStart
    {
        get { return (bool)GetValue(AutoStartProperty); }
        set { SetValue(AutoStartProperty, value); }
    }

    private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
            (sender as GifImage).StartAnimation();
    }

    public string GifSource
    {
        get { return (string)GetValue(GifSourceProperty); }
        set { SetValue(GifSourceProperty, value); }
    }


    private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        (sender as GifImage).Initialize();
    }
    public void StartAnimation()
    {
        if (!_isInitialized)
            this.Initialize();

        BeginAnimation(FrameIndexProperty, _animation);
    }
    public void StopAnimation()
    {
        BeginAnimation(FrameIndexProperty, null);
    }
}