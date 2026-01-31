using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Fitnesstracker.Models;

namespace Fitnesstracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<Exercise> ApiResult { get; set; } 
        public List<string> BodyParts { get; set; }
 
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGetAsync(string bodyPart = null)
        {
            var client = new HttpClient();

            // Request for specific body part or all exercises
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://exercisedb.p.rapidapi.com/exercises{(string.IsNullOrEmpty(bodyPart) ? "" : "/bodyPart/" + bodyPart)}"),
                Headers =
                {
                    { "X-RapidAPI-Key", "" },
                    { "X-RapidAPI-Host", "" },
                }, 
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                ApiResult = JsonConvert.DeserializeObject<List<Exercise>>(body);
            }

            // Request for all exercises to get the list of body parts
            request = new HttpRequestMessage 
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://exercisedb.p.rapidapi.com/exercises"),
                Headers =
                {
                    { "X-RapidAPI-Key", "" },
                    { "X-RapidAPI-Host", "" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var allExercises = JsonConvert.DeserializeObject<List<Exercise>>(body);
                BodyParts = allExercises.Select(e => e.BodyPart).Distinct().ToList();
            }
        }
    }
}