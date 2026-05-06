using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace XamlDiff.Library.Models;

/// <summary>
/// Generates XAML visual state setter markup by comparing source and destination XAML trees.
/// </summary>
public sealed partial class Diff : ObservableObject {

    private const string XamlNs = "http://schemas.microsoft.com/winfx/2006/xaml";

    /// <summary>
    /// Gets the collection of errors encountered while parsing or generating the diff.
    /// </summary>
    public ObservableCollection<string> Errors { get; set; } = [];

    /// <summary>
    /// Gets or sets the generated XAML output.
    /// </summary>
    [ObservableProperty]
    public partial string Output { get; set; } = string.Empty;


    /// <summary>
    /// Compares the source and destination XAML and generates visual state setters for differences between named
    /// elements.
    /// </summary>
    /// <param name="source">The source XAML markup.</param>
    /// <param name="destination">The destination XAML markup.</param>
    /// <param name="includeSource">
    /// True to merge the generated visual states into the source XAML; otherwise, only the generated markup is
    /// returned.
    /// </param>
    /// <param name="generateVisualStateGroups">
    /// True to include the <c> VisualStateManager.VisualStateGroups</c> wrapper.
    /// </param>
    /// <param name="generateVisualStateGroup">True to include a <c>VisualStateGroup</c> wrapper.</param>
    /// <param name="generateVisualState">True to include a <c>VisualState</c> wrapper.</param>
    /// <param name="generateStateTrigger">True to include a default <c>StateTrigger</c>.</param>
    public void Generate(string source, string destination, bool includeSource, bool generateVisualStateGroups, bool generateVisualStateGroup, bool generateVisualState, bool generateStateTrigger) {
        Errors.Clear();
        try {
            var xmlSource = XElement.Parse(source);
            try {
                var xmlDest = XElement.Parse(destination);

                var namedSourceElements = xmlSource.DescendantsAndSelf().Where(el => el.Attributes().Any(IsNameAttribute)).ToArray();
                var namedDestElements = xmlDest.DescendantsAndSelf().Where(el => el.Attributes().Any(IsNameAttribute)).ToArray();
                int indent = 0;

                if (namedSourceElements.Length == 0) {
                    Errors.Add("Source has no named elements, only elements with x:Names can be compared.");
                }
                if (namedDestElements.Length == 0) {
                    Errors.Add("Destination has no named elements, only elements with x:Names can be compared.");
                }

                var sb = new StringBuilder(4000);
                if (includeSource || generateVisualStateGroups) {
                    string ns = string.Empty;
                    if (includeSource) {
                        ns = "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"";
                    }
                    sb.AppendLine($"{Indent(indent++)}<VisualStateManager.VisualStateGroups {ns}>");
                }
                if (includeSource || generateVisualStateGroup) {
                    sb.AppendLine($"{Indent(indent++)}<VisualStateGroup x:Name=\"visualStateGroup1\">");
                }
                if (includeSource || generateVisualState) {
                    sb.AppendLine($"{Indent(indent++)}<VisualState x:Name=\"visualState1\">");
                }

                sb.AppendLine($"{Indent(indent)}<VisualState.Setters>");

                foreach (XElement itemSource in namedSourceElements) {
                    var itemSourceName = itemSource.Attributes().First(IsNameAttribute).Value;
                    var itemDest = namedDestElements.FirstOrDefault(el => el.Attributes().Any(a => IsNameAttribute(a) && a.Value == itemSourceName));

                    if (itemDest is null) {
                        Errors.Add($"Destination has no element with the x:Names '{itemSourceName}'");
                    } else {
                        AddState(itemSourceName, sb, itemSource, itemDest, indent);
                    }
                }
                sb.AppendLine($"{Indent(indent)}</VisualState.Setters>");

                if (generateStateTrigger) {
                    sb.AppendLine($"{Indent(indent)}<VisualState.StateTriggers>");
                    sb.AppendLine($"{Indent(indent + 1)}<StateTrigger IsActive=\"False\" />");
                    sb.AppendLine($"{Indent(indent)}</VisualState.StateTriggers>");
                }

                if (includeSource || generateVisualState) {
                    sb.AppendLine($"{Indent(--indent)}</VisualState>");
                }
                if (includeSource || generateVisualStateGroup) {
                    sb.AppendLine($"{Indent(--indent)}</VisualStateGroup>");
                }
                if (includeSource || generateVisualStateGroups) {
                    sb.AppendLine($"{Indent(--indent)}</VisualStateManager.VisualStateGroups>");
                }

                if (!includeSource) {
                    Output = sb.ToString();
                } else {
                    var root = xmlSource.Nodes().OfType<XElement>().FirstOrDefault(n => !n.Name.LocalName.Contains('.', StringComparison.CurrentCulture));
                    if (root != null) {
                        XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                        var vsg = root.Element(ns + "VisualStateManager.VisualStateGroups");
                        var vsg2 = XElement.Parse(sb.ToString());
                        if (vsg == null) {
                            root.AddFirst(vsg2);
                        } else {
                            vsg.Add(vsg2.FirstNode);
                        }
                        vsg2.Attributes().Where(e => e.IsNamespaceDeclaration).Remove();
                        Output = xmlSource.ToString();
                    }
                }

            } catch (Exception ex) {
                Errors.Add($"Destination is invalid XAML - {ex.Message}");
            }
        } catch (Exception ex) {
            Errors.Add($"Source is invalid XAML - {ex.Message}");
        }
    }

