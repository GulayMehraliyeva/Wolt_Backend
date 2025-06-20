using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels.Profile
{
	public class UpdateAddressVM
	{
		public string UserId { get; set; }

		[Required(ErrorMessage = "Ünvan boş ola bilməz.")]
		[StringLength(255, ErrorMessage = "Ünvan 255 simvoldan uzun ola bilməz.")]
		public string NewAddress { get; set; }
	}

}
