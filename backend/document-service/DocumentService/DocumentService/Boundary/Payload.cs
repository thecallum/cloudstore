using DocumentService.Domain;

namespace DocumentService.Boundary
{
    public class Payload
    {
        public double Exp { get; set; }
        public User User { get; set; }
    }
}
