using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Dto
{
	public class UserDto
	{
		public string Id { get; set; }
		public string DisplayName { get; set; }
		public string Email { get; set; }
		public string Roles { get; set; }
		public string Token { get; set; }
	}

	public class User
	{
		public string Id { get; set; }
		public string DisplayName { get; set; }
		public string Email { get; set; }
	}
}
