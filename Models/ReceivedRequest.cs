using Newtonsoft.Json;

namespace MockService.Models
{
    public class ReceivedRequest
    {
        #region Public properties
        public string Url { get; set; }

        public string Data { get; set; }
        #endregion

        #region Constructors
        public ReceivedRequest(string url){
            if (!string.IsNullOrEmpty(url))
            {
                Url = url;
            }
            
            Data = null;
        }
        public ReceivedRequest(string url, object data){
            if (!string.IsNullOrEmpty(url))
            {
                Url = url;
                if(data != null){
                    Data = JsonConvert.SerializeObject(data);
                }  
            }
        }
        #endregion
    }
}