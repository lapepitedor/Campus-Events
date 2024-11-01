using Campus_Events.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Campus_Events.Controllers
{
    public class EventController : Controller
    {

        public IActionResult ListEvent()
        {
            return View();
        }

        public IActionResult userEvents()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }

    }
}
