using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceProvider.Database;
using ServiceProvider.Helpers;
using ServiceProvider.Models;

namespace ServiceProvider.Controllers
{
    public class ServiceController : Controller
    {
		public const string UnauthorizedView = "../User/Unauthorized";

		IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<ServiceController> _logger;
		private AppDbContext _context;

		public ServiceController(IWebHostEnvironment webHostEnvironment, ILogger<ServiceController> logger, AppDbContext context)
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
            return View(_context.Services.ToList());
        }

		[HttpGet]
		public IActionResult Add()
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);
			return View();
		}

		[HttpPost]
		public IActionResult PostAdd(Service service)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			if (ModelState.IsValid)
			{
				_context.Services.Add(service);
				_context.SaveChanges();

				return RedirectToAction("Index");
			}
			return View("Add", service);
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);
			return View(_context.Services.FirstOrDefault(x => x.Id == id));
		}

		[HttpPost]
		public IActionResult PostEdit(Service service)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			if (ModelState.IsValid)
			{
				Service? srv = _context.Services.FirstOrDefault(x => x.Id == service.Id);
				if (srv != null)
				{
					srv.Edit(service);
					_context.SaveChanges();

					return RedirectToAction("Index");
				}
				else
					ModelState.AddModelError(string.Empty, "Service could not be found.");
			}
			return View("Edit", service);
		}

		[HttpGet]
		public IActionResult Delete(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);
			return View(_context.Services.FirstOrDefault(x => x.Id == id));
		}

		[HttpPost]
		public IActionResult PostDelete(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			Service? srv = _context.Services.FirstOrDefault(x => x.Id == id);
			if (srv != null)
			{
				_context.Services.Remove(srv);
				_context.SaveChanges();

				return RedirectToAction("Index");
			}
			return View("Delete", null);
		}
	}
}
