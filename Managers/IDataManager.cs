using System;
using MockService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MockService.Managers
{
    public interface IDataManager
    {
         Task<List<MockRelation>> LoadMockRelations();
         List<MockRelation> GetPOSTMockRelations(List<MockRelation> MockRelations);
         List<MockRelation> GetGETMockRelations(List<MockRelation> MockRelations);
    }
}