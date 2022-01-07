using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentServiceListener.Gateways.Exceptions
{
    public class DocumentDbNotFoundException : Exception
    {
        public DocumentDbNotFoundException(Guid userId, Guid documentId)
            : base($"Document was not found in database [UserId: {userId}] [documentId: {documentId}]")
        {

        }
    }
}
