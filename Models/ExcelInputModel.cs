namespace MyWebApp.Models;

public class ExcelInputModel
{
    public IFormFile? ExcelFile { get; set; }
    public List<Dictionary<string, string>>? Data { get; set; }
    public List<string>? ColumnHeaders { get; set; }
    public string? Status { get; set; }
    public string? ErrorMessage { get; set; }
}
