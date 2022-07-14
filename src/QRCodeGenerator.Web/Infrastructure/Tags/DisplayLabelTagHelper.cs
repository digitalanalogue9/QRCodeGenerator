namespace QRCodeGenerator.Web.Infrastructure.Tags
{
    using HtmlTags;

    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("display-label-tag", Attributes = nameof(For), TagStructure = TagStructure.WithoutEndTag)]
    public class DisplayLabelTagHelper : HtmlTagTagHelper
    {
        protected override string Category { get; } = nameof(TagConventions.DisplayLabels);
    }
}