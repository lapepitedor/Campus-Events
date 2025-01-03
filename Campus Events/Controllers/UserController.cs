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

        public IActionResult UserDashboard([FromQuery] Guid userId, EventFilter filter)
        {
            var events = eventRepository.GetAll(filter);
            ViewData["UserId"] = userId; // Passez userId à la vue
            return View(events);
        }

        public IActionResult ReturnToDashboard()
        {
            return RedirectToAction("UserDashboard");
        }

        public IActionResult RegisteredEvents(Guid userId)
        {
            var registeredEvents = eventRepository.GetEventsForUser(userId); // Récupère les événements inscrits pour cet utilisateur
                                                                            
            return View("RegisteredEvents", new PagedResult<Event> { Items = registeredEvents.ToList() });
        }

        // display event details
        [HttpGet("/User/EventDetails/{id}")]
        public IActionResult EventDetails(Guid id, string returnUrl = null)
        {
            var eventItem = eventRepository.GetSingle(id);
            ViewData["ReturnUrl"] = returnUrl ?? Url.Action("UserDashboard");
            return View(eventItem);
        }

        [HttpPost]
        public IActionResult Register(Guid eventId, Guid userId)
        {
            bool success = userRegistration.RegisterUserToEvent(eventId, userId);

            TempData["Message"] = success ? "Successful registration!" : "You have already registered or there are no more places available.";

            return RedirectToAction("UserDashboard");
        }

   
        [HttpPost]
        public IActionResult Unregister(Guid eventId, Guid userId)
        {
            bool success = userRegistration.UnregisterUserFromEvent(eventId,userId);
            TempData["Message"] = success ? "Unsubscription successful!" : "You have not registered for this event.";
            return RedirectToAction("UserDashboard");
        }

       

    }
}
