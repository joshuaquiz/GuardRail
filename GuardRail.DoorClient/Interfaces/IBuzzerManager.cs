using System;
using System.Threading.Tasks;

namespace GuardRail.DoorClient.Interfaces
{
    public interface IBuzzerManager
    {
        Task Buzz(TimeSpan duration);
    }
}