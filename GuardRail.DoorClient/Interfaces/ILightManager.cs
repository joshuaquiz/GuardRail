using System;
using System.Threading.Tasks;

namespace GuardRail.DoorClient.Interfaces
{
    public interface ILightManager
    {
        Task TurnOnRedLight(TimeSpan duration);

        Task TurnOnGreenLight(TimeSpan duration);
    }
}