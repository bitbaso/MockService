using MockService.Models;
using System.Threading.Tasks;

namespace MockService.Managers
{
    public interface IProxyManager
    {
         Task<Response> GetResponse(ReceivedRequest receivedRequest, string requestMethodType);
    }
}