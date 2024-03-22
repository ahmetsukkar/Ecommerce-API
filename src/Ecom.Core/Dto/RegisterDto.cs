using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Dto
{
	public class RegisterDto
	{
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(3,ErrorMessage = "Min length is 3 character")]
        public string DisplayName { get; set; }

        [Required]
        public string Role { get; set; } = "Customer";

        [Required]
		public string Password { get; set; }
    }
}
