using System.Threading.Tasks;
using MockService.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;

namespace MockService.Managers
{
    public class ProxyManager: IProxyManager
    {
        #region Private properties
        private bool _isProxyEnabled = false;
        private readonly ILogger<ProxyManager> _logger;
        #endregion

        #region Constructors
        public ProxyManager(IConfiguration config, ILogger<ProxyManager> logger){
            this._logger = logger;
            this._isProxyEnabled = config.GetValue<bool>("ProxyEnabled", false);
        }
        #endregion

        #region Public methods
        public async Task<Response> GetResponse(ReceivedRequest receivedRequest, string requestMethodType){
            try{
                Response outcome = null;
                if(_isProxyEnabled){
                    _logger.LogInformation("Calling proxy...");
                }
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetResponse proxy");
                return null;
            }
        }
        #endregion
    }
}