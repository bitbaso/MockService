using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MockService.Managers
{
    public class ResponseManager: IResponseManager
    {
        #region Private properties
        private readonly ILogger<ResponseManager> _logger;
        #endregion

        #region Constructors
        public ResponseManager(ILogger<ResponseManager> logger){
            this._logger = logger;
        }
        #endregion

        #region Public methods
        public IActionResult GetActionResultFromResponse(Response response)
        {
            try{
                IActionResult outcome = null;
                if(IsValidResponse(response)){
                    var content = response.Content;
                    var statusToResponse = response.StatusCode;

                    switch(response.ContentType){
                        case MediaType.TextPlain:
                            outcome = GetObjectResult(content, statusToResponse);
                            break;
                        case MediaType.ApplicationJson:
                            outcome = GetJsonResult(content, statusToResponse);
                            break;
                        default:
                            outcome = GetObjectResult(content, statusToResponse);
                            break;
                    }
                }
                else{
                    outcome = GetObjectResult("NoContent", StatusCodes.Status404NotFound);
                }
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetActionResultFromResponse");
                return null;
            }
        }
        #endregion

        #region Private methods
        private bool IsValidResponse(Response response){
            return response != null 
                    && response.Content != null 
                    && response.ContentType != null;
        }

        private ObjectResult GetObjectResult(Object objectValue, int statusCode){
            try{
                var outcome = new ObjectResult(objectValue);
                outcome.StatusCode = statusCode;
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetObjectResult");
                return null;
            }
            
        }

        private JsonResult GetJsonResult(Object objectValue, int statusCode){
            try{
                var outcome = new JsonResult(objectValue);
                outcome.StatusCode = statusCode;
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetJsonResult");
                return null;
            }
            
        }
        #endregion
    }
}