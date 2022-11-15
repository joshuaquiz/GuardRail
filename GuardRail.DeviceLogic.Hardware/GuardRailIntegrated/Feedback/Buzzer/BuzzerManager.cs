using GuardRail.DeviceLogic.Implementations.Feedback;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Feedback.Buzzer;

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