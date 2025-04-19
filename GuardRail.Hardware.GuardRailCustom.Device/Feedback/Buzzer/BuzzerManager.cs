using GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Feedback.Buzzer;

public sealed class BuzzerManager : CoreBuzzerManager<BuzzerManager, int>
{
    public BuzzerManager(
        IBuzzerConfiguration<int> buzzerConfiguration,
        IBuzzerHardwareManager<int> buzzerManager,
        ILogger<BuzzerManager> logger)
        : base(
            buzzerConfiguration,
            buzzerManager,
            logger)
    {
    }
}