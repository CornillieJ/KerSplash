using System.Windows;
using System.Windows.Input;

namespace KeySplash.HelperWindows;

public partial class PlacementWindow : Window
{
    public PlacementWindow(int width, int height)
    {
        InitializeComponent();
        this.Width = width;
        this.Height = height;
    }

    private void PlacementWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void BtnConfirm_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}