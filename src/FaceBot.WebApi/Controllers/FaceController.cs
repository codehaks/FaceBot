using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.WebSockets;
using System.Transactions;

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
    [Route("bytes")]
    public async Task<IActionResult> Get()
    {
        FindRandomFile(out int number, out string randomFile);

        var bytes = await System.IO.File.ReadAllBytesAsync(randomFile);

        return File(bytes, "imagee/jpg", $"face{number}.jpg", true);
    }

    private static void FindRandomFile(out int number, out string randomFile)
    {
        var files = System.IO.Directory.GetFiles(@"G:\Projects\Data\Fake-Faces");
        number = new Random().Next(0, files.Length);
        randomFile = files[number];
    }

    [HttpGet]
    [Route("stream")]
    public async Task GetStream()
    {
        FindRandomFile(out int number, out string randomFile);

        using var fileStream = new FileStream(randomFile, FileMode.Open);

        Response.StatusCode = 200;
        Response.Headers.Append(HeaderNames.ContentDisposition, $"attachment; filename=\"face{number}.jpg\"");
        Response.Headers.Append(HeaderNames.ContentType, "application/octet-stream");

        //-------------------
        byte[] buffer = new byte[16 * 1024];
        long totalBytes = fileStream.Length;

        long totalReadBytes = 0;
        int readBytes;

        var outputStream = Response.Body;

        while ((readBytes = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            await outputStream.WriteAsync(buffer, 0, readBytes);
            totalReadBytes += readBytes;
        }

        //-------------------

        fileStream.Close();

        await outputStream.FlushAsync();

    }

}


