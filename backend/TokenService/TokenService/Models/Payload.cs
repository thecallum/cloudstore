using System;
using System.Collections.Generic;
using System.Text;

namespace TokenService.Models
{
    public class Payload
    {
        public double Exp { get; set; }
        public User User { get; set; }
    }
}
