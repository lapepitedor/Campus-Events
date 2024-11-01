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
    public class AccountController : Controller
    {
        private ILogger<AccountController> logger;
        private IUserRepository userRepository;
        private EmailService mailService;
        private PasswordHelper passwordHelper;

        public AccountController(ILogger<AccountController> logger, IUserRepository userRepository, EmailService mailService, PasswordHelper passwordHelper)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.mailService = mailService;
            this.passwordHelper = passwordHelper;
        }

        [HttpGet("/Account/Login/")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/Account/Authenticate/")]
        public async Task<IActionResult> Authenticate([FromForm] LoginViewModel login)
        {
            if (!ModelState.IsValid)
                return View("Login", login);

            if (string.IsNullOrEmpty(login.EMail))
            {
                ModelState.AddModelError(string.Empty, "L'email est requis.");
                return View("Login", login);
            }

            // Vérifiez si login.Password n'est pas nul ou vide
            if (string.IsNullOrEmpty(login.Password))
            {
                ModelState.AddModelError(string.Empty, "Le mot de passe est requis.");
                return View("Login", login);
            }

            var user = userRepository.FindByLogin(login.EMail, login.Password);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect."); // Message d'erreur pour l'utilisateur
                return View("Login", user);
            }

            var claims = user.ToClaims();
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Vérifiez si l'utilisateur est un administrateur
            if (AdminConfig.Admins.Any(admin => admin.Email.Equals(login.EMail, StringComparison.OrdinalIgnoreCase)))
            {               
                return RedirectToAction("AdminDashboard", "Dashboard");
               // return RedirectToAction("ListEvent", "UserDashboard");
            
            }
            else
            {
                // Rediriger vers le tableau de bord de l'utilisateur
                return RedirectToAction("UserDashboard", "User");
               
            }
        }

        [HttpGet("/Account/Logout/")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Account/Login");
        }

        [HttpGet("/Account/PasswordForgotten/")]
        public IActionResult PasswordForgotten()
        {
            return View();
        }

        [HttpPost("/Account/SendPasswordResetMail/")]
        public IActionResult SendPasswordResetMail([FromForm] PasswordForgottenViewModel pf)
        {
            if (!ModelState.IsValid)
                return View("PasswordForgotten", pf);

            // Vérifiez si pf.EMail n'est pas nul ou vide
            if (string.IsNullOrEmpty(pf.EMail))
            {
                ModelState.AddModelError(string.Empty, "L'email est requis.");
                return View("PasswordForgotten", pf);
            }

            var user = userRepository.FindByEmail(pf.EMail);
            if (user is not null)
                mailService.SendPasswortResetMail(user);

            return View();
        }

        [HttpGet("/Account/ResetPassword/{token}")]
        public IActionResult ResetPassword([FromRoute] string token)
        {
            var user = userRepository.FindByPasswordResetToken(token);
            if (user is null)
                return NotFound();

            return View(new PasswordResetViewModel() { Token = token });
        }

        [HttpPost("/Account/ResetPassword")]
        public IActionResult ResetPassword([FromForm] PasswordResetViewModel pr)
        {
            if (!ModelState.IsValid)
                return View("ResetPassword", pr);

            // Vérifiez si pr.Token n'est pas nul ou vide
            if (string.IsNullOrEmpty(pr.Token))
            {
                ModelState.AddModelError(string.Empty, "Le jeton de réinitialisation est requis.");
                return View("ResetPassword", pr);
            }

            var user = userRepository.FindByPasswordResetToken(pr.Token);
            if (user is null)
                return View("ResetPassword", pr);

            // Vérifiez si pr.Password n'est pas nul ou vide
            if (string.IsNullOrEmpty(pr.Password))
            {
                ModelState.AddModelError(string.Empty, "Le mot de passe est requis.");
                return View("ResetPassword", pr);
            }

            user.PasswordHash = passwordHelper.ComputeSha256Hash(pr.Password);
            user.PasswordResetToken = string.Empty;
            userRepository.Update(user);
            return Redirect("/");
        }

        [HttpGet("/Account/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("/Account/Register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Vérification de l'existence d'un utilisateur avec le même email
            var existingUser = userRepository.FindByEmail(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Un compte avec cet email existe déjà.");
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

            return Redirect( "/UserDashboard");
        }
    }
    
}
