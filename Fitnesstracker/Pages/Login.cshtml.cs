using System.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Fitnesstracker.Pages
{
    public class Login : PageModel
    {
        private readonly IDbConnection _connection;

        public Login(IConfiguration configuration)
        {
            _connection = new MySqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE NAME = @Username AND PASSWORD = @Password",  
                new { Username, Password });

            // Create a cookie with the user id
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true // The cookie is accessible only by the server
            };
            Response.Cookies.Append("userId", user.Id.ToString(), cookieOptions);
            
            if (user != null)
            {
                return RedirectToPage("./Index");
            }

            // If user is not found or password is incorrect, stay on the login page
            return Page();
        }
    }
}