using System;
using ServiceStack.DataAnnotations;
using ServiceStack.ServiceHost;

namespace XamarinEvolveSSLibrary
{
#if !IOS
    [Alias("evolve_users")]
    [Route("/User")]
    [Route("/User/{UserName}")]
#endif
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }

        public User() { }

        public override string ToString()
        {
            return string.Format("Id='{0}', UserName='{1}', FullName='{2}', City='{3}', Email='{4}', Phone='{5}', Password='{6}', Avatar='{7}', Company='{8}', Title='{9}'",
                Id, UserName, FullName, City, Email, Phone, Password, Avatar, Company, Title);
        }

		public bool IsAnonymousUser ()
		{
			return string.IsNullOrEmpty (UserName);
		}

		public bool IsLocalUser ()
		{
			User localUser = Engine.Instance.UserAccess.GetCurrentUser ();
			
			if (string.IsNullOrEmpty (localUser.UserName))
			{
				if (string.IsNullOrEmpty (UserName))
					return true;
			}
			else
				return localUser.UserName.Equals (UserName);
			
			return false;
		}
    }
}
