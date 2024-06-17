using System.Windows;
using System.Windows.Input;

namespace KeySplash.HelperWindows;

public partial class PlacementWindow : Window
{
    private readonly bool _isRange;
    private readonly double _aspectRatio;

    public PlacementWindow(int width, int height, double? aspectRatio)
    {
        InitializeComponent();
        this.Width = width;
        this.Height = height;
        _isRange = aspectRatio == null;
        if (_isRange) btnConfirm.Content = "Confirm Range";
        _aspectRatio = aspectRatio??0;
    }

    private void PlacementWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void BtnConfirm_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void PlacementWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isRange) return;
        if (e.WidthChanged) Height = Width / _aspectRatio;
        if (e.HeightChanged) Width = Height *_aspectRatio;
    }
}