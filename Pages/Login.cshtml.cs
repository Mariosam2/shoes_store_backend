using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoesStore.Entities;
using ShoesStore.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ShoesStore.Pages
{
    public class LoginModel : PageModel
    {
        public string? ExceptionMessage { get; set; }
        public string? OldUsername { get; set; }
        public string? OldPassword { get; set; }
        private readonly StoreDBContext _context;
        public LoginModel(StoreDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public IActionResult OnGet(string? returnUrl = null)
        {

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("./Dashboard");
            }
            if (returnUrl != null)
            {
                ExceptionMessage = "Unauthorized";

            }
            return Page();

        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ExceptionMessage = null;
            var authUser = await _context.Users.FirstOrDefaultAsync(user => user.Username == Username);
            var passwordHasher = new PasswordHasher<User>();

            if (authUser == null)
            {
                OldUsername = Username;
                OldPassword = Password;
                ExceptionMessage = "User not found";
                return Page();
            }
            var verifiedPassword = passwordHasher.VerifyHashedPassword(authUser, authUser.Password, Password);
            if (!verifiedPassword.Equals(PasswordVerificationResult.Success))
            {
                OldUsername = Username;
                OldPassword = Password;
                ExceptionMessage = "Wrong credentials";
                return Page();
            }


            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, authUser.Username),
                new(ClaimTypes.NameIdentifier, authUser.UserId.ToString()),
                new(ClaimTypes.Role, "admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToPage("./Dashboard");


        }



    }
}