    /// <summary>
    /// Recursively compares source and destination elements and appends generated setters for changed, added, or
    /// removed values.
    /// </summary>
    /// <param name="name">The XAML target name used in generated setter targets.</param>
    /// <param name="sb">The string builder that receives generated markup.</param>
    /// <param name="itemSource">The source element.</param>
    /// <param name="itemDest">The destination element.</param>
    /// <param name="indentSize">The current indentation level.</param>
    private static void AddState(string name, StringBuilder sb, XElement? itemSource, XElement itemDest, int indentSize) {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(sb);
        ArgumentNullException.ThrowIfNull(itemDest);

        if (itemSource?.Name.LocalName is "VisualStateGroup" or "VisualState") {
            return;
        }

        string indent = Indent(indentSize + 1);
        if (itemSource is not null) {
            foreach (var aSource in itemSource.Attributes() ?? []) {
                var aDest = itemDest.Attributes().FirstOrDefault(a => a.Name == aSource.Name);

                if (aDest == null) {
                    var name2 = $"{itemSource.Name}.{aSource.Name}";
                    var xx = itemDest.Elements(name2).ToArray();
                    if (xx.Length == 0) {
                        // Remove deleted attributes by resetting them to their default value.
                        var n = aSource.Name.ToString().Contains('.') ? aSource.Name.ToString() : $"{itemSource.Name.LocalName}.{aSource.Name}";
                        var v = GetEmptyValue(n, aSource.Value, aSource.Name);
                        sb.AppendLine($"""{indent}<Setter Target="{name}.({n})" Value="{v}" />""");
                    } else {
                        if (xx[0].Value != aSource.Value) {
                            sb.AppendLine($"""{indent}<Setter Target="{name}.({name2})" Value="{xx[0].Value}" />""");
                        }
                    }
                } else if (aSource.Value != aDest.Value) {
                    // Apply changed attributes.
                    var n = aSource.Name.ToString().Contains('.') ? aSource.Name.ToString() : $"{itemSource.Name.LocalName}.{aSource.Name}";
                    sb.AppendLine($"""{indent}<Setter Target="{name}.({n})" Value="{WebUtility.HtmlEncode(aDest.Value)}" />""");
                }
            }
        }

        if (itemDest is not null) {

            foreach (var aDest in itemDest.Attributes() ?? []) {
                var aSource = itemSource?.Attributes().FirstOrDefault(a => a.Name == aDest.Name);

                if (aSource == null) {

                    var name2 = $"{itemDest.Name}.{aDest.Name}";
                    var xx = itemSource?.Elements(name2).ToArray();
                    if (xx?.Length == 0) {
                        // Add newly introduced attributes.
                        var n = aDest.Name.ToString().Contains('.') ? aDest.Name.ToString() : $"{itemDest.Name.LocalName}.{aDest.Name}";
                        sb.AppendLine($"""{indent}<Setter Target="{name}.({n})" Value="{WebUtility.HtmlEncode(aDest.Value)}" />""");
                    } else {
                        if (xx?[0].Value != aDest.Value) {
                            sb.AppendLine($"""{indent}<Setter Target="{name}.({name2})" Value="{aDest.Value}" />""");
                        }
                    }
                }
            }
        }

        foreach (var subItemSource in itemSource?.Elements() ?? []) {
            var subItemDest = itemDest?.Elements().FirstOrDefault(el => el.Name == subItemSource.Name);

            if (subItemDest is null) {
                //sb.AppendLine($"{indent}<Setter Target=\"{name}.({subItemSource.Name.LocalName})\" Value=\"{{x:Null}}\" />");
            } else {
                if (subItemSource.Value != subItemDest.Value) {
                    var s = name;
                    if (subItemDest.Name.LocalName.Contains('.')) {
                        s += $".({subItemDest.Name.LocalName})";
                        AddState(s, sb, subItemSource, subItemDest, indentSize);
                    }
                }
            }
        }

        foreach (XElement subItemDest in itemDest?.Elements() ?? []) {
            var subItemSource = itemSource?.Elements().FirstOrDefault(el => el.Name == subItemDest.Name);

            if (subItemSource == null) {
                var s = name;
                if (subItemDest.Name.LocalName.Contains('.')) {
                    s += $".({subItemDest.Name.LocalName})";
                }
                AddState(s, sb, null, subItemDest, indentSize);
            }
        }

        if (itemSource is not null && itemDest is not null && !itemSource.HasElements && !itemDest.HasElements && itemSource.Value != itemDest.Value) {
            sb.AppendLine($"""{indent}<Setter Target="{name}" Value="{WebUtility.HtmlEncode(itemDest.Value)}" />""");
        }
    }

