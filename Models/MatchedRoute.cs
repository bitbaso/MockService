using System.Collections.Generic;
namespace MockService.Models
{
    public class MatchedRoute
    {
        #region Public properties
        public bool IsMatch { get; set; }
        public Dictionary<string, string> MatchedValues { get; set; }
        #endregion
    }
}