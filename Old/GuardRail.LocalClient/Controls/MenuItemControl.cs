using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GuardRail.LocalClient.Controls;

/// <summary>
/// A menu item for the UI.
/// </summary>
public sealed class MenuItemControl : Grid, INotifyPropertyChanged
{
    private ImageSource _icon;
    private string _title;
    private readonly Rectangle _activeIndicator;

    /// <summary>
    /// PropertyChanged event.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// The icon of the menu item.
    /// </summary>
    public ImageSource Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            OnPropertyChanged(nameof(Icon));
        }
    }

    /// <summary>
    /// The title of the menu item.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    /// <summary>
    /// A menu item for the UI.
    /// </summary>
    public MenuItemControl()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        Height = 50;
        Cursor = Cursors.Hand;
        ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(50)
            });
        ColumnDefinitions.Add(
            new ColumnDefinition());
        ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(5)
            });
        var iconBinding = new Binding(nameof(Icon))
        {
            Source = this
        };
        var image = new Image
        {
            Height = 30,
            Width = 30,
            Margin = new Thickness(10)
        };
        image.SetBinding(Image.SourceProperty, iconBinding);
        image.SetValue(RowProperty, 0);
        image.SetValue(ColumnProperty, 0);
        var titleBinding = new Binding(nameof(Title))
        {
            Source = this
        };
        var label = new Label
        {
            FontFamily = new FontFamily("Teko Bold"),
            Height = 40,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(5)
        };
        label.SetBinding(ContentControl.ContentProperty, titleBinding);
        label.SetValue(RowProperty, 0);
        label.SetValue(ColumnProperty, 1);
        _activeIndicator = new Rectangle
        {
            Visibility = Visibility.Hidden,
            Fill = new SolidColorBrush(Color.FromRgb(0, 156, 204)),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        _activeIndicator.SetValue(RowProperty, 0);
        _activeIndicator.SetValue(ColumnProperty, 2);
        Children.Add(image);
        Children.Add(label);
        Children.Add(_activeIndicator);
        MouseEnter += OnMouseEnter;
        MouseLeave += OnMouseLeave;
    }

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        Background = Brushes.Transparent;
    }

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Enable the active indicator.
    /// </summary>
    public void SetActive() =>
        _activeIndicator.Visibility = Visibility.Visible;

    /// <summary>
    /// Disable the active indicator.
    /// </summary>
    public void SetNotActive() =>
        _activeIndicator.Visibility = Visibility.Hidden;
}