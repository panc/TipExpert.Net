using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using TipExpert.Core;
using TipExpert.Net.Models;

namespace TipExpert.Net.Controllers
{
    // TODO:
    // http://www.asp.net/web-api/overview/security/individual-accounts-in-web-api

    [Authorize]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserStore _userStore;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IUserStore userStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginDto model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.email, model.password, model.rememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userStore.FindUserByEmail(model.email, new CancellationToken());
                    return Json(Mapper.Map<UserDto>(user));
                }
            }

            // If we got this far, something failed, redisplay form
            return HttpBadRequest("Invalid login attemt.");
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup([FromBody] SignupDto model)
        {
            var message = "Invalid register information.";

            if (ModelState.IsValid)
            {
                var appUser = new ApplicationUser { UserName = model.name, Email = model.email };
                var result = await _userManager.CreateAsync(appUser, model.password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(appUser, isPersistent: false);

                    var user = await _userStore.FindUserByEmail(model.email, new CancellationToken());
                    return Json(Mapper.Map<UserDto>(user));
                }

                message = result.Errors.First().Description;
            }

            // If we got this far, something failed, redisplay form
            return HttpBadRequest(message);
        }

        [HttpGet]
        public async Task<UserDto[]> Get()
        {
            var user = await _userStore.GetAll();
            return Mapper.Map<UserDto[]>(user);
        }

        [HttpGet("friends")]
        public async Task<PlayerDto[]> GetFriends()
        {
            // return all users for now
            // we can load the friends of a user later on

            var user = await _userStore.GetAll();
            return user.Select(x => new PlayerDto
                {
                    userId = x.Id,
                    name = x.Name
                })
                .ToArray(); 
        }

        [HttpPut("{id}")]
        public async Task<UserDto> Put(Guid id, [FromBody]UserDto userDto)
        {
            var user = await _userStore.GetById(id);
            user.Name = userDto.name;
            user.Email = userDto.email;
            user.Role = userDto.role;

            await _userStore.SaveChangesAsync();

            return Mapper.Map<UserDto>(user);
        }

        [HttpPost("logout")]
//        [ValidateAntiForgeryToken]
        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        #region TODO

        //
        // POST: /Account/ExternalLogin
        [HttpPost("[Action]")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet("[Action]")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return _RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return View("Error");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Email);
                //return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
                return View("Error");
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost("[Action]")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (User.IsSignedIn())
            {
                //                return RedirectToAction(nameof(ManageController.Index), "Manage");
                return _RedirectToLocal(".");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    //return View("ExternalLoginFailure");
                    return View("Error");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return _RedirectToLocal(returnUrl);
                    }
                }
                _AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        #endregion

        #region Helpers

        private void _AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult _RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
