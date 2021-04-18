using System.Threading.Tasks;
using TRMWPFDesktopUI.Models;

namespace TRMWPFDesktopUI.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
    }
}