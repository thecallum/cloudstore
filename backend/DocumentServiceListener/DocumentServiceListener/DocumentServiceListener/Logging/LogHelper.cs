using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentServiceListener.Logging
{
    public static class LogHelper
    {
        public static void LogUseCase(string useCaseName)
        {
            LambdaLogger.Log($"Calling UseCase [{useCaseName}]");
        }

        public static void LogGateway(string gatewayName, string gatewayMethodName)
        {
            LambdaLogger.Log($"Calling Gateway [{gatewayName}] [{gatewayMethodName}]");
        }
    }
}
