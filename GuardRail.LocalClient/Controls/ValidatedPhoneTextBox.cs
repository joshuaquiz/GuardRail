using System.Threading.Tasks;
using GuardRail.Core;

namespace GuardRail.LocalClient.Controls;

/// <summary>
/// A custom TextBox that validates for phones.
/// </summary>
public sealed class ValidatedPhoneTextBox : ValidatedTextBox
{
    /// <summary>
    /// A custom TextBox that validates for phones.
    /// </summary>
    public ValidatedPhoneTextBox()
        : base((x, ct) => Task.FromResult(x.IsValidPhoneFormat()))
    {
    }
}