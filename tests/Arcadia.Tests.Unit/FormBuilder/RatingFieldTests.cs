using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class RatingFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Value, 0));

        cut.Find("label").TextContent.Should().Contain("Rating");
    }

    [Fact]
    public void Renders_CorrectNumberOfStars()
    {
        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Value, 0)
             .Add(c => c.MaxRating, 5));

        cut.FindAll(".arcadia-field__rating-star").Should().HaveCount(5);
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, 3));

        var stars = cut.FindAll(".arcadia-field__rating-star");
        stars.Should().AllSatisfy(s => s.HasAttribute("disabled").Should().BeTrue());
    }

    [Fact]
    public void ValueChanged_FiresOnClick()
    {
        int newValue = 0;

        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Value, 0)
             .Add(c => c.ValueChanged, (int v) => newValue = v));

        cut.FindAll(".arcadia-field__rating-star")[3].Click();

        newValue.Should().Be(4);
    }

    [Fact]
    public void ActiveStars_MatchValue()
    {
        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Value, 3)
             .Add(c => c.MaxRating, 5));

        var stars = cut.FindAll(".arcadia-field__rating-star");
        stars.Count(s => s.ClassList.Contains("arcadia-field__rating-star--active")).Should().Be(3);
    }

    [Fact]
    public void ClearButton_Visible_WhenValueSet()
    {
        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Value, 3)
             .Add(c => c.ShowClear, true));

        cut.FindAll(".arcadia-field__rating-clear").Should().HaveCount(1);
    }

    [Fact]
    public void ClearButton_Hidden_WhenValueIsZero()
    {
        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Value, 0)
             .Add(c => c.ShowClear, true));

        cut.FindAll(".arcadia-field__rating-clear").Should().HaveCount(0);
    }

    [Fact]
    public void ClearButton_ResetsToZero()
    {
        int newValue = 3;

        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Value, 3)
             .Add(c => c.ShowClear, true)
             .Add(c => c.ValueChanged, (int v) => newValue = v));

        cut.Find(".arcadia-field__rating-clear").Click();

        newValue.Should().Be(0);
    }

    [Fact]
    public void CustomMaxRating_RendersCorrectStars()
    {
        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Value, 0)
             .Add(c => c.MaxRating, 10));

        cut.FindAll(".arcadia-field__rating-star").Should().HaveCount(10);
    }

    [Fact]
    public void HasRole_Radiogroup()
    {
        var cut = Render<RatingField>(p =>
            p.Add(c => c.Label, "Rating")
             .Add(c => c.Value, 0));

        cut.Find("[role='radiogroup']").Should().NotBeNull();
    }
}
