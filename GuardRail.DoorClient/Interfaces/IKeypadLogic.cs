using System.Threading.Tasks;

namespace GuardRail.DoorClient.Interfaces
{
    public interface IKeypadLogic
    {
        Task OnKeyPressedAsync(char key);
    }
}