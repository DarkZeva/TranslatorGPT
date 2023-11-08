using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using TranslateGPT.DTO;
using TranslateGPT.Models;

namespace TranslateGPT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly List<string> mostUsedLanguages = new List<string>()
        {
            "Polish",
            "English",
            "German",
            "Japanese",
            "Korean",
            "Turkish",
        };

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            ViewBag.Languages = new SelectList(mostUsedLanguages);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OpenAIGPT(string query, string selectedLanguage)
        {
            var openAPIKey = _configuration["OpenAI:ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAPIKey}");

            var payload = new
            {
                model = "gpt-3.5-turbo-1106",
				messages = new object[]
                {
                    new { role = "system", content = $"Translate to {selectedLanguage}"},
                    new { role = "user", content = query}
                },
				temperature = 0,
				max_tokens = 256
			};
            string jsonPayload = JsonConvert.SerializeObject(payload);
            HttpContent httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var responseMessage = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);
            var responseMessageJson = await responseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<AIResponse>(responseMessageJson);

            ViewBag.Result = response.Choices[0].Message.Content;
			ViewBag.Languages = new SelectList(mostUsedLanguages);
			return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}