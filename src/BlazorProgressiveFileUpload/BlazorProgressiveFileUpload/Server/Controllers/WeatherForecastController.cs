using BlazorProgressiveFileUpload.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorProgressiveFileUpload.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
		}

		[HttpPost]
		public async Task<IActionResult> Upload([FromForm] IFormFile? file)
		{
			// Check if the file is there
			if (file == null)
				return BadRequest("File is required");

			// Get the file name
			var fileName = file.FileName;

			// Get the extension
			var extension = Path.GetExtension(fileName);

			// Validate the extension based on your business needs

			// Generate a new file to avoid duplicates = (FileName withoutExtension - GUId.extension)
			var newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-{Guid.NewGuid().ToString()}{extension}";

			// Create the full path of the file including the directory (For this demo we will save the file inside a folder called Data within wwwroot)
			var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data");
			var fullPath = Path.Combine(directoryPath, newFileName);

			// Make sure the directory is there by creating it if it's not exist
			Directory.CreateDirectory(directoryPath);

			// Create a new file stream where you want to put your file and copy the content from the current file stream to the new one
			using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
			{
				// Copy the content to the new stream
				await file.CopyToAsync(fileStream);
			}

			// You are done return the new URL which is (your application url/data/newfilename)
			return Ok($"https://localhost:44302/Data/{newFileName}");
		}

	}
}