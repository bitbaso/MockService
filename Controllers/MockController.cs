using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MockService.Managers;
using MockService.Models;
using Microsoft.Extensions.Logging;

namespace MockService.Controllers
{
    [Route("")]
    [ApiController]
    public class MocksController : ControllerBase
    {
        #region Private properties
        private readonly IRequestManager _requestManager;
        private readonly IProxyManager _proxyManager;
        private readonly IResponseManager _responseManager;
        private readonly ILogger<MocksController> _logger;
        #endregion

        #region Constructors
        public MocksController(IRequestManager requestManager, 
                               IProxyManager proxyManager,
                               IResponseManager responseManager,
                               ILogger<MocksController> logger)
        {
            this._requestManager = requestManager;
            this._proxyManager = proxyManager;
            this._responseManager = responseManager;
            this._logger = logger;
        }
        #endregion

        [HttpGet("{*url}")]
        public async Task<IActionResult> Get(string url)
        {
            try{
                return await GetActionResultOfRequestType(url, 
                                                          null, 
                                                          RequestType.GET);
            }
            catch(Exception ex){
                _logger.LogError(ex,"Get");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }            
        }

        [HttpPost("{*url}")]
        public async Task<IActionResult> Post(string url, [FromBody] object data)
        {
            try{
                return await GetActionResultOfRequestType(url, 
                                                          data, 
                                                          RequestType.POST);
            }
            catch(Exception ex){
                _logger.LogError(ex,"Post");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }
        }
        
        [HttpDelete("{*url}")]
        public async Task<IActionResult> Delete(string url)
        {
            try{
                return await GetActionResultOfRequestType(url, 
                                                          null, 
                                                          RequestType.GET);
            }
            catch(Exception ex){
                _logger.LogError(ex,"Get");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }            
        }

        [HttpPut("{*url}")]
        public async Task<IActionResult> Put(string url, [FromBody] object data)
        {
            try{
                return await GetActionResultOfRequestType(url, 
                                                          data, 
                                                          RequestType.POST);
            }
            catch(Exception ex){
                _logger.LogError(ex,"Post");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }
        }

        private async Task<IActionResult> GetActionResultOfRequestType(string url, 
                                                                       object data, 
                                                                       string requestType){
            try{
                IActionResult outcome = null;
                var queryString = HttpContext.Request.QueryString.Value;
                var completeUrl = $"{url}{queryString}";
                var receivedRequest = new ReceivedRequest(completeUrl, data);
                var response = await _requestManager.GetResponseFromRequest(receivedRequest, requestType);
                if(response == null){
                    response = await _proxyManager.GetResponse(receivedRequest, requestType);
                }
                outcome = _responseManager.GetActionResultFromResponse(response);
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetActionResultOfRequestType");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }
        } 
    }
}
