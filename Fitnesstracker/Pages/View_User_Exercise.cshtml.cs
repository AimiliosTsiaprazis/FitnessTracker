using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fitnesstracker.Models;  // Existierende Exercise-Klasse verwenden
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace Fitnesstracker.Pages
{
    public class ViewUserExerciseModel : PageModel
    {
        private readonly IDbConnection _connection;

        public List<string> ExerciseIds { get; set; } = new List<string>();  // Liste der ExerciseIds als Strings
        public int CurrentIndex { get; set; } = 0;  // Aktueller Index der Übung
        public Exercise CurrentExercise { get; set; }  // Die aktuelle Übung, die angezeigt wird

        public ViewUserExerciseModel(IConfiguration configuration)
        {
            _connection = new MySqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void OnGet()
        {
            // Hier den Benutzer aus der Session oder den Cookies ermitteln
            int userId = GetUserIdFromSession(); // Diese Methode musst du implementieren
            ExerciseIds = GetUserExerciseIds(userId);
            CurrentIndex = Convert.ToInt32(Request.Query["currentIndex"].FirstOrDefault() ?? "0");

            if (ExerciseIds.Count > 0)
            {
                var exerciseId = ExerciseIds[CurrentIndex];
                CurrentExercise = FetchExerciseDetailsAsync(exerciseId).Result;  // Sync Aufruf für Razor
            }
        }

        public async Task<IActionResult> OnPostAsync(string action, int currentIndex)
        {
            int userId = GetUserIdFromSession(); // Benutzer-ID ermitteln
            ExerciseIds = GetUserExerciseIds(userId);

            if (action == "next" && currentIndex < ExerciseIds.Count - 1)
            {
                currentIndex++;
            }
            else if (action == "previous" && currentIndex > 0)
            {
                currentIndex--;
            }

            if (ExerciseIds.Count > 0)
            {
                var exerciseId = ExerciseIds[currentIndex];
                CurrentExercise = await FetchExerciseDetailsAsync(exerciseId);
            }

            // Return to the same page with the updated index
            return RedirectToPage(new { currentIndex });
        }

        // Methode um die EXERCISE_IDs als Strings aus der Datenbank zu holen
        private List<string> GetUserExerciseIds(int userId)
        {
            // SQL-Abfrage, um alle EXERCISE_IDs für einen Benutzer aus der user_exercises-Tabelle zu erhalten
            string query = "SELECT EXERCISE_ID FROM user_exercises WHERE USER_ID = @UserId";

            // Dapper verwendet, um die Datenbank abzufragen und die EXERCISE_ID als String zu holen
            var exerciseIds = _connection.Query<string>(query, new { UserId = userId }).AsList();

            return exerciseIds;  // Rückgabe der Liste von EXERCISE_IDs als Strings
        }

        private async Task<Exercise> FetchExerciseDetailsAsync(string exerciseId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://exercisedb.p.rapidapi.com/exercises/exercise/{exerciseId}"),
                Headers =
                {
                    { "X-RapidAPI-Key", "" },
                    { "X-RapidAPI-Host", "" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Exercise>(body);  // Use the existing Exercise class
                }
                else
                {
                    // Log a warning if fetching fails
                    Console.WriteLine($"Failed to fetch details for exercise ID: {exerciseId}");
                    return null;
                }
            }
        }

        // Beispiel für eine Methode, die die Benutzer-ID aus der Session oder Cookies holt
        private int GetUserIdFromSession()
        {
            // Hier implementierst du die Logik, um die Benutzer-ID aus der Session oder einem Cookie abzurufen
            // Zum Beispiel:
            string userIdCookie = Request.Cookies["userId"];
            return int.Parse(userIdCookie);  // Stelle sicher, dass der Cookie vorhanden und gültig ist
        }
    }
}
