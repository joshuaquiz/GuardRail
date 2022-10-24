using GuardRail.DeviceLogic.Implementations.Feedback;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Implementation.Feedback.Buzzer;

public sealed class BuzzerManager : CoreBuzzerManager<BuzzerManager>
{
    public BuzzerManager(
        IBuzzerConfiguration buzzerConfiguration,
        IBuzzerHardwareManager buzzerManager,
        ILogger<BuzzerManager> logger)
        : base(
            buzzerConfiguration,
            buzzerManager,
            logger)
    {
    }
}