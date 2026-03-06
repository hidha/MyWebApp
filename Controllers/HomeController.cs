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
                .Select(line => $"'{line.Replace("'", "\\'")}'");

            model.SecondContent = $"({string.Join(",", lines)})";
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult ExcelReader()
    {
        return View(new ExcelInputModel());
    }

    [HttpPost]
    public async Task<IActionResult> ExcelReader(ExcelInputModel model)
    {
        if (model.ExcelFile != null && model.ExcelFile.Length > 0)
        {
            try
            {
                var data = new List<Dictionary<string, string>>();
                var headers = new List<string>();

                using (var stream = new MemoryStream())
                {
                    await model.ExcelFile.CopyToAsync(stream);
                    stream.Position = 0;

                    // Simple CSV parsing as an alternative to Excel libraries
                    using (var reader = new StreamReader(stream))
                    {
                        var headerLine = await reader.ReadLineAsync();
                        if (headerLine != null)
                        {
                            headers = headerLine.Split(',').Select(h => h.Trim()).ToList();

                            string? line;
                            while ((line = await reader.ReadLineAsync()) != null)
                            {
                                var values = line.Split(',');
                                var row = new Dictionary<string, string>();

                                for (int i = 0; i < headers.Count && i < values.Length; i++)
                                {
                                    row[headers[i]] = values[i].Trim();
                                }

                                data.Add(row);
                            }
                        }
                    }
                }

                model.ColumnHeaders = headers;
                model.Data = data;
                model.Status = $"Successfully read {data.Count} rows from Excel file.";
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Error reading file: {ex.Message}";
            }
        }
        else
        {
            model.ErrorMessage = "Please select a file to upload.";
        }

        return View(model);
    }

    [HttpPost]
    public ActionResult ReadExcel(string fileName)
    {
        var result = new { success = true, data = "ReadExcel result, FileName: " + fileName };
        return Json(result);
    }
}