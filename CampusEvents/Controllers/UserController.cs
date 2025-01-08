using Campus_Events.Models;
using Campus_Events.Persistence;
using Campus_Events.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Campus_Events.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> logger;
        private readonly IEventRepository eventRepository;
        private readonly IUserRegistration userRegistration;


        public UserController(ILogger<UserController> logger, IEventRepository eventRepository, IUserRegistration userRegistration)
        {
            this.logger = logger;
            this.eventRepository = eventRepository;
            this.userRegistration = userRegistration;

        }

        public IActionResult UserDashboard([FromQuery] int pg = 1)

        {
            // user infos
            var firstName = User.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            var lastName = User.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value;

            ViewBag.UserFullName = $"{firstName} {lastName}";

            const int pageSize = 9;
            var filter = new EventFilter
            {
                StartPage = pg,
                ItemsPerPage = pageSize
            };

            var result = eventRepository.GetAll(filter);
            return View(result);
        }


        public IActionResult ReturnToDashboard()
        {
            return RedirectToAction("UserDashboard");
        }

        public IActionResult RegisteredEvents()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Unable to identify the user.";
                return RedirectToAction("UserDashboard");
            }

            var registeredEvents = eventRepository.GetEventsForUser(Guid.Parse(userId)).OrderBy(e => e.Date).ToList();

            return View(new PagedResult<Event>
            {
                Items = registeredEvents
            });
        }

        // display event details

        [HttpGet("/User/EventDetails/{id}")]
        public IActionResult EventDetails(Guid id)
        {
            var eventItem = eventRepository.GetSingle(id);
            return View(eventItem);
        }


        [HttpPost]
        public IActionResult RegisterEvent(Guid eventId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Unable to identify the user.";
                return RedirectToAction("UserDashboard");
            }

            bool success = userRegistration.RegisterUserToEvent(eventId, Guid.Parse(userId));

            TempData["Message"] = success
                ? "Successful registration!"
                : "You have already registered or there are no more places available.";

            return RedirectToAction("RegisteredEvents");
        }


        [HttpPost]
        public IActionResult UnregisterEvent(Guid eventId)
        {
    
            var userId = User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Unable to identify the user.";
                return RedirectToAction("UserDashboard");
            }

         
            bool success = userRegistration.UnregisterUserFromEvent(eventId, Guid.Parse(userId));

            TempData["Message"] = success
                ? "Unsubscription successful!"
                : "You have not registered for this event.";

            return RedirectToAction("RegisteredEvents");
        }

    }
}
