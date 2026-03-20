namespace HelixUI.FormBuilder.Schema;

/// <summary>
/// The type of field to render in the form.
/// </summary>
public enum FieldType
{
    /// <summary>Single-line text input.</summary>
    Text,

    /// <summary>Numeric input.</summary>
    Number,

    /// <summary>Multi-line text input.</summary>
    TextArea,

    /// <summary>Dropdown/select input.</summary>
    Select,

    /// <summary>Checkbox (boolean) input.</summary>
    Checkbox,

    /// <summary>Radio button group.</summary>
    RadioGroup,

    /// <summary>Date picker.</summary>
    Date,

    /// <summary>Toggle switch (boolean).</summary>
    Switch,

    /// <summary>File upload.</summary>
    File,

    /// <summary>Autocomplete/combobox.</summary>
    Autocomplete,

    /// <summary>Repeating section with add/remove rows.</summary>
    Repeater
}
