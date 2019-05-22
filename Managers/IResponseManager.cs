using MockService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MockService.Managers
{
    public interface IResponseManager
    {
         IActionResult GetActionResultFromResponse(Response response);
    }
}