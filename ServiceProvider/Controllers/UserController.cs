using Microsoft.AspNetCore.Mvc;
using ServiceProvider.Database;
using ServiceProvider.Models;
using Newtonsoft.Json.Converters;
using ServiceProvider.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServiceProvider.Controllers
{
	public class UserController : Controller
	{
		public const string  NotFoundView     = "../Home/NotFound",
							 LoggedInView     = "LoginRequired",
							 LoggedOutView    = "LogoutRequired",
							 UnauthorizedView = "Unauthorized";

		IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<UserController> _logger;
		private AppDbContext _context;

		public UserController(IWebHostEnvironment webHostEnvironment, ILogger<UserController> logger, AppDbContext context)
		{
			_webHostEnvironment = webHostEnvironment;
			_logger = logger;
			_context = context;
		}

		[HttpGet]
		public IActionResult Login()
		{
			if (SessionHelper.IsLoggedIn(this))
				return View(LoggedOutView);
			return View();
		}

		[HttpPost]
		public IActionResult PostLogin(IUser info)
		{
			if (SessionHelper.IsLoggedIn(this))
				return View(LoggedOutView);
			else if (!string.IsNullOrEmpty(info.Password) && !Regex.IsMatch(info.Password.ToLower(), "^[a-zA-Z0-9]*$"))
				ModelState.AddModelError("Password", "Password must contain only alphanumeric characters.");

			if (ModelState.GetValidationState("Email") == ModelValidationState.Valid &&
				ModelState.GetValidationState("Password") == ModelValidationState.Valid)
			{
				IUser? user = _context.Users
					.Where(x =>
								x.Email.ToLower() == info.Email.ToLower() &&
								x.Password.ToLower() == IUser.GetPassswordHash(info.Password).ToLower()
						  )
					.Include(x => x.Services)
						.ThenInclude(x => x.Service)
					.Include(x => x.BannedUser)
					.FirstOrDefault();

				if (user != null)
				{
					if (user.BannedUser == null)
					{
						SessionHelper.SetSession(this, user);
						return RedirectToAction("Index");
					}
					else
						ModelState.AddModelError(string.Empty, string.Format("This user is banned for {0}.", user.BannedUser.BanReason));
				}
				else
					ModelState.AddModelError(string.Empty, "Email address or password was incorrect, please try again.");
			}
			return View("Login");
		}

		[HttpGet]
		public IActionResult Register()
		{
			if (SessionHelper.IsLoggedIn(this))
				return View(LoggedOutView);
			return View();
		}

		[HttpPost]
		public IActionResult PostRegister(IUser user, string pwconfirm, IFormFile? ppimg)
		{
			if (SessionHelper.IsLoggedIn(this))
				return View(LoggedOutView);
			else if (!string.IsNullOrEmpty(user.Password) && !Regex.IsMatch(user.Password.ToLower(), "^[a-zA-Z0-9]*$"))
				ModelState.AddModelError("Password", "Password must contain only alphanumeric characters.");
			else if (string.IsNullOrEmpty(pwconfirm) || (!string.IsNullOrEmpty(user.Password) && user.Password != pwconfirm))
				ModelState.AddModelError(string.Empty, "Please confirm your password correctly.");

			if (ModelState.IsValid)
			{
				if (_context.Users.FirstOrDefault(x => x.Email.ToLower() == user.Email.ToLower()) == null)
				{
					user.SetImage(_webHostEnvironment.WebRootPath, ppimg);
					user.HashPassword();

					_context.Users.Add(user);
					_context.SaveChanges();

					SessionHelper.SetSession(this, user);
					return RedirectToAction("Index");
				}
				else
					ModelState.AddModelError(string.Empty, "This email address is already in use.");
			}
			return View("Register");
		}

		[HttpGet]
		public IActionResult Index()
		{
			IUser? user = SessionHelper.GetSession(this);
			if (user == null)
				return RedirectToAction("Login");

			if (user.IsAdmin)
			{
				ViewBag.UserCount = _context.Users.Count();
				ViewBag.ServiceCount = _context.Services.Count();
				ViewBag.RequestCount = _context.ServiceRequests.Count();
				ViewBag.BannedUserCount = _context.BannedUsers.Count();
			}
			return View(user);
		}

		[HttpGet]
		public IActionResult Profile(int? id)
		{
			IUser? user = SessionHelper.GetSession(this);
			if (user == null)
				return View(LoggedInView);

			return View(
				user.IsAdmin && id != null && id > 0
				? _context.Users.FirstOrDefault(x => x.Id == id) //admin user lookup
				: user //normal user profile
				);
		}

		[HttpPost]
		public IActionResult EditProfile(IUser info, IFormFile? ppimg)
		{
			IUser? user = SessionHelper.GetSession(this);
			if (user == null)
				return View(LoggedInView);

			if (
				ModelState.GetValidationState("Name") == ModelValidationState.Valid &&
				ModelState.GetValidationState("BirthDate") == ModelValidationState.Valid
				)
			{
				IUser? target = null;
				if (info.Id > 0 && (info.Id == user.Id || user.IsAdmin))
					target = _context.Users.FirstOrDefault(x => x.Id == info.Id);

				if (target != null)
				{
					target.Edit(info);
					if (ppimg != null)
						target.ReplaceImage(_webHostEnvironment.WebRootPath, ppimg);

					info = target;
					if (info.Id == user.Id)
						SessionHelper.SetSession(this, target);

					_context.SaveChanges();
					ViewBag.Success = true;
				}
				else
					ModelState.AddModelError(string.Empty, "User could not be found.");
			}
			return View("Profile", info);
		}

		[HttpGet]
		public IActionResult DeletePP(int id)
		{
			IUser? user = SessionHelper.GetSession(this);
			if (user == null)
				return View(LoggedInView);

			IUser? target = null;
			if (id != null & id > 0 && (user.Id == id || user.IsAdmin))
				target = _context.Users.FirstOrDefault(x => x.Id == id);

			if (target != null)
			{
				if (target.DeleteImage(_webHostEnvironment.WebRootPath))
				{
					if (id == user.Id)
						SessionHelper.SetSession(this, target);
					_context.SaveChanges();

					ViewBag.Success = true;
				}
				else
					ModelState.AddModelError(string.Empty, "There's no profile picture to be deleted.");
			}
			else
				ModelState.AddModelError(string.Empty, "User could not be found.");

			return View("Profile", target);
		}

		[HttpGet]
		public IActionResult Admin()
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			return View("Admin/Index", _context.Users
				.Include(x => x.BannedUser)
				.Include(x => x.Services)
				.ToList());
		}

		[HttpGet]
		public IActionResult SetAdmin(int id, bool to)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			IUser? user = _context.Users.FirstOrDefault(x => x.Id == id);
			if (user != null)
			{
				user.IsAdmin = to;
				_context.SaveChanges();

				return RedirectToAction("Admin");
			}
			return View(NotFoundView);
		}

		[HttpGet]
		public IActionResult Service(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			return View("Service/Index", _context.Users
				.Where(x => x.Id == id)
				.Include(x => x.Services)
					.ThenInclude(x => x.Service)
				.FirstOrDefault());
		}

		[HttpGet]
		public IActionResult AddService(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			IUser? user = _context.Users.FirstOrDefault(x => x.Id == id);
			if (user != null)
				ViewBag.Services = new SelectList(_context.Services, "Id", "Name");

			return View("Service/Add", new UserService
			{
				UserId = id
			});
		}

		[HttpPost]
		public IActionResult PostAddService(UserService service)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			IUser? user = _context.Users.FirstOrDefault(x => x.Id == service.UserId);
			if (user != null)
			{
				if (_context.Services.FirstOrDefault(x => x.Id == service.ServiceId) != null)
				{
					_context.UserServices.Add(service);
					_context.SaveChanges();

					return RedirectToAction("Service", new { id = service.UserId });
				}
				else
					ModelState.AddModelError(string.Empty, "Service could not be found.");
			}
			else
				ModelState.AddModelError(string.Empty, "User could not be found.");
			return View("Service/Add", service);
		}

		[HttpGet]
		public IActionResult DeleteService(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			return View("Service/Delete", _context.UserServices
				.Where(x => x.Id == id)
				.Include(x => x.User)
				.Include(x => x.Service)
				.FirstOrDefault());
		}

		[HttpPost]
		public IActionResult PostDeleteService(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			UserService? service = _context.UserServices.FirstOrDefault(x => x.Id == id);
			if (service != null)
			{
				int userid = service.UserId;

				_context.UserServices.Remove(service);
				_context.SaveChanges();

				return RedirectToAction("Service", new { id = service.UserId });
			}
			else
				ModelState.AddModelError(string.Empty, "Service could not be found.");
			return View("Service/Add", service);
		}

		[HttpGet]
        public IActionResult Logout()
        {
            if (!SessionHelper.IsLoggedIn(this))
                return View(LoggedInView);

			SessionHelper.DeleteSession(this);
            return RedirectToAction("Index", "Home");
        }
	}
}
