using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class SliderFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<SliderField>(p =>
            p.Add(c => c.Label, "Volume")
             .Add(c => c.Value, 50));

        cut.Find("label").TextContent.Should().Contain("Volume");
    }

    [Fact]
    public void Renders_WithMinMax()
    {
        var cut = Render<SliderField>(p =>
            p.Add(c => c.Label, "Volume")
             .Add(c => c.Value, 50)
             .Add(c => c.Min, 0)
             .Add(c => c.Max, 100));

        var slider = cut.Find("input[type='range']");
        slider.GetAttribute("min").Should().Be("0");
        slider.GetAttribute("max").Should().Be("100");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<SliderField>(p =>
            p.Add(c => c.Label, "Volume")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, 50));

        cut.Find("input[type='range']").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValueChanged_FiresOnInput()
    {
        double newValue = 0;

        var cut = Render<SliderField>(p =>
            p.Add(c => c.Label, "Volume")
             .Add(c => c.Value, 50.0)
             .Add(c => c.ValueChanged, (double v) => newValue = v));

        cut.Find("input[type='range']").Input("75");

        newValue.Should().Be(75);
    }

    [Fact]
    public void Renders_CurrentValue_Display()
    {
        var cut = Render<SliderField>(p =>
            p.Add(c => c.Label, "Volume")
             .Add(c => c.Value, 50));

        cut.Find(".arcadia-field__slider-value").TextContent.Should().Contain("50");
    }

    [Fact]
    public void Renders_MinMaxLabels_WhenShowMinMaxTrue()
    {
        var cut = Render<SliderField>(p =>
            p.Add(c => c.Label, "Volume")
             .Add(c => c.Value, 50)
             .Add(c => c.Min, 0)
             .Add(c => c.Max, 100)
             .Add(c => c.ShowMinMax, true));

        cut.FindAll(".arcadia-field__slider-labels span").Should().HaveCount(2);
    }

    [Fact]
    public void AriaAttributes_Present()
    {
        var cut = Render<SliderField>(p =>
            p.Add(c => c.Label, "Volume")
             .Add(c => c.Value, 50)
             .Add(c => c.Min, 0)
             .Add(c => c.Max, 100));

        var slider = cut.Find("input[type='range']");
        slider.GetAttribute("aria-valuemin").Should().Be("0");
        slider.GetAttribute("aria-valuemax").Should().Be("100");
        slider.GetAttribute("aria-valuenow").Should().Be("50");
    }
}
