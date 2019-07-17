namespace MockService.Models
{
    public class Response
    {
        #region Public properties
        public string Content { get; set; }
        public int StatusCode { get; set; }
        public string ContentType { get; set; }
        
        #endregion

        #region Contructors
        public Response(){
            StatusCode = 200;
            ContentType = "text/plain";
        }
        #endregion
    }
}