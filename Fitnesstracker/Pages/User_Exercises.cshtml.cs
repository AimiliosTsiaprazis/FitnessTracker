using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using Fitnesstracker.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Fitnesstracker.Pages
{
    public class UserExercisesModel : PageModel
    {
        private readonly ILogger<UserExercisesModel> _logger;
        public List<Exercise> Exercises { get; set; } 
        public List<string> BodyParts { get; set; }
        public IConfiguration Configuration { get; }

        public UserExercisesModel(IConfiguration configuration, ILogger<UserExercisesModel> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public List<dynamic> userExercises { get; set; }

        public async Task OnGetAsync()
        {
            var userId = Request.Cookies["userId"];
            _logger.LogInformation($"Retrieved userId from cookie: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("userId cookie is null or empty.");
                userExercises = new List<dynamic>();
                return;
            }

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                var sql = "SELECT * FROM user_exercises WHERE USER_ID = @UserId";
                userExercises = connection.Query(sql, new { UserId = userId }).ToList();

                Exercises = new List<Exercise>();
                foreach (var userExercise in userExercises)
                {
                    var exercise = await FetchExerciseDetailsAsync(userExercise.EXERCISE_ID);
                    if (exercise != null)
                    {
                        Exercises.Add(exercise);
                    }
                }
            }
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
                    return JsonConvert.DeserializeObject<Exercise>(body);
                }
                else
                {
                    _logger.LogWarning($"Failed to fetch details for exercise ID: {exerciseId}");
                    return null;
                }
            }
        }
    }
}