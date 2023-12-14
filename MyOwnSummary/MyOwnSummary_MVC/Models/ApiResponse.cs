using System.Net;

namespace MyOwnSummary_MVC.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; } = false;

        public object Result { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}
