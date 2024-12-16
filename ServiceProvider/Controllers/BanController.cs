using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceProvider.Database;
using ServiceProvider.Helpers;
using ServiceProvider.Models;

namespace ServiceProvider.Controllers
{
	public class BanController : Controller
	{
		public const string UnauthorizedView = "../User/Unauthorized";

		IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<BanController> _logger;
		private AppDbContext _context;

		public BanController(IWebHostEnvironment webHostEnvironment, ILogger<BanController> logger, AppDbContext context)
		{
			_webHostEnvironment = webHostEnvironment;
			_logger = logger;
			_context = context;
		}

		[HttpGet]
		public IActionResult Index()
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			return View(_context.BannedUsers
				.Include(x => x.User)
				.OrderByDescending(x => x.BanTime)
				.ToList()
				);
		}

		[HttpGet]
		public IActionResult Add(int? id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			return View(id != null ?
				new BannedUser { UserId = id.Value }
				: null);
		}

		[HttpPost]
		public IActionResult PostAdd(BannedUser ban)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			if (ModelState.IsValid)
			{
				if (_context.Users.FirstOrDefault(x => x.Id == ban.UserId) != null)
				{
					if (_context.BannedUsers.FirstOrDefault(x => x.UserId ==  ban.UserId) == null)
					{
						ban.BanTime = DateTime.Now;

						_context.BannedUsers.Add(ban);
						_context.SaveChanges();

						return RedirectToAction("Index");
					}
					else
						ModelState.AddModelError(string.Empty, "This user is already banned.");
				}
				else
					ModelState.AddModelError(string.Empty, "User could not be found.");
			}
			return View("Add", ban);
		}

		[HttpGet]
		public IActionResult Delete(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			return View(_context.BannedUsers
				.Where(x => x.Id == id)
				.Include(x => x.User)
				.FirstOrDefault()
				);
		}

		[HttpPost]
		public IActionResult PostDelete(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			if (id != null && id > 0)
			{
				BannedUser? ban = _context.BannedUsers.FirstOrDefault(x => x.Id == id);
				if (ban != null)
				{
					_context.BannedUsers.Remove(ban);
					_context.SaveChanges();

					return RedirectToAction("Index");
				}
			}
			return View("Delete", null);
		}
	}
}
