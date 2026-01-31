using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace Fitnesstracker.Pages.ajax;

public class ExerciseHandler : PageModel
{
    private readonly IConfiguration _configuration;

    public ExerciseHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult OnPostInsertExercise(int userId, string exerciseId, int repetitions, int sets)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "INSERT INTO user_exercises (USER_ID, EXERCISE_ID, REPETITIONS, SETS) VALUES (@UserId, @ExerciseId, @Repetitions, @Sets)";
            connection.Execute(sql, new { UserId = userId, ExerciseId = exerciseId, Repetitions = repetitions, Sets = sets });
        }

        return new JsonResult(new { success = true, message = "Exercise added successfully." });
    }
}