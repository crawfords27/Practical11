using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Markdig;

namespace SMS.Web;

[HtmlTargetElement(Attributes = "asp-condition")] 
public class ConditionTagHelper : TagHelper
{
    [HtmlAttributeName("asp-condition")]
    public bool Condition { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!Condition)
        {
            output.SuppressOutput();
        }
    }
}





