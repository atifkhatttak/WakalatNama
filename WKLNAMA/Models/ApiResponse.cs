using System.Net;

namespace WKLNAMA.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public object? Data { get; set; }

        public ApiResponse()
        {
            //Success = false;
            //Message = HttpStatusCode.BadRequest.ToString();
            //HttpStatusCode = HttpStatusCode.BadRequest;
            //Data =default(T);
        }
    }
}
