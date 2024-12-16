using ServiceProvider.Database;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ServiceProvider.Models
{
    public class IUser
    {
		[NotMapped]
		public const string ProfilePicturePath    = @"img\profile",
							ProfilePictureNoImage = @"img\profile\no_image.png";

		[Display(Name = "User ID")]
		public int Id { get; set; }

        [Display(Name = "Email address")]
		[Required]
		[EmailAddress]
		[MinLength(5, ErrorMessage = "Email cannot be less than 5 characters.")]
		[MaxLength(30, ErrorMessage = "Email cannot exceed 30 characters.")]
		public string Email { get; set; } = null!;

		[Display(Name = "Password")]
		[Required]
		[MinLength(6, ErrorMessage = "Password cannot be less than 6 characters.")]
		[MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
		public string Password { get; set; } = null!;

		[Display(Name = "User name")]
		[Required]
		[MaxLength(30, ErrorMessage = "User name cannot exceed 30 characters.")]
		public string Name { get; set; } = null!;

		[Display(Name = "Date of Birth")]
		[Required(ErrorMessage = "Please enter a valid date of birth.")]
		public DateTime? BirthDate { get; set; }

		[Display(Name = "Profile Picture")]
		[ValidateNever]
		public string? ProfilePicture { get; set; }

		[Display(Name = "Administrator")]
        [DefaultValue(false)]
        public bool IsAdmin { get; set; }

		[Display(Name = "Banned")]
		public BannedUser? BannedUser { get; set; }

		public ICollection<UserService> Services { get; } = new List<UserService>();
		

		[NotMapped]
		public string ProfilePictureImage { get { return !string.IsNullOrEmpty(ProfilePicture) ? ProfilePicture : ProfilePictureNoImage; } }

		[NotMapped]
		public string ProfilePictureState { get { return !string.IsNullOrEmpty(ProfilePicture) ? "" : "disabled"; } }


		public void Edit(IUser user)
		{
			Name = user.Name;
			BirthDate = user.BirthDate;
		}

        public void HashPassword()
        {
            Password = GetPassswordHash(Password);
        }

		public bool SetImage(string rootpath, IFormFile? ppimg)
		{
			if (ppimg != null)
			{
				ProfilePicture = string.Format(@"{0}\{1}{2}", ProfilePicturePath, Guid.NewGuid(), Path.GetExtension(ppimg.FileName));

				using (FileStream stream = new FileStream(string.Format(@"{0}\{1}", rootpath, ProfilePicture), FileMode.Create))
				{
					ppimg.CopyTo(stream);
					stream.Close();
				}
				return true;
			}

			ProfilePicture = null;
			return false;
		}

		public bool DeleteImage(string rootpath)
		{
			if (!string.IsNullOrEmpty(ProfilePicture))
			{
				File.Delete(string.Format(@"{0}\{1}", rootpath, ProfilePicture));
				ProfilePicture = null;
				return true;
			}
			return false;
		}

		public bool ReplaceImage(string rootpath, IFormFile? ppimg)
		{
			if (ppimg != null)
			{
				DeleteImage(rootpath);
				SetImage(rootpath, ppimg);
				return true;
			}
			return false;
		}

		public bool IsSamePassword(string password)
        {
            return Password.Equals(GetPassswordHash(password), StringComparison.OrdinalIgnoreCase);
        }

		public static string GetPassswordHash(string password)
		{
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				return Convert.ToHexString(hashBytes);
			}
		}
	}
}
