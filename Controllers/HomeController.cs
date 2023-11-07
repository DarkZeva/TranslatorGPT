using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
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

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}