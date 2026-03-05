using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;

namespace MyWebApp.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new TextInputModel());
    }

    [HttpPost]
    public IActionResult Index(TextInputModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Content))
        {
            var lines = model.Content
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line => $"'{line}'");

            model.SecondContent = $"({string.Join(",", lines)})";
        }

        return View(model);
    }
}