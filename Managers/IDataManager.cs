using System;
using MockService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MockService.Managers
{
    public interface IDataManager
    {
         Task<List<MockRelation>> LoadMockRelations();
         List<MockRelation> GetMockRelations(List<MockRelation> MockRelations, string requestMethodType);
         Task<bool> AddMockRelation(MockRelation mockRelation);
    }
}