using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Entities
{
	public enum CustomerClassification
	{
		[EnumMember(Value = "Regular Customer")]
		Regular,
		[EnumMember(Value = "Premium Customer")]
		Premium,
		[EnumMember(Value = "Employee Customer")]
		Employee
	}
}
