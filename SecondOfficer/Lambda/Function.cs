using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace SecondOfficer.Lambda
{
    public class Function
    {
        public virtual APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var verb = request.HttpMethod.ToUpperInvariant();
            var pathParts = request.Path.Split('/');
            var method = string.Empty;
            switch (verb)
            {
                //id is present
                case "GET" when (pathParts.Length == 6 && !string.IsNullOrEmpty(pathParts.Last())):
                    method = "GetById";
                    break;
                case "GET":
                    method = "GetAll";
                    break;
                case "POST":
                case "PUT":
                    method = "Save";
                    break;
                case "DELETE":
                    method = "Delete";
                    break;
            }



            // switch (verb)
            // {   
            //     case "GET":
            //         return Get(request, context);
            //     case "POST":
            //         return Post(request, context);
            //     case "PUT":
            //         return Put(request, context);
            //     case "DELETE":
            //         return Delete(request, context);
            //     default:
            //         return new APIGatewayProxyResponse
            //         {
            //             StatusCode = 405,
            //             Body = "Method not allowed"
            //         };
            // }
            return new APIGatewayProxyResponse
            {
                StatusCode = 405,
                Body = "Method not allowed"
            };
        }
    }
}
