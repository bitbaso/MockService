using Newtonsoft.Json;

namespace MockService.Models
{
    public class Request
    {
        #region Public properties
        public string Url { get; set; }

        public string Data { get; set; }

        public string Type { get; set; }
        #endregion

        #region Constructors
        public Request(){}

        public Request(string url){
            if (!string.IsNullOrEmpty(url))
            {
                Url = url;
            }
            
            Data = null;
        }
        public Request(string url, object data){
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