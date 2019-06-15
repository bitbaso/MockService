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
        private List<MockRelation> _GETMockRelations;
        private List<MockRelation> _POSTMockRelations;
        
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
        public async Task LoadMocksData(){
            var MockRelations = await this._dataManager.LoadMockRelations();
            if(MockRelations != null) {
                this._GETMockRelations = this._dataManager.GetMockRelations(MockRelations, RequestType.GET);
                this._POSTMockRelations = this._dataManager.GetMockRelations(MockRelations, RequestType.POST);
            }
        }
        public Response GetResponseFromRequest(ReceivedRequest receivedRequest, string requestMethodType){
            try{
                Response outcome = null;
                if(receivedRequest != null && requestMethodType != null){
                    MockRelation _mockRelationOfReceivedRequest = null;
                    switch(requestMethodType){
                        case RequestType.GET:
                            _mockRelationOfReceivedRequest = GetGetMockRelationOfReceivedRequest(receivedRequest, this._GETMockRelations);
                            break;
                        case RequestType.POST:
                            _mockRelationOfReceivedRequest = GetPostMockRelationOfReceivedRequest(receivedRequest, this._POSTMockRelations);
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

        private MockRelation GetGetMockRelationOfReceivedRequest(ReceivedRequest receivedRequest, List<MockRelation> MockRelations){
            try{
                MockRelation outcome = null;

                if(receivedRequest != null 
                    && !string.IsNullOrEmpty(receivedRequest.Url) 
                    && MockRelations != null){
                    foreach(var MockRelation in MockRelations){
                        if(MockRelation?.Request != null){
                            var matchedRoute = _routeMatcher.MatchRoute(receivedRequest.Url, MockRelation.Request.Url);
                            if(matchedRoute != null && matchedRoute.IsMatch){
                                outcome = MockRelation;
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

         private MockRelation GetPostMockRelationOfReceivedRequest(ReceivedRequest receivedRequest, List<MockRelation> MockRelations){
            try{
                MockRelation outcome = null;

                if(receivedRequest != null 
                    && !string.IsNullOrEmpty(receivedRequest.Url) 
                    && MockRelations != null){
                    foreach(var MockRelation in MockRelations){
                        if(MockRelation?.Request != null){

                            var matchedRoute = _routeMatcher.MatchRoute(receivedRequest.Url, MockRelation.Request.Url);
                            if(matchedRoute != null 
                                && matchedRoute.IsMatch
                                && IsSameObject(MockRelation.Request.Data, receivedRequest.Data)){
                                outcome = MockRelation;
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


        private bool IsSameObject(object obj1, object obj2){
            try{
                var outcome = false;
                var serializedObject1 = JsonConvert.SerializeObject(obj1);
                var serializedObject2 = JsonConvert.SerializeObject(obj2);

                if(serializedObject1 == serializedObject2){
                    outcome = true;
                }
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"IsSameObject");
                return false;
            }
        }
        #endregion


    }
}