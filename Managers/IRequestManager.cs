using MockService.Models;
using System.Threading.Tasks;

namespace MockService.Managers
{
    public interface IRequestManager
    {
        Task<Response> GetResponseFromRequest(ReceivedRequest receivedRequest, string requestMethodType);
    }
}