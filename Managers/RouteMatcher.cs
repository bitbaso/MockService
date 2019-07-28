using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using MockService.Models;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace MockService.Managers
{
    public class RouteMatcher: IRouteMatcher
    {
        #region Private properties
        private readonly ILogger<RouteMatcher> _logger;
        #endregion

        #region Constructors
        public RouteMatcher(ILogger<RouteMatcher> logger)
        {
            this._logger = logger;
        }
        #endregion

        #region Public methods
        public MatchedRoute MatchRoute(string route, string routePattern){
            try{
                var matchedRoute = new MatchedRoute();
                if(!string.IsNullOrEmpty(route)){
                    matchedRoute.IsMatch = IsMatchRoute(route, 
                                                        routePattern);

                    
                }
                return matchedRoute;
            }
            catch(Exception ex){
                _logger.LogError(ex,"MatchRoute");
                return null;
            }
        }
        #endregion

        #region Private methods
        private bool IsMatchRoute(string route, 
                                string routePattern){
            try{
                var outcome = false;

                if(route == routePattern){
                    outcome = true;
                }

                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"IsMatchRoute");
                return false;
            }
        }

        #endregion
    }
}