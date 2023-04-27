using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using SMS.Data.Services;
using SMS.Web.Models;
using System.Security.Claims;
using SMS.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SMS.Web.Controllers;

public class UserController : BaseController
{
    private readonly IUserService svc;

    public UserController(IUserService _svc)
    { 
        svc = _svc;
    }

    // GET /user/login
    public IActionResult Login()
    {
        return View();
    }

    // POST /user/login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginViewModel m)
    {        
        // call service to Authenticate User
        var user = svc.Authenticate(m.Email, m.Password);

        // if user not authenticated manually add validation errors for email and password
        if (user == null)
        {
            ModelState.AddModelError("Email", "Invalid Login Credentials");
            ModelState.AddModelError("Password", "Invalid Login Credentials");
            return View(m);
        }
        
        // authenticated so sign user in using cookie authentication to store principal
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            BuildClaimsPrincipal(user)
        );
        return RedirectToAction("Index","Home");
    }

    // GET /user/register
    public IActionResult Register()
    {
        return View();
    }


    // POST /user/register
    [HttpPost] [ValidateAntiForgeryToken]
    public IActionResult Register(UserRegisterViewModel m)
    {
        // if email address is already in use add validation error
        if (svc.GetUserByEmail(m.Email) != null) {
            ModelState.AddModelError(nameof(m.Email),"This email address is already in use. Choose another");
        }

        // check validation
        if (ModelState.IsValid)
        {
            // register user
            var user = svc.Register(m.Name, m.Email, m.Password, m.Role);               

            // registration successful now redirect to login page
            return RedirectToAction(nameof(Login));
        }

        // redisplay the registration form
        return View(m);   
    }


    // POST /user/logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    // GET /user/errornotauthorised
    public IActionResult ErrorNotAuthorised()
    {   
        Alert("You are not Authorised to Carry out that action");
        return RedirectToAction("Index", "Home");
    }
    
    // GET /user/errornotauthenticated
    public IActionResult ErrorNotAuthenticated()
    {
         Alert("You must first Authenticate to carry out that action");
        return RedirectToAction("Login", "User"); 
    }
    
    // GET /user/profile
    [Authorize]
    public IActionResult Profile()
    {
        // retrieve the id of currently signed in user from the ClaimsPrincipal
        var id = User.GetSignedInUserId();
        var user  = svc.GetUser( id );
        if (user is null)
        {
            Alert("User profile is not accessible", AlertType.warning);
            return RedirectToAction("Index","Home");
        }
        // display the user profile
        return View(user);
    }

    
    // ======================= UTILITY METHODS ======================

    // Used by Remote Validator to verify email is unique 
    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyEmailAddress(string email)
    {
        // TBC - replace with code to validate the email and return a validation response
        // email must not already exist and if it does then return validation error
        // see https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation#remote-attribute.
 
        return Json(true);
    }

    // return a claims principle using the info from the user parameter
    private ClaimsPrincipal BuildClaimsPrincipal(User user)
    { 
        // define user claims
        var claims = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Sid, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),                      
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        // build principal using claims
        return  new ClaimsPrincipal(claims);
    }       

}

