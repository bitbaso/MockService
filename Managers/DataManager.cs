using System.Linq;
using System.IO;
using System;
using MockService.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MockService.Managers
{
    public class DataManager: IDataManager
    {
        #region Private properties
        private const string _dataFolder = "Data";
        private const string _filePattern = "*.json";
        private readonly ILogger<DataManager> _logger;

        private readonly string  RequestTypePOST = "POST";
        private readonly string  RequestTypeGET = "GET";
        #endregion

        #region Constructors
        public DataManager(ILogger<DataManager> logger){
           this._logger = logger;
        }
        #endregion

        public async Task<List<MockRelation>> LoadMockRelations(){
            try{
                List<MockRelation> outcome = null;
                outcome = await LoadMockRelationsFromJson(_dataFolder, _filePattern);
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"LoadMockRelations");
                return null;
            }
        }

        public List<MockRelation> GetPOSTMockRelations(List<MockRelation> MockRelations){
            try{
                var postMockRelations = new List<MockRelation>();
                if(MockRelations != null){
                    postMockRelations = MockRelations.Where((MockRelation) => MockRelation.Request.Type == RequestTypePOST).ToList();
                };
                return postMockRelations;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetPOSTMockRelations");
                return null;
            }
        }

        public List<MockRelation> GetGETMockRelations(List<MockRelation> MockRelations){
            try{
                var postMockRelations = new List<MockRelation>();
                if(MockRelations != null){
                    postMockRelations = MockRelations.Where((MockRelation) => MockRelation.Request.Type == RequestTypeGET).ToList();
                };
                return postMockRelations;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetGETMockRelations");
                return null;
            }
        }

        private async Task<List<MockRelation>> LoadMockRelationsFromJson(string dataFolder, string filePattern)
        {
            var outcome = new List<MockRelation>();

            string[] files = Directory.GetFiles(dataFolder, 
                                                filePattern, 
                                                SearchOption.AllDirectories);
            if(files != null){
                foreach(var file in files){
                    using (var r = new StreamReader(file))
                            {
                                string json = r.ReadToEnd();
                                var loadedMockRealtion = JsonConvert.DeserializeObject<MockRelation>(json);
                                outcome.Add(loadedMockRealtion);
                            }
                }
            }            
            
            return outcome;
        }

       
    }
}