using System.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Fitnesstracker.Pages
{
    public class Register : PageModel
    {
        private readonly IDbConnection _connection;

        public Register(IConfiguration configuration)
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

            var userExists = await _connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE NAME = @Username",
                new { Username });

            if (userExists != null)
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return RedirectToPage("./Register", new { userfound = 1 });
            }

            var insertSql = "INSERT INTO users (NAME, PASSWORD) VALUES (@Username, @Password)";
            await _connection.ExecuteAsync(insertSql, new { Username, Password });

            return RedirectToPage("./Login");
        }
    }
}