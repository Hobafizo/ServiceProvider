using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceProvider.Models
{
	public class ServiceRequest
	{
		[Display(Name = "Request ID")]
		public int Id { get; set; }

		public int UserId { get; set; }
		public IUser User { get; set; } = null!;

		[Required]
		public int ServiceId { get; set; }
		public Service Service { get; set; } = null!;

		[Display(Name = "About your request")]
		[MaxLength(300, ErrorMessage = "About field cannot exceed 100 characters.")]
		public string? About { get; set; }

		[Display(Name = "Time of request")]
		[ValidateNever]
		public DateTime RequestTime { get; set; }
	}
}
