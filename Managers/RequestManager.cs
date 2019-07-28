using System.Runtime.Serialization;
using System;
using MockService.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MockService.Managers
{
    public class RequestManager: IRequestManager
    {
        #region Private properties
        private readonly IDataManager _dataManager;
        private readonly IRouteMatcher _routeMatcher;
        private readonly ILogger<RequestManager> _logger;
        #endregion

        #region Constructors
        public RequestManager(IDataManager dataManager, 
                              IRouteMatcher routeMatcher,
                              ILogger<RequestManager> logger){

            this._logger = logger;                                  
            this._dataManager = dataManager;
            this._routeMatcher = routeMatcher;            
        }
        #endregion

        #region Public methods        
        public async Task<Response> GetResponseFromRequest(ReceivedRequest receivedRequest, string requestMethodType){
            try{
                Response outcome = null;
                if(receivedRequest != null && requestMethodType != null){
                    MockRelation _mockRelationOfReceivedRequest = null;
                    switch(requestMethodType){
                        case RequestType.GET:
                            _mockRelationOfReceivedRequest = await GetGetMockRelationOfReceivedRequest(receivedRequest);
                            break;
                        case RequestType.POST:
                            _mockRelationOfReceivedRequest = await GetPostMockRelationOfReceivedRequest(receivedRequest);
                            break;
                    }

                    if(_mockRelationOfReceivedRequest != null){
                        outcome = _mockRelationOfReceivedRequest.Response;
                    }
                }
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetResponseFromRequest");
                return null;
            }
        }
        #endregion


        #region Private methods

        private async Task<MockRelation> GetGetMockRelationOfReceivedRequest(ReceivedRequest receivedRequest){
            try{
                MockRelation outcome = null;

                var mockRelations = await this._dataManager.LoadMockRelations();
                var getMockRelations = this._dataManager.GetMockRelations(mockRelations, RequestType.GET);

                if(receivedRequest != null 
                    && !string.IsNullOrEmpty(receivedRequest.Url) 
                    && getMockRelations != null){
                    foreach(var mockRelation in getMockRelations){
                        if(mockRelation?.Request != null){
                            var matchedRoute = _routeMatcher.MatchRoute(receivedRequest.Url, mockRelation.Request.Url);
                            if(matchedRoute != null && matchedRoute.IsMatch){
                                outcome = mockRelation;
                            }
                        }
                    }
                }

                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetGetMockRelationOfReceivedRequest");
                return null;
            }
        }

         private async Task<MockRelation> GetPostMockRelationOfReceivedRequest(ReceivedRequest receivedRequest){
            try{
                MockRelation outcome = null;

                var mockRelations = await this._dataManager.LoadMockRelations();
                var postMockRelations = this._dataManager.GetMockRelations(mockRelations, RequestType.POST);

                if(receivedRequest != null 
                    && !string.IsNullOrEmpty(receivedRequest.Url) 
                    && postMockRelations != null){
                    foreach(var mockRelation in postMockRelations){
                        if(mockRelation?.Request != null){

                            var matchedRoute = _routeMatcher.MatchRoute(receivedRequest.Url, mockRelation.Request.Url);
                            var jsonEscaped = JsonConvert.ToString(receivedRequest.Data);
                            if(matchedRoute != null 
                                && matchedRoute.IsMatch
                                && mockRelation.Request.Data == jsonEscaped){
                                outcome = mockRelation;
                            }
                        }
                    }
                }

                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetPostMockRelationOfReceivedRequest");
                return null;
            }
        }

        #endregion

    }
}