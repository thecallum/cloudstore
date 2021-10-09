using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CDK.AWS.IAM;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Authorizer
{
    public class Function
    {
        public APIGatewayCustomAuthorizerResponse FunctionHandler(APIGatewayCustomAuthorizerRequest apigAuthRequest, ILambdaContext context)
        {
            var authStatus = true;
            var token = "___token___";

            return GenerateResponse(authStatus, token);
        }

        private APIGatewayCustomAuthorizerResponse GenerateResponse(bool authorized, string token)
        {



            return new APIGatewayCustomAuthorizerResponse
            {
                PrincipalID = token,
                PolicyDocument = new APIGatewayCustomAuthorizerPolicy
                {
                    Version = "2012-10-17",
                    Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
                    {
                        new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
                        {
                            Action = new HashSet<string>(new string[] { "execute-api:Invoke" }),
                            Effect = authorized ? "Allow" : "Deny",
                            Resource = new HashSet<string>(new string[] { "arn:aws:execute-api:eu-west-1:714664911966:8mcbqgx7eh/*/*" })
                        }
                    }
                },
                Context = new APIGatewayCustomAuthorizerContextOutput()
            };
        }
    }
}
