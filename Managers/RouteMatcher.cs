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
        private char _variableStartChar = '{';
        private char _variableEndChar = '}';
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
                                                        routePattern,
                                                        this._variableStartChar,
                                                        this._variableEndChar);

                    
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
                                string routePattern, 
                                char variableStartChar,
                                char variableEndChar){
            try{
                var outcome = false;

                var template = TemplateParser.Parse(routePattern);

                var matcher = new TemplateMatcher(template, GetDefaults(template));

                if(!route.StartsWith('/')){
                    route = $"/{route}";
                }
                var parameterValues = new RouteValueDictionary();
                outcome = matcher.TryMatch(route, parameterValues);

                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"IsMatchRoute");
                return false;
            }
        }

        private RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }

        #endregion
    }
}