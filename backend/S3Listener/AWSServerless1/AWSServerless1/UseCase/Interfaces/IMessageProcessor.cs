using System.Threading.Tasks;
using static Amazon.S3.Util.S3EventNotification;

namespace AWSServerless1
{
    public interface IMessageProcessor
    {
        Task ProcessMessageAsync(S3Entity entity);
    }
}
