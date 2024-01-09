using System;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class csUser : IdentityUser<Guid>
    {
		public csUser()
		{
		}
	}
}

