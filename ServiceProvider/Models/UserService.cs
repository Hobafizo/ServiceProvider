using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceProvider.Models
{
    public class UserService
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public IUser User { get; set; } = null!;

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
	}
}
