using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST10296771_CLDV7112_Practicum.Models;

namespace ST10296771_CLDV7112_Practicum.Controllers
{
    [Authorize]
    public class ImageryController : Controller
    {
        private readonly BlobContainerClient _container;
        private readonly ILogger<ImageryController> _log;

        public ImageryController(
            BlobServiceClient blobs, IConfiguration cfg, ILogger<ImageryController> log)
        {
            _container = blobs.GetBlobContainerClient(
                cfg["Storage:ImageryContainer"] ?? "raw-imagery");
            _log = log;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = new List<BlobListItemViewModel>();

            if (await _container.ExistsAsync(ct))
            {
                await foreach (var b in _container.GetBlobsAsync(cancellationToken: ct))
                {
                    var client = _container.GetBlobClient(b.Name);

                    // Read-only SAS valid for one hour. Requires an account-key
                    // connection string — CanGenerateSasUri is true only then.
                    string url = client.CanGenerateSasUri
                        ? client.GenerateSasUri(BlobSasPermissions.Read,
                            DateTimeOffset.UtcNow.AddHours(1)).ToString()
                        : client.Uri.ToString();

                    list.Add(new BlobListItemViewModel
                    {
                        Name = b.Name,
                        ContentType = b.Properties.ContentType ?? "application/octet-stream",
                        SizeBytes = b.Properties.ContentLength ?? 0,
                        UploadedUtc = b.Properties.LastModified,
                        Url = url
                    });
                }
            }

            return View(list.OrderByDescending(x => x.UploadedUtc).Take(60).ToList());
        }

        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        [RequestSizeLimit(500_000_000)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
        {
            if (file is null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Choose an image.");
                return View();
            }

            await _container.CreateIfNotExistsAsync(cancellationToken: ct);
            var name = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";

            await using var stream = file.OpenReadStream();
            await _container.GetBlobClient(name).UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            }, ct);

            TempData["Message"] = $"Uploaded {file.FileName}.";
            return RedirectToAction(nameof(Index));
        }
    }
}