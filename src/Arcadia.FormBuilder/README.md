<p align="center">
  <strong>Arcadia.FormBuilder</strong><br>
  <em>21 field types, schema-driven forms, and multi-step wizards for Blazor</em>
</p>

## Quick Start

```bash
dotnet add package Arcadia.FormBuilder
dotnet add package Arcadia.Theme
```

```html
<link href="_content/Arcadia.FormBuilder/css/arcadia-forms.css" rel="stylesheet" />
```

## Three Ways to Build Forms

**1. Individual fields** — drop in exactly what you need:
```razor
<TextField Label="Name" @bind-Value="name" Required="true" />
<SelectField Label="Country" @bind-Value="country" Options="@countries" />
<DateField Label="Birthday" @bind-Value="dob" />
```

**2. Schema-driven** — define the form as data, render it dynamically:
```razor
<ArcadiaFormBuilder Schema="@schema" State="@state" OnValidSubmit="HandleSubmit" />
```

**3. Model-driven** — annotate a C# class, get a form:
```csharp
public class ContactForm
{
    [ArcadiaField(Type = FieldType.Text, Label = "Name")]
    [Required]
    public string Name { get; set; }
}
```

## 21 Field Types

Text · Password · Email · TextArea · Number · Select · Radio · Checkbox · Switch · Date · Time · DateTime · Slider · Rating · Color · File · Masked · Autocomplete · Tags · RichText · Hidden

## Features

- **Validation** — built-in rules, FluentValidation, async validators
- **Conditional fields** — show/hide fields based on other field values
- **Wizard mode** — multi-step forms with progress bar and per-step validation
- **Accessibility** — all fields have proper labels, error messages, and ARIA attributes

**[Docs](https://arcadiaui.com/docs/forms)** · **[GitHub](https://github.com/ArcadiaUIDev/arcadia)**
