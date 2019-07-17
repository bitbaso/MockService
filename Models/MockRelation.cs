using System;

namespace MockService.Models
{
    public class MockRelation
    {
        #region Public properties
        public Guid Id { get; set; }
        public Request Request { get; set; }

        public Response Response {get; set;}
        #endregion

        #region Constructors
        public MockRelation(){}
        public MockRelation(Request request, Response response){
            Request = request;
            Response = response;
        }
        #endregion
    }
}
