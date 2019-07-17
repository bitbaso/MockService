using System.Linq;
using System.IO;
using System;
using MockService.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using LiteDB;

namespace MockService.Managers
{
    public class DataManager: IDataManager
    {
        #region Private properties
        private const string _dataFolder = "Data";
        private const string _filePattern = "*.json";
        private const string _dbFilePath = "Data/MockService.db";
        private const string _mockRelationCollectionName = "MockRelationList";
        private readonly ILogger<DataManager> _logger;
        #endregion

        #region Constructors
        public DataManager(ILogger<DataManager> logger){
           this._logger = logger;
        }
        #endregion

        public async Task<List<MockRelation>> LoadMockRelations(){
            try{
                List<MockRelation> outcome = null;
                //var mockRelationsFromJson = await LoadMockRelationsFromJson(_dataFolder, _filePattern);
                //await AddMockRelations(mockRelationsFromJson);
                outcome = await GetMockRelationsFromDB();
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"LoadMockRelations");
                return null;
            }
        }

        public List<MockRelation> GetMockRelations(List<MockRelation> MockRelations, string requestMethodType){
            try{
                var mockRelations = new List<MockRelation>();
                if(MockRelations != null){
                    mockRelations = MockRelations.Where((MockRelation) => MockRelation.Request.Type == requestMethodType).ToList();
                };
                return mockRelations;
            }
            catch(Exception ex){
                _logger.LogError(ex,"GetMockRelations");
                return null;
            }
        }

        public async Task<bool> AddMockRelations(List<MockRelation> mockRelations){
            try{
                var outcome = false;
                if(mockRelations != null){
                    foreach(var mockRelation in mockRelations){
                        await AddMockRelation(mockRelation);
                    }
                    outcome = true;
                }
                return outcome;
            }
            catch(Exception ex){
                 _logger.LogError(ex,"AddMockRelations");
                return false;
            }
        }

        public async Task<bool> AddMockRelation(MockRelation mockRelation){
            try{
                var outcome = false;

                if(mockRelation != null){
                    using(var db = new LiteDatabase(_dbFilePath))
                    {
                        var mockRelationsCollection = db.GetCollection<MockRelation>(_mockRelationCollectionName);

                        var mockRelationFiltered = mockRelationsCollection.FindOne(x => x.Request.Url == mockRelation.Request.Url 
                                                    && x.Request.Data == mockRelation.Request.Data);

                        if(mockRelationFiltered == null){
                            mockRelationsCollection.Insert(mockRelation);
                            mockRelationsCollection.EnsureIndex(x => x.Request.Url);
                            outcome = true;
                        }
                    }
                }
                
                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"AddMockRelation");
                return false;
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

        private async Task<List<MockRelation>> GetMockRelationsFromDB(){
            try{
                List<MockRelation> outcome = null;
                using(var db = new LiteDatabase(_dbFilePath))
                {
                    var col = db.GetCollection<MockRelation>(_mockRelationCollectionName);
                    outcome = col.FindAll().ToList();
                }

                return outcome;
            }
            catch(Exception ex){
                _logger.LogError(ex,"LoadMockRelationsFromDB");
                return null;
            }
        }

    }
}