using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hotels.Utilities
{
    public class DataResponse
    {
        public DataResponse(bool isSuccess, string msg, object data= null)
        {
            this.IsSuccess = isSuccess;
            this.Msg = msg;
            this.Data = data;
        }
        public bool IsSuccess { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }

    }

}
