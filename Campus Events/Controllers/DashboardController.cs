using Campus_Events.Models;
using Campus_Events.Persistence;
using Campus_Events.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Campus_Events.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> logger;
        private readonly IEventRepository eventRepository;
      

        public DashboardController(ILogger<DashboardController> logger, IEventRepository eventRepository)
        {
            this.logger = logger;
            this.eventRepository = eventRepository;
           
        }

        public IActionResult AdminDashboard([FromQuery] EventFilter filter)
        {
            var events = eventRepository.GetAll(filter);
           return View(events);
        }

        [HttpGet("/Dashboard/Edit/{id}")]
        public IActionResult Edit([FromRoute] Guid id)
        {
            var obj = eventRepository.GetSingle(id);
            if (obj == null)
                return NotFound();

            // Map Event to CreateOrEditEventViewModel
            var model = new CreateOrEditEventViewModel
            {
                ID = obj.ID,
                Title = obj.Title!,
                Date = obj.Date,
                Location = obj.Location!,
                Description = obj.Description,
                Type = obj.Type,
                Organizer = obj.Organizer!,
                TotalSeats = obj.TotalSeats,
               
            };

            return View(model);
        }




        [HttpGet("/Dashboard/New")]
        public IActionResult New()
        {
            // Initialisation d'un ViewModel vide pour la création
            var model = new CreateOrEditEventViewModel
            {
                ID = Guid.Empty, // Indique une création
                Date = DateTime.Now // Initialiser avec une date par défaut
            };

            return View("Edit", model);
        }

        [HttpPost("/Dashboard/Save/")]
        public IActionResult Save([FromForm] CreateOrEditEventViewModel model)
        {

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Validation failed for event: {Event}", model.Title);
                return View("Edit", model);
            }

            if (model.ID == Guid.Empty)
            {
                logger.LogInformation("Creating a new event: {Title}", model.Title);
                var newEvent = new Event
                {
                    ID = Guid.NewGuid(),
                    Title = model.Title,
                    Date = model.Date,
                    Location = model.Location,
                    Description = model.Description,
                    Type = model.Type,
                    Organizer = model.Organizer,
                    TotalSeats = model.TotalSeats
                };
                eventRepository.Add(newEvent);
            }
            else
            {
                logger.LogInformation("Updating event ID: {EventID}", model.ID);
                var existingEvent = eventRepository.GetSingle(model.ID);
                if (existingEvent == null)
                {
                    logger.LogWarning("Event not found: {EventID}", model.ID);
                    return NotFound();
                }

                existingEvent.Title = model.Title;
                existingEvent.Date = model.Date;
                existingEvent.Location = model.Location;
                existingEvent.Description = model.Description;
                existingEvent.Type = model.Type;
                existingEvent.Organizer = model.Organizer;
                existingEvent.TotalSeats = model.TotalSeats;
               
                eventRepository.Update(existingEvent);
            }


            // Redirects to AdminDashboard to display the list of events
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult Delete([FromRoute] Guid id)
        {
            var obj = eventRepository.GetSingle(id);
            if (obj == null)
                return NotFound();

            eventRepository.Delete(id);
           
            return RedirectToAction("AdminDashboard");
        }

       


    }
}
