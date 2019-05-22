namespace MockService.Models
{
    public class ReceivedRequest
    {
        #region Public properties
        public string Url { get; set; }

        public object PostData { get; set; }
        #endregion

        #region Constructors
        public ReceivedRequest(string url){
            if (!string.IsNullOrEmpty(url))
            {
                Url = url;
            }
            
            PostData = null;
        }
        public ReceivedRequest(string url, object postData){
            if (!string.IsNullOrEmpty(url))
            {
                Url = url;
                PostData = postData;
            }
        }
        #endregion
    }
}