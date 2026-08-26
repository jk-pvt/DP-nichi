namespace DesignPatternCatalog.Models;

public class CodeFileItem
{
    public string FileName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Language { get; set; } = "csharp";
}
