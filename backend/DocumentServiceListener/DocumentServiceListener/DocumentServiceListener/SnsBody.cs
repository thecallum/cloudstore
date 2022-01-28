
namespace DocumentServiceListener
{
    public class SnsBody {
        public string TopicArn { get; set; }
        public string Message { get; set; }
    }
}