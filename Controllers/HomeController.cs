using Microsoft.AspNetCore.Mvc;
using ST10296771_CLDV7112_Practicum.Models;
using System.Diagnostics;

namespace ST10296771_CLDV7112_Practicum.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}