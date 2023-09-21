using CourseUdemy.Helpers;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace CourseUdemy.Extensions
{
    public static class HttpExtenstion
    {
        public static void AddPaginationHeader ( this HttpResponse response, PagintionHelper header )
        {
            var jsonOptions=new JsonSerializerOptions{ PropertyNamingPolicy=JsonNamingPolicy.CamelCase};
            response.Headers.Add ("Pagination",JsonSerializer.Serialize(header, jsonOptions));
            response.Headers.Add ("Access-Control-Expose-Headers","Pagination");
        }
    }
}
