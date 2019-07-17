using System.Threading.Tasks;
using MockService.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace MockService.Managers
{
    public class ProxyManager: IProxyManager
    {
        #region Private properties
        private bool _isProxyEnabled = false;
        private string _proxyHost = "";
        private readonly IDataManager _dataManager;
        private readonly ILogger<ProxyManager> _logger;
        private readonly HttpClient client = new HttpClient();        
        #endregion

        #region Constructors
        public ProxyManager(IConfiguration config, 
                            ILogger<ProxyManager> logger,
                            IDataManager dataManager){
            this._logger = logger;
            this._isProxyEnabled = config.GetValue<bool>("ProxyEnabled", false);
            this._proxyHost = config.GetValue<string>("ProxyHost", "");
            this._dataManager = dataManager;
        }
        #endregion

        #region Public methods
        public async Task<Response> GetResponse(ReceivedRequest receivedRequest, string requestMethodType){
            try{
                Response outcome = null;
                if(_isProxyEnabled && !string.IsNullOrEmpty(_proxyHost)){
                    var urlToCall = $"{_proxyHost}{receivedRequest.Url}";

                    switch(requestMethodType){
                        case RequestType.GET:
                            outcome = await GetHttpCall(urlToCall);
                            break;
                        case RequestType.POST:
                            outcome = await PostHttpCall(urlToCall, receivedRequest.Data);
                            break;
                    }
                }

                if(outcome != null){
                    SaveMockRelationInDB(outcome, 
                                         receivedRequest, 
                                         requestMethodType);
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
        private async Task<Response> GetHttpCall(string url){
            try{
                var response = await client.GetAsync(url);
                var statusCode = response?.StatusCode;
                var mediaType = response?.Content?.Headers?.ContentType?.MediaType;
                var responseContent = await response?.Content?.ReadAsStringAsync();

                var outcome = new Response(){
                            Content = responseContent,
                            StatusCode = (int)statusCode,
                            ContentType = mediaType
                        };

                return outcome;
            }
            catch(Exception ex){
                 _logger.LogError(ex,"GetHttpCall");
                return null;
            }
        }

        private async Task<Response> PostHttpCall(string url, object data){
            try{
                var myContent = JsonConvert.SerializeObject(data);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue(MediaType.ApplicationJson);

                var response = await client.PostAsync(url, byteContent);
                var statusCode = response?.StatusCode;
                var mediaType = response?.Content?.Headers?.ContentType?.MediaType;
                var responseContent = await response?.Content?.ReadAsStringAsync();

                var outcome = new Response(){
                            Content = responseContent,
                            StatusCode = (int)statusCode,
                            ContentType = mediaType
                        };


                return outcome;
            }
            catch(Exception ex){
                 _logger.LogError(ex,"PostHttpCall");
                return null;
            }
        }

        private async Task<bool> SaveMockRelationInDB(Response response, 
                                                      ReceivedRequest receivedRequest, 
                                                      string requestMethodType){
            try{
                var outcome = false;
                if(response != null 
                   && receivedRequest != null 
                   && requestMethodType != null){
                       var request = new Request(receivedRequest.Url, receivedRequest.Data){
                           Type = requestMethodType
                       };
                       var mockRelationToSave = new MockRelation(request, response);
                       _dataManager.AddMockRelation(mockRelationToSave);
                       outcome = true;
                }
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"SaveMockRelationInDB");
                return false;
            }
        }
        #endregion
    }
}