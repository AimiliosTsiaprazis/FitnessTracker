using Dapper;
using Fitnesstracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fitnesstracker.Pages
{
    public class Profile : PageModel
    {
        private readonly IDbConnection _connection;

        public Profile(IConfiguration configuration)
        {
            _connection = new MySqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public List<string> ExerciseIds { get; set; } = new List<string>();  // Liste der ExerciseIds als Strings
        public int CurrentIndex { get; set; } = 0;  // Aktueller Index der �bung
        public Exercise CurrentExercise { get; set; }  // Die aktuelle �bung, die angezeigt wird

        // Z�hler f�r jede Muskelgruppe
        public Dictionary<string, int> BodyPartCounter { get; set; } = new Dictionary<string, int>
        {
            { "waist", 0 },
            { "upper legs", 0 },
            { "back", 0 },
            { "lower legs", 0 },
            { "chest", 0 }
        };

        public async Task OnGetAsync()
        {
            // Hier den Benutzer aus der Session oder den Cookies ermitteln
            int userId = GetUserIdFromSession(); // Diese Methode musst du implementieren
            ExerciseIds = GetUserExerciseIds(userId);
            CurrentIndex = Convert.ToInt32(Request.Query["currentIndex"].FirstOrDefault() ?? "0");

            // Initialisiere den Z�hler f�r Muskelgruppen
            foreach (var exerciseId in ExerciseIds)
            {
                Console.WriteLine(exerciseId);
                var exercise = await FetchExerciseDetailsAsync(exerciseId);
                Console.WriteLine(exercise.BodyPart);

                if (exercise != null && BodyPartCounter.ContainsKey(exercise.BodyPart))
                {
                    // Z�hle, wie oft jede Muskelgruppe vorkommt
                    BodyPartCounter[exercise.BodyPart]++;
                }
            }

            // Ausgabe zur �berpr�fung (optional)
            foreach (var bodyPart in BodyPartCounter)
            {
                Console.WriteLine($"{bodyPart.Key}: {bodyPart.Value} �bungen");
            }

            if (ExerciseIds.Count > 0)
            {
                var exerciseId = ExerciseIds[CurrentIndex];
                CurrentExercise = await FetchExerciseDetailsAsync(exerciseId);  // Aktuelle �bung abrufen
            }


            var bodyPartCounts = BodyPartCounter.Values.ToList();

            // �bergebe die Liste der H�ufigkeiten an die View
            ViewData["BodyPartData"] = JsonConvert.SerializeObject(bodyPartCounts);
        }

        public IActionResult OnPost()
        {
            // Clear the userId cookie
            Response.Cookies.Delete("userId");

            // Redirect to the index page
            return RedirectToPage("/Index");
        }

        private int GetUserIdFromSession()
        {
            // Hier implementierst du die Logik, um die Benutzer-ID aus der Session oder einem Cookie abzurufen
            string userIdCookie = Request.Cookies["userId"];
            return int.Parse(userIdCookie);  // Stelle sicher, dass der Cookie vorhanden und g�ltig ist
        }

        private List<string> GetUserExerciseIds(int userId)
        {
            // SQL-Abfrage, um alle EXERCISE_IDs f�r einen Benutzer aus der user_exercises-Tabelle zu erhalten
            string query = "SELECT EXERCISE_ID FROM user_exercises WHERE USER_ID = @UserId";

            // Dapper verwendet, um die Datenbank abzufragen und die EXERCISE_ID als String zu holen
            var exerciseIds = _connection.Query<string>(query, new { UserId = userId }).AsList();

            return exerciseIds;  // R�ckgabe der Liste von EXERCISE_IDs als Strings
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
                    return JsonConvert.DeserializeObject<Exercise>(body);  // Deserialize the JSON to Exercise object
                }
                else
                {
                    Console.WriteLine($"Failed to fetch details for exercise ID: {exerciseId}");
                    return null;
                }
            }
        }
    }
}
