using System.Threading.Tasks;
using GuardRail.Core;

namespace GuardRail.LocalClient.Controls;

/// <summary>
/// A custom TextBox that validates for emails.
/// </summary>
public sealed class ValidatedEmailTextBox : ValidatedTextBox
{
    /// <summary>
    /// A custom TextBox that validates for emails.
    /// </summary>
    public ValidatedEmailTextBox()
        : base((x, ct) => Task.FromResult(x.IsValidEmailFormat()))
    {
    }
}