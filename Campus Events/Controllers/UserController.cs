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

        public IActionResult RegisteredEvents(Guid userId)
        {
            var registeredEvents = eventRepository.GetEventsForUser(userId); // Récupère les événements inscrits pour cet utilisateur
            return View("UserDashboard", new PagedResult<Event> { Items = registeredEvents.ToList() }); // Utilise la vue UserDashboard pour affichage
        }


        // Action pour afficher les détails d'un événement
        [HttpGet("/User/EventDetails/{id}")]
        public IActionResult EventDetails(Guid id)
        {
            var eventItem = eventRepository.GetSingle(id);
            return View(eventItem);
        }

        // Action pour s'inscrire à un événement
        [HttpPost]
        public IActionResult Register(Guid eventId, Guid userId)
        {
            bool success = userRegistration.RegisterUserToEvent(eventId, userId);

            TempData["Message"] = success ? "Inscription réussie!" : "Vous êtes déjà inscrit ou il n'y a plus de places disponibles.";

           // return RedirectToAction("EventDetails", new { id = eventId });
            return RedirectToAction("UserDashboard");
        }

        // Action pour se désinscrire d'un événement
        [HttpPost]
        public IActionResult Unregister(Guid eventId, Guid userId)
        {
            bool success = userRegistration.UnregisterUserFromEvent(eventId,userId);
            TempData["Message"] = success ? "Désinscription réussie!" : "Vous n'êtes pas inscrit à cet événement.";

            //return RedirectToAction("EventDetails", new { id = eventId });
            return RedirectToAction("UserDashboard");
        }

    }
}
