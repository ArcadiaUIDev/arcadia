using Bunit;
using FluentAssertions;
using Arcadia.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Arcadia.Tests.Unit.Notifications;

public class ArcadiaToastContainerTests : Arcadia.Tests.Unit.ChartTestBase
{
    public ArcadiaToastContainerTests()
    {
        Services.AddScoped<ToastService>();
    }

    [Fact]
    public void Renders_EmptyContainer()
    {
        var cut = Render<ArcadiaToastContainer>();

        cut.Find(".arcadia-toast-container").Should().NotBeNull();
        cut.FindAll(".arcadia-toast").Should().BeEmpty();
    }

    [Fact]
    public void Renders_DefaultPosition_TopRight()
    {
        var cut = Render<ArcadiaToastContainer>();

        cut.Find(".arcadia-toast-container").ClassList.Should().Contain("arcadia-toast-container--top-right");
    }

    [Theory]
    [InlineData(ToastPosition.TopLeft, "arcadia-toast-container--top-left")]
    [InlineData(ToastPosition.BottomRight, "arcadia-toast-container--bottom-right")]
    [InlineData(ToastPosition.BottomCenter, "arcadia-toast-container--bottom-center")]
    public void Renders_WithPositionClass(ToastPosition position, string expectedClass)
    {
        var cut = Render<ArcadiaToastContainer>(p => p.Add(c => c.Position, position));

        cut.Find(".arcadia-toast-container").ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void Shows_Toast_WhenServiceAdds()
    {
        var toastService = Services.GetRequiredService<ToastService>();
        var cut = Render<ArcadiaToastContainer>();

        toastService.ShowInfo("Hello world");

        cut.FindAll(".arcadia-toast").Should().HaveCount(1);
    }

    [Fact]
    public void Shows_MultipleToasts()
    {
        var toastService = Services.GetRequiredService<ToastService>();
        var cut = Render<ArcadiaToastContainer>();

        toastService.ShowInfo("First");
        toastService.ShowSuccess("Second");
        toastService.ShowWarning("Third");

        cut.FindAll(".arcadia-toast").Should().HaveCount(3);
    }

    [Fact]
    public void Dismiss_RemovesToast()
    {
        var toastService = Services.GetRequiredService<ToastService>();
        var cut = Render<ArcadiaToastContainer>();

        var id = toastService.ShowInfo("Will be dismissed");
        cut.FindAll(".arcadia-toast").Should().HaveCount(1);

        toastService.Dismiss(id);
        cut.FindAll(".arcadia-toast").Should().BeEmpty();
    }

    [Fact]
    public void Renders_WithCustomClass()
    {
        var cut = Render<ArcadiaToastContainer>(p => p.Add(c => c.Class, "my-toasts"));

        cut.Find(".arcadia-toast-container").ClassList.Should().Contain("my-toasts");
    }
}
