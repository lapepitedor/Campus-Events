using Campus_Events.Models;
using Campus_Events.Persistence;
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

            return View(obj);
        }


        [HttpGet("/Dashboard/New")]
        public IActionResult New()
        {
            return View("Edit", new Event());
        }

        [HttpPost("/Dashboard/Save/")]
        public IActionResult Save([FromForm] Event obj)
        {
            if (obj == null)
                return Redirect("/");

            if (!ModelState.IsValid)
                return View("Edit", obj);

            if (obj.ID == Guid.Empty) // Ajout si ID est vide
            {
                obj.ID = Guid.NewGuid(); // Crée un nouvel ID pour l'ajout
                eventRepository.Add(obj);
            }
            else
            {
                eventRepository.Update(obj); // Met à jour si ID existe
            }

            // Redirige vers AdminDashboard pour afficher la liste des événements
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult Delete([FromRoute] Guid id)
        {
            var obj = eventRepository.GetSingle(id);
            if (obj == null)
                return NotFound();

            eventRepository.Delete(id);
            // Redirige vers AdminDashboard pour afficher la liste des événements
            return RedirectToAction("AdminDashboard");
        }

       


    }
}
