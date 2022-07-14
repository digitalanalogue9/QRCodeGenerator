namespace QRCodeGenerator.Web.Infrastructure.Tags
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using HtmlTags;
    using HtmlTags.Conventions;

    public class TagConventions : HtmlConventionRegistry
    {
        public TagConventions()
        {

            this.Editors.Always.AddClass("form-control");
            //this.Editors.IfPropertyIs<DateTime?>().ModifyWith(m => m.CurrentTag
            //    .AddPattern("9{1,2}/9{1,2}/9999")
            //    .AddPlaceholder("DD/MM/YYYY")
            //    .AddClass("datepicker")
            //    .Value(m.Value<DateTime?>() != null ? m.Value<DateTime>().ToShortDateString() : string.Empty));
            //this.Editors.If(er => er.Accessor.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase)).BuildBy(a => new HiddenTag().Value(a.StringValue()));

            this.Editors.IfPropertyIs<byte[]>().BuildBy(a => new HiddenTag().Value(Convert.ToBase64String(a.Value<byte[]>())));

            //this.Editors.If(er => er.Accessor.Name.EndsWith("Time", StringComparison.OrdinalIgnoreCase)).BuildBy(a => new HiddenTag().Value(a.StringValue()));



            this.Labels.Always.AddClass("control-label");
            this.Labels.ModifyForAttribute<DisplayAttribute>((t, a) => t.Text(a.Name));

            Labels
                .Always
                .ModifyWith(er => er.CurrentTag.Attr("for",er.CurrentTag.Attr("for").Replace("Data","Data.")));
            

            Labels
                .Always
                .ModifyWith(er => er.CurrentTag.Text(er.CurrentTag.Text().Replace("Data ", "")));

            this.DisplayLabels.Always.BuildBy<DefaultDisplayLabelBuilder>();
            this.DisplayLabels.ModifyForAttribute<DisplayAttribute>((t, a) => t.Text(a.Name));
            this.Displays.IfPropertyIs<DateTime>().ModifyWith(m => m.CurrentTag.Text(m.Value<DateTime>().ToShortDateString()));
            this.Displays.IfPropertyIs<DateTime?>().ModifyWith(m => m.CurrentTag.Text(m.Value<DateTime?>()?.ToShortDateString()));
            this.Displays.IfPropertyIs<decimal>().ModifyWith(m => m.CurrentTag.Text(m.Value<decimal>().ToString("C")));

            this.Defaults();
        }

        public ElementCategoryExpression DisplayLabels => new ElementCategoryExpression(this.Library.TagLibrary.Category(nameof(this.DisplayLabels)).Profile(TagConstants.Default));
    }
}