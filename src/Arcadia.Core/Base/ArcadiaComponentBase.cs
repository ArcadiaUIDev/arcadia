using Microsoft.AspNetCore.Components;

namespace Arcadia.Core.Base;

/// <summary>
/// Base class for all Arcadia Controls components. Provides common parameters for
/// CSS classes, inline styles, and arbitrary HTML attributes.
/// </summary>
public abstract class ArcadiaComponentBase : ComponentBase, Abstractions.IHasClass, Abstractions.IHasStyle
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the component's root element.
    /// These are appended to the component's default classes, never replacing them.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets inline styles to apply to the component's root element.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied
    /// to the component's root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the component's unique element ID for accessibility purposes.
    /// Lazily generated on first access.
    /// </summary>
    protected string ElementId => _elementId ??= Utilities.IdGenerator.Generate();
    private string? _elementId;

    /// <summary>
    /// <see cref="AdditionalAttributes"/> with Blazor generic-parameter names (TItem, TValue,
    /// TKey, TSource) filtered out so they don't leak into the rendered DOM. Generic Razor
    /// components frequently get type arguments specified via attribute syntax (e.g.
    /// <c>&lt;ArcadiaLineChart TItem="DataPoint"&gt;</c>); when the class declares the
    /// generic with a different name (e.g. <c>&lt;T&gt;</c>) the attribute falls through
    /// into <see cref="AdditionalAttributes"/> and is splatted onto the DOM. Use this
    /// property instead of <see cref="AdditionalAttributes"/> when splatting via
    /// <c>@attributes="..."</c>.
    /// </summary>
    protected IReadOnlyDictionary<string, object>? FilteredAttributes
    {
        get
        {
            if (AdditionalAttributes is null || AdditionalAttributes.Count == 0) return AdditionalAttributes;
            // Avoid allocating when there's nothing to filter
            bool anyGeneric = false;
            foreach (var key in AdditionalAttributes.Keys)
            {
                if (IsBlazorGenericParameterName(key)) { anyGeneric = true; break; }
            }
            if (!anyGeneric) return AdditionalAttributes;

            var filtered = new Dictionary<string, object>(AdditionalAttributes.Count);
            foreach (var kv in AdditionalAttributes)
            {
                if (!IsBlazorGenericParameterName(kv.Key))
                    filtered[kv.Key] = kv.Value;
            }
            return filtered;
        }
    }

    private static bool IsBlazorGenericParameterName(string key) =>
        key.Length >= 2 && key[0] == 'T' && char.IsUpper(key[1]);
}
