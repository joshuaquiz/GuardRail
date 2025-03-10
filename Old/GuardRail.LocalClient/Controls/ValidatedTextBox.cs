using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GuardRail.LocalClient.Controls;

/// <summary>
/// A custom TextBox that validates its contents.
/// </summary>
public class ValidatedTextBox : TextBox
{
    private readonly Func<string, CancellationToken, Task<bool>> _validate;
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Whether or not the TextBox has a valid value.
    /// </summary>
    public bool HasValidationError { get; set; }

    /// <summary>
    /// A custom TextBox that validates its contents.
    /// </summary>
    /// <param name="validate">The function to validate the contents of the TextBox.</param>
    public ValidatedTextBox(Func<string, CancellationToken, Task<bool>> validate)
    {
        _validate = validate;
        _cancellationTokenSource = new CancellationTokenSource();
        FontFamily = new FontFamily("Teko Bold");
        VerticalContentAlignment = VerticalAlignment.Center;
        TextChanged += Input_TextChanged;
    }

    private async void Input_TextChanged(object sender, TextChangedEventArgs e)
    {
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        var isValid = await _validate(Text, _cancellationTokenSource.Token);
        HasValidationError = !isValid;
        Background = HasValidationError
            ? Brushes.Tomato
            : Brushes.Transparent;
    }
}