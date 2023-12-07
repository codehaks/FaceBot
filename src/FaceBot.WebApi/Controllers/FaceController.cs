using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FaceBot.WebApi.Controllers;
[ApiController]
[Route("[controller]")]
public class FaceController : ControllerBase
{
    private readonly ILogger<FaceController> _logger;

    public FaceController(ILogger<FaceController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        FindRandomFile(out int number, out string randomFile);

        _logger.LogInformation($"File Id {number} and name {randomFile}");
        var bytes = await System.IO.File.ReadAllBytesAsync(randomFile);

        return File(bytes, "imagee/jpg", $"face{number}.jpg", true);
    }

    private static void FindRandomFile(out int number, out string randomFile)
    {
        var files = System.IO.Directory.GetFiles(@"G:\Projects\Data\Fake-Faces");
        number = new Random().Next(0, files.Length);
        randomFile = files[number];
    }

}


