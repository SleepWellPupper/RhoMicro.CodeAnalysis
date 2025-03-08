namespace RhoMicro.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Text.Templating;

internal partial class TemplateAttribute
{
    public partial record Model
    {
        public String NewlineValue => Newline switch
        {
            Newline.CrLf => "\r\n",
            Newline.Cr => "\r",
            _ => "\n"
        };
    }
}
