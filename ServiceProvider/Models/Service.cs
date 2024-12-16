using System.ComponentModel.DataAnnotations;

namespace ServiceProvider.Models
{
    public class Service
    {
		[Display(Name = "Service ID")]
        public int Id { get; set; }

        [Display(Name = "Name of service")]
        [Required]
		[MinLength(2, ErrorMessage = "Service name cannot be less than 2 characters.")]
		[MaxLength(20, ErrorMessage = "Service name cannot exceed 20 characters.")]
		public string Name { get; set; }

		[Display(Name = "Description of service")]
		[Required]
		[MinLength(5, ErrorMessage = "Service description cannot be less than 5 characters.")]
		[MaxLength(50, ErrorMessage = "Service description cannot exceed 50 characters.")]
		public string Description { get; set; }

		public void Edit(Service service)
		{
			Name = service.Name;
			Description = service.Description;
		}
    }
}
