using System;
using System.Collections.Generic;

namespace XamarinEvolveSSLibrary
{
    public class UserResponse
    {
        public List<User> Users { get; set; }
        public Exception Exception { get; set; }
    }
}
