using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using OfficeOpenXml;

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
        if (!string.IsNullOrWhiteSpace(model.input))
        {
            var lines = model.input
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line => $"'{line.Replace("'", "\\'")}'");

            model.output = $"({string.Join(",", lines)})";
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
    public IActionResult GetSheetNames(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        using var stream = new MemoryStream();
        file.CopyTo(stream);

        using var package = new ExcelPackage(stream);

        // Collect sheet names
        var sheetNames = package.Workbook.Worksheets
            .Select(ws => ws.Name)
            .ToList();

        return Ok(sheetNames);
    }

    /// <summary>
    /// Reads an uploaded Excel file and returns its content as JSON.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult ReadExcel(IFormFile file, string sheetName, string outputType)
    {
        // var result = new { success = true, data = "ReadExcel result, FileName: " + fileName };
        // return Json(result);
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using var stream = new MemoryStream();
        file.CopyTo(stream);
        using var package = new ExcelPackage(stream);
        ExcelWorksheet? worksheet = null;
        if (String.IsNullOrEmpty(sheetName)) {
            worksheet = package.Workbook.Worksheets.FirstOrDefault();
        }
        else
        {
            worksheet = package.Workbook.Worksheets[sheetName];
        }
        
        if (worksheet == null)
        {
            return BadRequest("No worksheet found in the uploaded file.");
        }

        var twoRowsData = ReadTwoRowsAsOne(worksheet);
        // var data = new List<Dictionary<string, string>>();
        // var headers = new List<string>();

        // for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        // {
        //     headers.Add(worksheet.Cells[1, col].Text);
        // }

        // for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        // {
        //     var rowData = new Dictionary<string, string>();
        //     for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        //     {
        //         rowData[headers[col - 1]] = worksheet.Cells[row, col].Text;
        //     }
        //     data.Add(rowData);
        // }


        // var result = new { success = true, data };
        return twoRowsData != null
            ? Json(new { success = true, data = twoRowsData })
            : Json(new { success = false, message = "Failed to read Excel file." });
    }

    public List<string> ReadTwoRowsAsOne(ExcelWorksheet ws)
    {
        var result = new List<string>();

        int totalRows = ws.Dimension.Rows;
        int totalCols = ws.Dimension.Columns;

        // Read header row once
        var header = new List<string>();
        for (int c = 1; c <= totalCols; c++)
        {
            header.Add(ws.Cells[1, c].Text);
        }        

        var headerLine = new List<string>();
        headerLine.AddRange(header);
        headerLine.AddRange(header);

        result.Add(string.Join(",", headerLine));

        for (int r = 2; r <= totalRows; r += 2)   // step by 2
        {
            var combined = new List<string>();

            // First row
            for (int c = 1; c <= totalCols; c++)
            {
                combined.Add(ws.Cells[r, c].Text);
            }

            // Second row (only if it exists)
            if (r + 1 <= totalRows)
            {
                for (int c = 1; c <= totalCols; c++)
                {
                    combined.Add(ws.Cells[r + 1, c].Text);
                }
            }
            // Convert combined row to CSV string
            string csvLine = string.Join(",", combined);
            result.Add(csvLine);
        }

        return result;
    }    
}