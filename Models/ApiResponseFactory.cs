using System.Linq;
using System.Threading.Tasks;

namespace MongoRepository.Models
{
    public static class ApiResponseFactory
    {
        public static ApiResponse Success()
        {
            return new ApiResponse
            {
                Success = true
            };
        }

        public static Task<ApiResponse<T>> Success<T>(T data)
        {
            return Task.FromResult<ApiResponse<T>>(new ApiResponse<T>
            {
                Success = true,
                Data = data
            });
        }

        public static Task<ApiResponse<T>> Warning<T>(T data, string[] messages)
        {
            return Task.FromResult<ApiResponse<T>>(new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Messages = messages.ToList()
            });
        }

        public static Task<ApiResponse> Error(string[] messages)
        {
            return Task.FromResult<ApiResponse>(new ApiResponse
            {
                Success = false,
                Messages = messages.ToList()
            });
        }

        public static Task<ApiResponse<T>> Error<T>(string[] messages)
        {
            return Task.FromResult<ApiResponse<T>>(new ApiResponse<T>
            {
                Success = false,
                Messages = messages.ToList()
            });
        }
    }
}