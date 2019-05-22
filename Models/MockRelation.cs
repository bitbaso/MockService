namespace MockService.Models
{
    public class MockRelation
    {
        #region Public properties
        public Request Request { get; set; }

        public Response Response {get; set;}
        #endregion

        #region Constructors
        public MockRelation(Request request, Response response){
            Request = request;
            Response = response;
        }
        #endregion
    }
}
