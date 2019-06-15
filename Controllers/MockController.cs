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
        private readonly IResponseManager _responseManager;
        private readonly ILogger<MocksController> _logger;
        #endregion

        #region Constructors
        public MocksController(IRequestManager requestManager, 
                               IResponseManager responseManager,
                               ILogger<MocksController> logger)
        {
            this._requestManager = requestManager;
            this._responseManager = responseManager;
            this._logger = logger;
        }
        #endregion

        [HttpGet("{*url}")]
        public async Task<IActionResult> Get(string url)
        {
            try{
                IActionResult outcome = null;
                var receivedRequest = new ReceivedRequest(url);
                await _requestManager.LoadMocksData();
                var response = _requestManager.GetResponseFromRequest(receivedRequest);
                outcome = _responseManager.GetActionResultFromResponse(response);
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"Get");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");;
            }
            
        }

        [HttpPost("{*url}")]
        public async Task<IActionResult> Post(string url, [FromBody] object data)
        {
            try{
                IActionResult outcome = null;
                var receivedRequest = new ReceivedRequest(url, data);
                await _requestManager.LoadMocksData();
                var response = _requestManager.GetResponseFromRequest(receivedRequest);
                outcome = _responseManager.GetActionResultFromResponse(response);
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"Post");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");;
            }
        }
    }
}
