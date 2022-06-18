using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VKR.Models;
using VKR.Models.AuthorizationModels;
using VKR.Models.ProfessionalGradeModels;
using VKR.Repositories.Authorization;

namespace VKR.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly DBContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AuthorizationController(IAuthorizationRepository authorizationRepository, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _authorizationRepository = authorizationRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View("Login");
        }

        public async Task<IActionResult> LoginUser([FromBody] LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, _authorizationRepository.GetHashPassword(model.Password), false, false);
                    if (result.Succeeded)
                    {
                        return Json(new { Success = true });
                    }
                }
                return Json(new { Success = false, Message = "Неправильный логин и (или) пароль" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Неправильный логин и (или) пароль" });
            }
        }

        public IActionResult Register()
        {
            return View("Register");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> RegisterNewUser([FromBody] Userdf user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityUser userModel = new IdentityUser { Email = user.Email, UserName = user.Email };
                    //var userModel = new AspNetUser
                    //{
                    //    PhoneNumberConfirmed = false,
                    //    AccessFailedCount = 0,
                    //    LockoutEnabled = false,
                    //    TwoFactorEnabled = false,
                    //    EmailConfirmed = false,
                    //    Email = user.UserName,
                    //    UserName = user.UserName,
                    //    PasswordHash = _authorizationRepository.GetHashPassword(user.Password)
                    //};
                    // добавляем пользователя

                    var result = await _userManager.CreateAsync(userModel, _authorizationRepository.GetHashPassword(user.Password));
                    if (result.Succeeded)
                    {
                        // установка куки
                        await _signInManager.SignInAsync(userModel, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Internal Error" });
                    }
                }
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Internal Error" });
            }
            //return View(model);
            //try
            //{
            //    await _authorizationRepository.RegisterNewUser(user);
            //    return Json(new { Success = true });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { Success = false, Message = "Internal Error" });
            //}
        }
    }
}
