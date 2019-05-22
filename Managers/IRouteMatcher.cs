using MockService.Models;

namespace MockService.Managers
{
    public interface IRouteMatcher
    {
         MatchedRoute MatchRoute(string route, string routePattern);
    }
}