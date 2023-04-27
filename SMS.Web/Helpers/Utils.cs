
using Markdig;
using Microsoft.AspNetCore.Html;

namespace SMS.Web;

public static class Utils
{
    // convert markdown to santitised html
    // https://www.codemag.com/article/1811071/Marking-up-the-Web-with-ASP.NET-Core-and-Markdown
    public static string ParseMarkdown(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder()
                            .UseAdvancedExtensions()
                            .DisableHtml() // stops use of script tags or other potential xss issues
                            .Build();
        return Markdown.ToHtml(markdown, pipeline);
    }

    // Convert an IFormFile to a Base64 encoded string    
    public static string ConvertToBase64(IFormFile file)
    {
        if (file is null) return null;
        
        string base64Representation = string.Empty;
        using (var memoryStream = new MemoryStream())
        {
           file.CopyTo(memoryStream);
           base64Representation = Convert.ToBase64String(memoryStream.ToArray());
        }
        var result = $"data:{file.ContentType};base64,{base64Representation}"; 
        return result;
    }    

    // utility method to copy IFormFile to a path within wwwroot (defaults to /)
    public static string CopyToPathInRoot(IFormFile file, string path="")
    {
        if (file is null) return null;
        
        string filePath = null;
        if (file.Length > 0)
        {
            string _FileName = Path.GetFileName(file.FileName);
               
            //var filePath = Path.GetTempFileName();
            filePath = Path.Combine(path, Path.GetFileName(file.FileName));
            using (var stream = System.IO.File.Create(Path.Combine("wwwroot",filePath)))
            {
                file.CopyTo(stream);
            }
        }
        return "/" + filePath;
    } 
}

