using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class DateRangeFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLegend()
    {
        var cut = Render<DateRangeField>(p =>
            p.Add(c => c.Label, "Booking Period")
             .Add(c => c.StartDate, (DateTime?)null)
             .Add(c => c.EndDate, (DateTime?)null));

        cut.Find("legend").TextContent.Should().Contain("Booking Period");
    }

    [Fact]
    public void Renders_TwoDateInputs()
    {
        var cut = Render<DateRangeField>(p =>
            p.Add(c => c.Label, "Date Range")
             .Add(c => c.StartDate, new DateTime(2026, 1, 1))
             .Add(c => c.EndDate, new DateTime(2026, 12, 31)));

        cut.FindAll("input[type='date']").Should().HaveCount(2);
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<DateRangeField>(p =>
            p.Add(c => c.Label, "Date Range")
             .Add(c => c.Disabled, true)
             .Add(c => c.StartDate, (DateTime?)null)
             .Add(c => c.EndDate, (DateTime?)null));

        var inputs = cut.FindAll("input[type='date']");
        inputs.Should().AllSatisfy(i => i.HasAttribute("disabled").Should().BeTrue());
    }

    [Fact]
    public void Renders_StartDate_Formatted()
    {
        var cut = Render<DateRangeField>(p =>
            p.Add(c => c.Label, "Date Range")
             .Add(c => c.StartDate, new DateTime(2026, 3, 15))
             .Add(c => c.EndDate, (DateTime?)null));

        var inputs = cut.FindAll("input[type='date']");
        inputs[0].GetAttribute("value").Should().Be("2026-03-15");
    }

    [Fact]
    public void Renders_EndDate_Formatted()
    {
        var cut = Render<DateRangeField>(p =>
            p.Add(c => c.Label, "Date Range")
             .Add(c => c.StartDate, (DateTime?)null)
             .Add(c => c.EndDate, new DateTime(2026, 6, 30)));

        var inputs = cut.FindAll("input[type='date']");
        inputs[1].GetAttribute("value").Should().Be("2026-06-30");
    }

    [Fact]
    public void StartDateChanged_FiresOnChange()
    {
        DateTime? newStart = null;

        var cut = Render<DateRangeField>(p =>
            p.Add(c => c.Label, "Date Range")
             .Add(c => c.StartDate, (DateTime?)null)
             .Add(c => c.EndDate, (DateTime?)null)
             .Add(c => c.StartDateChanged, (DateTime? v) => newStart = v));

        cut.FindAll("input[type='date']")[0].Change("2026-05-01");

        newStart.Should().NotBeNull();
        newStart!.Value.Month.Should().Be(5);
    }

    [Fact]
    public void EndDateChanged_FiresOnChange()
    {
        DateTime? newEnd = null;

        var cut = Render<DateRangeField>(p =>
            p.Add(c => c.Label, "Date Range")
             .Add(c => c.StartDate, (DateTime?)null)
             .Add(c => c.EndDate, (DateTime?)null)
             .Add(c => c.EndDateChanged, (DateTime? v) => newEnd = v));

        cut.FindAll("input[type='date']")[1].Change("2026-12-25");

        newEnd.Should().NotBeNull();
        newEnd!.Value.Month.Should().Be(12);
    }

    [Fact]
    public void Renders_CustomLabels()
    {
        var cut = Render<DateRangeField>(p =>
            p.Add(c => c.Label, "Vacation")
             .Add(c => c.StartLabel, "Check-in")
             .Add(c => c.EndLabel, "Check-out")
             .Add(c => c.StartDate, (DateTime?)null)
             .Add(c => c.EndDate, (DateTime?)null));

        var sublabels = cut.FindAll(".arcadia-field__daterange-sublabel");
        sublabels[0].TextContent.Should().Be("Check-in");
        sublabels[1].TextContent.Should().Be("Check-out");
    }

    [Fact]
    public void Renders_Separator()
    {
        var cut = Render<DateRangeField>(p =>
            p.Add(c => c.Label, "Date Range")
             .Add(c => c.StartDate, (DateTime?)null)
             .Add(c => c.EndDate, (DateTime?)null));

        cut.Find(".arcadia-field__daterange-separator").TextContent.Should().Be("to");
    }
}
