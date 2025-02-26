namespace RhoMicro.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Text.Templating;

internal partial class TemplateAttribute
{
    public partial record Model
    {
        public String NewlineValue => Newline switch
        {
            Newline.CarriageReturnNewline => "\r\n",
            Newline.CarriageReturn => "\r",
            _ => "\n"
        };
    }
}
