using System.Threading.Tasks;

namespace GuardRail.Hardware.GuardRailCustom.Device.Interfaces;

public interface IAsyncInit
{
    /// <summary>
    /// Initialize the service.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the work to initialize the service.</returns>
    public ValueTask InitAsync();
}