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

                    _mockRelationOfReceivedRequest = await GetMockRelationOfReceivedRequest(receivedRequest, requestMethodType);

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

        private async Task<MockRelation> GetMockRelationOfReceivedRequest(ReceivedRequest receivedRequest, string requestType){
            try{
                MockRelation outcome = null;

                var mockRelations = await this._dataManager.LoadMockRelations();
                var mockRelationsOfType = this._dataManager.GetMockRelations(mockRelations, requestType);

                if(receivedRequest != null 
                    && !string.IsNullOrEmpty(receivedRequest.Url) 
                    && mockRelationsOfType != null){
                    foreach(var mockRelation in mockRelationsOfType){
                        if(mockRelation?.Request != null){
                            var matchedRoute = _routeMatcher.MatchRoute(receivedRequest.Url, mockRelation.Request.Url);
                            var urlIsEqual = false;
                            var dataIsEqual = false;
                            var hasData = false;

                            if(matchedRoute != null && matchedRoute.IsMatch){
                                urlIsEqual = true;                            
                            }

                            if(receivedRequest.Data != null){
                                hasData = true;
                            }

                            if(hasData && urlIsEqual){
                                var jsonEscaped = JsonConvert.ToString(receivedRequest.Data);
                                if(mockRelation.Request.Data == jsonEscaped){
                                    dataIsEqual = true;
                                }
                            }

                            if((urlIsEqual && !hasData) || (urlIsEqual && dataIsEqual)){
                                outcome = mockRelation;
                                break;
                            }                                
                        }
                    }
                }

                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetMockRelationOfReceivedRequest");
                return null;
            }
        }

        #endregion
    }
}