﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TokenService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}