    /// <summary>
    /// Creates indentation using four spaces per level.
    /// </summary>
    /// <param name="size">The indentation level.</param>
    /// <returns>A string containing indentation spaces.</returns>
    private static string Indent(int size) => string.Create(length: size * 4, ' ', (destination, c) => destination.Fill(c));

    /// <summary>
    /// Returns the default or empty value for a removed XAML property.
    /// </summary>
    /// <param name="n">The fully qualified XAML property name.</param>
    /// <param name="value">The original property value.</param>
    /// <param name="name">The XML attribute name.</param>
    /// <returns>A string representing the value that resets the property.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Improves readability")]
    private static string GetEmptyValue(string n, string value, XName name) {
        var pos = n.IndexOf('.');
        if (pos > -1) {
            // Read DefaultValue from the DependencyProperty

            // UWP
            var type = typeof(Control);
            var tn = $"Microsoft.UI.Xaml.Controls.{n[..pos]}";
            type = type.Assembly.GetTypes().FirstOrDefault(t => t.FullName == tn);

            if (type != null) {
                if (!type.GetProperty(n[(pos + 1)..])?.GetType().IsValueType == true) {
                    return "{x:Null}";
                }

                try {
                    var dpn = $"{n[(pos + 1)..]}Property";
                    var pi = type.GetProperty(dpn, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
                    if (pi != null) {
                        var dp = pi.GetValue(null) as DependencyProperty;
                        
                        // TODO: The next line crashes, find alternative

                        //var dv = dp?.GetMetadata(type).DefaultValue.ToString();
                        //return dv is null || dv.Contains("System.__ComObject") ? "{x:Null}" : dv;
                    }
                } catch { // Ignore
                }
            }
        }

        // Fallback if Property not found using Reflection

        if (n.EndsWith(".Margin") ||
            n.EndsWith("Thickness") ||
            n.EndsWith("Span") ||
            n.EndsWith(".Top") ||
            n.EndsWith(".Left") ||
            n.EndsWith(".Row") ||
            n.EndsWith(".Column")) {
            return "0";
        }

        if (n.EndsWith("Width") ||
            n.EndsWith("Height")) {
            return "NaN";
        }

        if (double.TryParse(value, out var d)) {
            return "0";
        }

        var isTrue = value is "True" or "true";
        if (isTrue && n.EndsWith("WithPanel")) {
            return "False";
        }

        return string.Empty;
    }

    /// <summary>
    /// Determines whether an attribute is an <c> x:Name</c> attribute.
    /// </summary>
    /// <param name="a">The attribute to inspect.</param>
    /// <returns><see langword="true"/> if the attribute is <c>x:Name</c>; otherwise, <see langword="false"/>.</returns>
    private static bool IsNameAttribute(XAttribute a) => a.Name.NamespaceName == XamlNs && a.Name.LocalName == "Name";
}
