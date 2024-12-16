using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ServiceProvider.Database;
using ServiceProvider.Helpers;
using ServiceProvider.Models;

namespace ServiceProvider.Controllers
{
	public class RequestController : Controller
	{
		public const string UnauthorizedView = "../User/Unauthorized",
							LoggedInView = "../User/LoginRequired";

		IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<RequestController> _logger;
		private AppDbContext _context;

		public RequestController(IWebHostEnvironment webHostEnvironment, ILogger<RequestController> logger, AppDbContext context)
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

			return View(_context.ServiceRequests
				.Include(x => x.User)
				.Include(x => x.Service)
				.ToList()
				);
		}

		[HttpGet]
		public IActionResult View(int id)
		{
			if (!SessionHelper.IsLoggedIn(this, true))
				return View(UnauthorizedView);

			return View(_context.ServiceRequests
				.Where(x => x.Id == id)
				.Include(x => x.User)
				.Include(x => x.Service)
				.FirstOrDefault());
		}

		[HttpGet]
		public IActionResult Add()
		{
			if (!SessionHelper.IsLoggedIn(this))
				return View(LoggedInView);

			ViewBag.Services = new SelectList(_context.Services.ToList(), "Id", "Name");
			return View();
		}

		[HttpPost]
		public IActionResult PostAdd(ServiceRequest request)
		{
			IUser? user = SessionHelper.GetSession(this);
			if (user == null)
				return View(LoggedInView);

			if (
				ModelState.GetValidationState("ServiceId") == ModelValidationState.Valid &&
                ModelState.GetValidationState("About") == ModelValidationState.Valid
				)
			{
				if (_context.Services.FirstOrDefault(x => x.Id == request.ServiceId) != null)
				{
					request.UserId = user.Id;
					request.RequestTime = DateTime.Now;

                    if (request.About != null)
                        request.About = request.About.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\t", "\\t");

                    _context.ServiceRequests.Add(request);
					_context.SaveChanges();

					return RedirectToAction("Submitted");
				}
				else
					ModelState.AddModelError(string.Empty, "Service could not be found.");
			}
			return View("Add", request);
		}

		[HttpGet]
		public IActionResult Submitted()
		{
			if (!SessionHelper.IsLoggedIn(this))
				return View(LoggedInView);
			return View();
		}
	}
}
