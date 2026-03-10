using System.Text.Json;

namespace StudentClub.API.Extensions
{
    public static class HttpResponseExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, object metadata)
        {
            var json = JsonSerializer.Serialize(metadata);

            response.Headers.Append("X-Pagination", json);
        }
    }
}
