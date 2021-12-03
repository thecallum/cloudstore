using Amazon.Lambda.SQSEvents;
using DocumentServiceListener.Boundary.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.UseCase.Interfaces
{
    public interface IDeleteDirectoryUseCase : IMessageProcessing
    { }
}
