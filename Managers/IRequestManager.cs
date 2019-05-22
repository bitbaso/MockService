using MockService.Models;
using System.Threading.Tasks;

namespace MockService.Managers
{
    public interface IRequestManager
    {
        Task LoadMocksData();
        Response GetResponseFromRequest(ReceivedRequest receivedRequest);
    }
}