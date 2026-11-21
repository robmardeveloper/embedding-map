using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace EmbeddingMap.Web.Components;

/// <summary>
/// An SVG &lt;text&gt; element. Razor reserves the &lt;text&gt; tag for itself, so this one is built in code.
/// </summary>
public sealed class SvgLabel : ComponentBase
{
    [Parameter, EditorRequired]
    public required string Text { get; set; }

    [Parameter]
    public double X { get; set; }

    [Parameter]
    public double Y { get; set; }

    [Parameter]
    public double FontSize { get; set; }

    [Parameter]
    public double StrokeWidth { get; set; }

    [Parameter]
    public string? CssClass { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "text");
        builder.AddAttribute(1, "class", CssClass);
        builder.AddAttribute(2, "x", F(X));
        builder.AddAttribute(3, "y", F(Y));
        builder.AddAttribute(4, "font-size", F(FontSize));
        builder.AddAttribute(5, "stroke-width", F(StrokeWidth));
        builder.AddContent(6, Text);
        builder.CloseElement();
    }

    private static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
}
