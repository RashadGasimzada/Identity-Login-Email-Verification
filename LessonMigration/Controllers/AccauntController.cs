using LessonMigration.Models;
using LessonMigration.ViewModels.Accaunt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace LessonMigration.Controllers
{
    public class AccauntController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        
        public AccauntController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //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
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            AppUser newUser = new AppUser()
            {
                FullName = registerVM.FullName,
                UserName = registerVM.Username,
                Email = registerVM.Email
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(registerVM);
            }

          //  await _signInManager.SignInAsync(newUser, false);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var link = Url.Action(nameof(VerifyEmail), "Accaunt", new { userId = newUser.Id, token = code },Request.Scheme,Request.Host.ToString());

            await  SendEmail(newUser.Email, link);
           
            return RedirectToAction(nameof(EmailVerification));
        }
        public async Task<IActionResult> VerifyEmail(string userId,string token)
        {
            if (userId== null || token == null) return BadRequest();

            AppUser user = await _userManager.FindByIdAsync(userId);

            if (user is null) return BadRequest();


           await _userManager.ConfirmEmailAsync(user, token);

            await _signInManager.SignInAsync(user, false);
            
            return RedirectToAction("Index","Home");
        }
        public IActionResult EmailVerification()
        {
            return View();
        }
        public async Task SendEmail(string emailAddress,string url)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("asgaraa@code.edu.az", "Asgar");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress(emailAddress, "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = $"<a href ={url}>Click Here</a>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            AppUser user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
               
            }

            if (user is null)
            {
                ModelState.AddModelError("", "Email or Password is Wrong");
                return View(loginVM);
            }

            if (!user.IsActivated)
            {
                ModelState.AddModelError("", "Contact with Admin");
                return View(loginVM);
            }

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Please Confirm Your Accaunt");
                    return View(loginVM);
                }
                ModelState.AddModelError("", "Email or Password is Wrong");
                return View(loginVM);
            }

            return RedirectToAction("Index", "Home");
        }

    }
}

