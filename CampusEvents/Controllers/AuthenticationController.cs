using Campus_Events.Misc;
using Campus_Events.Persistence;
using Campus_Events.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Campus_Events.Models;

namespace Campus_Events.Controllers
{
    public class AuthenticationController : Controller
    {
        private ILogger<AuthenticationController> logger;
        private IUserRepository userRepository;
        private EmailService mailService;
        private PasswordHelper passwordHelper;

        public AuthenticationController(ILogger<AuthenticationController> logger, IUserRepository userRepository, EmailService mailService, PasswordHelper passwordHelper)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.mailService = mailService;
            this.passwordHelper = passwordHelper;
        }

        [HttpGet("/Authentication/Login/")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/Authentication/Authenticate/")]
        public async Task<IActionResult> Authenticate([FromForm] LoginViewModel login)
        {
            if (!ModelState.IsValid)
                return View("Login", login);

            if (string.IsNullOrEmpty(login.EMail))
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return View("Login", login);
            }

            if (string.IsNullOrEmpty(login.Password))
            {
                ModelState.AddModelError(string.Empty, "The password is required.");
                return View("Login", login);
            }

            var user = userRepository.FindByLogin(login.EMail, login.Password);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Incorrect email address or password."); 
                return View("Login", user);
            }

            var claims = user.ToClaims();
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            if (user.IsAdmin == true) 
            {
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
            else
            {
                return RedirectToAction("UserDashboard", "User");
            }
        }

        [HttpGet("/Authentication/Logout/")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Authentication/Login");
        }

        [HttpGet("/Authentication/PasswordForgotten/")]
        public IActionResult PasswordForgotten()
        {
            return View();
        }

        //[HttpPost("/Authentication/SendPasswordResetMail/")]
        //public IActionResult SendPasswordResetMail([FromForm] PasswordForgottenViewModel pf)
        //{
        //    if (!ModelState.IsValid)
        //        return View("PasswordForgotten", pf);

        //    if (string.IsNullOrEmpty(pf.EMail))
        //    {
        //        ModelState.AddModelError(string.Empty, "L'email est requis.");
        //        return View("PasswordForgotten", pf);
        //    }

        //    var user = userRepository.FindByEmail(pf.EMail);

        //    if (user == null)
        //    {
        //        ModelState.AddModelError(string.Empty, "Aucun utilisateur trouvé avec cet e-mail.");
        //        return View("PasswordForgotten", pf);
        //    }

        //    // Envoi du mail de réinitialisation du mot de passe
        //    mailService.SendPasswortResetMail(pf.EMail); // Passez l'e-mail

        //    TempData["Message"] = "Un e-mail de réinitialisation du mot de passe a été envoyé, si un compte correspondant existe.";
        //    return View("PasswordForgotten", pf);
        //}


        [HttpPost("/Authentication/SendPasswordResetMail/")]
        public IActionResult SendPasswordResetMail([FromForm] PasswordForgottenViewModel pf)
        {
            if (!ModelState.IsValid)
                return View("PasswordForgotten", pf);

            var user = userRepository.FindByEmail(pf.EMail);
            if (user is not null)
                mailService.SendPasswortResetMail(user);

            return View();
        }

        [HttpGet("/Authentication/ResetPassword/{token}")]
        public IActionResult ResetPassword([FromRoute] string token)
        {
            var user = userRepository.FindByPasswordResetToken(token);
            if (user is null)
                return NotFound();

            return View(new PasswordResetViewModel() { Token = token });
        }

        [HttpPost("/Authentication/ResetPassword")]
        public IActionResult ResetPassword([FromForm] PasswordResetViewModel pr)
        {
            if (!ModelState.IsValid)
                return View("ResetPassword", pr);

            // Vérifiez si pr.Token n'est pas nul ou vide
            if (string.IsNullOrEmpty(pr.Token))
            {
                ModelState.AddModelError(string.Empty, "The reset token is required.");
                return View("ResetPassword", pr);
            }

            var user = userRepository.FindByPasswordResetToken(pr.Token);
            if (user is null)
                return View("ResetPassword", pr);

            // Vérifiez si pr.Password n'est pas nul ou vide
            if (string.IsNullOrEmpty(pr.Password))
            {
                ModelState.AddModelError(string.Empty, "Password required.");
                return View("ResetPassword", pr);
            }

            user.PasswordHash = passwordHelper.ComputeSha256Hash(pr.Password);
            user.PasswordResetToken = string.Empty;
            userRepository.Update(user);
            return Redirect("/");
        }

        [HttpGet("/Authentication/Register")]

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("/Authentication/Register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check that a user with the same email address exists
            var existingUser = userRepository.FindByEmail(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                return View(model);
            }

            // Création de l'utilisateur
            var user = new User
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                EMail = model.Email,
                PasswordHash = passwordHelper.ComputeSha256Hash(model.Password),
            };

            userRepository.Add(user);

            // Connexion après inscription
            var claims = user.ToClaims();
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Redirect("/Authentication/Login");
        }
    }
    
}
