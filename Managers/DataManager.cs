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
        private const string _dbFilePath = "data/MockService.db";
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
                outcome = await LoadMockRelationsFromJson(_dataFolder, _filePattern);
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

        private async Task<List<MockRelation>> GetMockRelationsFromDB(string dbFilePath){
            try{
                List<MockRelation> outcome = null;
                using(var db = new LiteDatabase(dbFilePath))
                {
                    var col = db.GetCollection<MockRelation>("MockRelationList");
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