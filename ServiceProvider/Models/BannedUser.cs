using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceProvider.Models
{
    public class BannedUser
    {
        public int Id { get; set; }

        [Display(Name = "User ID")]
        [Required]
        public int UserId { get; set; }

		[ValidateNever]
		public IUser User { get; set; } = null!;

        [Display(Name = "Reason for ban")]
		[Required]
		[MinLength(5, ErrorMessage = "Ban reason cannot be less than 5 characters.")]
		[MaxLength(50, ErrorMessage = "Ban reason cannot exceed 50 characters.")]
		public string BanReason { get; set; }

        [Display(Name = "Time of ban")]
        [ValidateNever]
        public DateTime BanTime { get; set; }
    }
}
