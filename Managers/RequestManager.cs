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
                this._GETMockRelations = this._dataManager.GetGETMockRelations(MockRelations);
                this._POSTMockRelations = this._dataManager.GetPOSTMockRelations(MockRelations);
            }
        }
        public Response GetResponseFromRequest(ReceivedRequest receivedRequest){
            try{
                Response outcome = null;
                if(receivedRequest != null){
                    var MockRelationOfReceivedRequest = GetMockRelationOfReceivedRequest(receivedRequest, 
                                                                                         this._GETMockRelations, 
                                                                                         this._POSTMockRelations);
                    if(MockRelationOfReceivedRequest != null){
                        outcome = MockRelationOfReceivedRequest.Response;
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
        private MockRelation GetMockRelationOfReceivedRequest(ReceivedRequest receivedRequest, 
                                                              List<MockRelation> getMockRelations,
                                                              List<MockRelation> postMockRelations){
            try{
                MockRelation outcome = null;

                if(receivedRequest != null && receivedRequest.Data != null){
                    outcome = GetPostMockRelationOfReceivedRequest(receivedRequest, postMockRelations);
                }
                else{
                    outcome = GetGetMockRelationOfReceivedRequest(receivedRequest, getMockRelations);
                }

                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetMockRelationOfReceivedRequest");
                return null;
            }
        }

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