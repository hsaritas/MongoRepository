using System.Collections.Generic;

namespace MongoRepository.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }
}