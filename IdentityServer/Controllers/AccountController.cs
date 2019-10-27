using IdentityServer.Extensions;
using IdentityServer.Models.AccountViewModels;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Battles.Models;
using NETCore.MailKit.Core;
using TrickingRoyal.Database;

namespace IdentityServer.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            AppDbContext ctx,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IEmailService emailService,
            ILogger<AccountController> logger)
        {
            _ctx = ctx;
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult BackToHome() => RedirectToAction("Index", "Home");

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            return _signInManager.IsSignedIn(HttpContext.User)
                       ? BackToHome()
                       : View(new LoginViewModel()
                       {
                           ExternalAuth = await HttpContext.GetExternalProvidersAsync(),
                           ReturnUrl = returnUrl
                       });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                return BackToHome();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null)
                    {
                        model.ExternalAuth = await HttpContext.GetExternalProvidersAsync();
                        ModelState.AddModelError(string.Empty, "Incorrect username or password.");
                        return View(model);
                    }

                    var result =
                        await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe,
                                                                 lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        return Redirect(_interaction.IsValidReturnUrl(model.ReturnUrl) ? model.ReturnUrl : "~/");
                    }

                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(SendCode), new {model.ReturnUrl, model.RememberMe});
                    }

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning(2, "User account locked out.");
                        return View("Lockout");
                    }

                    model.ExternalAuth = await HttpContext.GetExternalProvidersAsync();
                    ModelState.AddModelError(string.Empty, "Incorrect username or password.");
                    return View(model);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception during login attempt for {0}", model.Email);
                }
            }

            model.ExternalAuth = await HttpContext.GetExternalProvidersAsync();
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            return _signInManager.IsSignedIn(HttpContext.User)
                       ? BackToHome()
                       : View(new RegisterViewModel
                       {
                           ReturnUrl = returnUrl
                       });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                return BackToHome();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser(model.Email);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _ctx.UserInformation.Add(new UserInformation
                {
                    Id = user.Id,
                    DisplayName = model.NickName,
                });

                await _ctx.SaveChangesAsync();
                await _signInManager.SignInAsync(user, true);
                _logger.LogInformation(3, "User created a new account with password.");

                return Redirect(model.ReturnUrl);
            }

            AddErrors(result);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrEmpty(logout.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(logout.PostLogoutRedirectUri);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new {ReturnUrl = returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result =
                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                                                              isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new {ReturnUrl = returnUrl});
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
            {
                Email = email,
                ReturnUrl = returnUrl,
                LoginProvider = info.LoginProvider
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginFailure");
            }

            var user = new ApplicationUser(model.Email)
            {
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    _ctx.UserInformation.Add(new UserInformation
                    {
                        Id = user.Id,
                        DisplayName = model.DisplayName,
                    });
                    await _ctx.SaveChangesAsync();
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);

                    return RedirectToLocal(model.ReturnUrl);
                }
            }

            AddErrors(result);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string returnUrl) =>
            View(new ForgotPasswordViewModel {ReturnUrl = returnUrl});

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new {userId = user.Id, code},
                                         protocol: HttpContext.Request.Scheme);
            await _emailService.SendAsync(model.Email, "Reset Password",
                                          $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>",
                                          true);
            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation() => View();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null) => code == null ? View("Error") : View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }

            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation() => View();

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem {Text = purpose, Value = purpose})
                                           .ToList();
            return View(new SendCodeViewModel
                            {Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe});
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailService.SendAsync(await _userManager.GetEmailAsync(user), "Security Code", message, true);
            }

            return RedirectToAction(nameof(VerifyCode),
                                    new
                                    {
                                        Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl,
                                        RememberMe = model.RememberMe
                                    });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null) =>
            await _signInManager.GetTwoFactorAuthenticationUserAsync() == null
                ? View("Error")
                : View(new VerifyCodeViewModel
                {
                    Provider = provider,
                    ReturnUrl = returnUrl,
                    RememberMe = rememberMe
                });

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result =
                await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe,
                                                          model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

#region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl) =>
            Url.IsLocalUrl(returnUrl)
                ? RedirectToUrl(returnUrl)
                : RedirectToAction(nameof(HomeController.Index), "Home");

        private IActionResult RedirectToUrl(string url) => Redirect(url);

#endregion
    }
}