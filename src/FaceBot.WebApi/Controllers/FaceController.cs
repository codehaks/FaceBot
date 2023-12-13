using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace FaceBot.WebApi.Controllers;
[ApiController]
[Route("[controller]")]
public class FaceController : ControllerBase
{
    private readonly ILogger<FaceController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public FaceController(ILogger<FaceController> logger, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    [Route("bytes")]
    public async Task<IActionResult> Get()
    {
        FindRandomFile(out int number, out string randomFile);

        var bytes = await System.IO.File.ReadAllBytesAsync(randomFile);

        return File(bytes, "image/jpg", $"face{number}.jpg", true);
    }

    private void FindRandomFile(out int number, out string randomFile)
    {
        var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Images");
        var files = System.IO.Directory.GetFiles(path);
        number = new Random().Next(0, files.Length);
        randomFile = files[number];
    }

    [HttpGet]
    [Route("stream")]
    public async Task GetStream()
    {
        FindRandomFile(out int number, out string randomFile);

        _logger.LogInformation("File Found: {0}", number);

        using var fileStream = new FileStream(randomFile, FileMode.Open,FileAccess.Read);

        Response.StatusCode = 200;
        Response.Headers.Append(HeaderNames.ContentDisposition, $"attachment; filename=\"face{number}.jpg\"");
        Response.ContentType = "image/jpeg"; // Set more specific content type for JPEG images

        var outputStream = Response.Body;

        await using (outputStream)
        {
            byte[] buffer = new byte[16 * 1024];
            int readBytes;

            while ((readBytes = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await outputStream.WriteAsync(buffer, 0, readBytes);
            }
        }

        _logger.LogInformation("File Found Sent: {0}", number);
    }

}


