using System.Threading.Tasks;
using MockService.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
﻿using System.Net.Http.Headers;

namespace MockService.Managers
{
    public class ProxyManager: IProxyManager
    {
        #region Private properties
        private bool _isProxyEnabled = false;
        private string _proxyHost = "";
        private readonly ILogger<ProxyManager> _logger;
        private readonly HttpClient client = new HttpClient();
        #endregion

        #region Constructors
        public ProxyManager(IConfiguration config, ILogger<ProxyManager> logger){
            this._logger = logger;
            this._isProxyEnabled = config.GetValue<bool>("ProxyEnabled", false);
            this._proxyHost = config.GetValue<string>("ProxyHost", "");
        }
        #endregion

        #region Public methods
        public async Task<Response> GetResponse(ReceivedRequest receivedRequest, string requestMethodType){
            try{
                Response outcome = null;
                if(_isProxyEnabled && !string.IsNullOrEmpty(_proxyHost)){
                    var urlToCall = $"{_proxyHost}{receivedRequest.Url}";

                    var callResponse = "";
                    switch(requestMethodType){
                        case RequestType.GET:
                            callResponse = await GetHttpCall(urlToCall);
                            break;
                        case RequestType.POST:
                            callResponse = await PostHttpCall(urlToCall, receivedRequest.Data);
                            break;
                    }
                    if(!string.IsNullOrEmpty(callResponse)){
                        outcome = new Response(){
                            Content = callResponse
                        };
                    }
                }

                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetResponse proxy");
                return null;
            }
        }
        #endregion

        #region Private methods
        private async Task<string> GetHttpCall(string url){
            try{
                return await client.GetStringAsync(url);
            }
            catch(Exception ex){
                 _logger.LogError(ex,"GetHttpCall");
                return null;
            }
        }

        private async Task<string> PostHttpCall(string url, object data){
            try{
                //var content = new FormUrlEncodedContent(data);

                var myContent = JsonConvert.SerializeObject(data);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                var response = await client.PostAsync(url, byteContent);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch(Exception ex){
                 _logger.LogError(ex,"PostHttpCall");
                return null;
            }
        }
        #endregion
    }
}