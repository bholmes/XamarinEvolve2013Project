using System;
using ServiceStack.DataAnnotations;
using ServiceStack.ServiceHost;

namespace XamarinEvolveSSLibrary
{
    [Alias("evolve_users")]
    [Route("/user")]
    [Route("/user/{username}")]
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string fullname { get; set; }
        public string city { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public string avatar { get; set; }

        public User() { }

        public override string ToString()
        {
            return string.Format("id='{0}', username='{1}', fullname='{2}', city='{3}', email='{4}', phone='{5}', password='{6}', avatar='{7}'",
                id, username, fullname, city, email, phone, password, avatar);
        }
    }
}
