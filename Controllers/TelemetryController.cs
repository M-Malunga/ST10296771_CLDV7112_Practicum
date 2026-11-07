using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST10296771_CLDV7112_Practicum.Models;

namespace ST10296771_CLDV7112_Practicum.Controllers
{
    [Authorize]
    public class TelemetryController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;

        public TelemetryController(IHttpClientFactory http, IConfiguration cfg)
        {
            _http = http; _cfg = cfg;
        }

        public async Task<IActionResult> Index(string sensorId = "sensor-001", CancellationToken ct = default)
        {
            var vm = new TelemetryViewModel { SensorId = sensorId };
            var baseUrl = _cfg["Functions:TelemetryQueryUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                vm.Error = "Telemetry function URL is not configured.";
                return View(vm);
            }

            var url = $"{baseUrl.TrimEnd('/')}/{Uri.EscapeDataString(sensorId)}";
            var key = _cfg["Functions:ApiKey"];
            if (!string.IsNullOrWhiteSpace(key)) url += $"?code={Uri.EscapeDataString(key)}";

            var response = await _http.CreateClient("TelemetryApi").GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                vm.Error = $"Telemetry API returned {(int)response.StatusCode}.";
                return View(vm);
            }

            vm.Readings = await response.Content
                .ReadFromJsonAsync<List<TelemetryReadingDto>>(cancellationToken: ct) ?? new();
            return View(vm);
        }
    }
}