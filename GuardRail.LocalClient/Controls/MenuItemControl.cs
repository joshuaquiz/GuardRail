using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GuardRail.LocalClient.Controls
{
    /// <summary>
    /// A menu item for the UI.
    /// </summary>
    public class MenuItemControl : Grid, INotifyPropertyChanged
    {
        private ImageSource _icon;
        private string _title;

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
            var iconBinding = new Binding(nameof(Icon))
            {
                Source = this
            };
            var image = new Image
            {
                Height = 40,
                Width = 40,
                Margin = new Thickness(5)
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
            Children.Add(image);
            Children.Add(label);
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

        /// <summary>
        /// OnPropertyChanged handler.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}