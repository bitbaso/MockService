namespace MockService.Models
{
    public class Request
    {
        #region Public properties
        public string Url { get; set; }

        public object PostData { get; set; }
        #endregion

        #region Constructors
        public Request(){}

        public Request(string url){
            if (!string.IsNullOrEmpty(url))
            {
                Url = url;
            }
            
            PostData = null;
        }
        public Request(string url, object postData){
            if (!string.IsNullOrEmpty(url))
            {
                Url = url;
                PostData = postData;
            }
        }
        #endregion
    }
}