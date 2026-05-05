using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace XamlDiff.Models;

public sealed partial class Diff : ObservableObject {

    private const string XamlNs = "http://schemas.microsoft.com/winfx/2006/xaml";

    public ObservableCollection<string> Errors { get; set; } = new();

    [ObservableProperty]
    public partial string Output { get; set; } = string.Empty;


    public void Generate(string source, string destination, bool includeSource, bool generateVisualStateGroups, bool generateVisualStateGroup, bool generateVisualState, bool generateStateTrigger) {
        Errors.Clear();
        try {
            var xmlSource = XElement.Parse(source);
            try {
                var xmlDest = XElement.Parse(destination);

                var namedSourceElements = xmlSource.DescendantsAndSelf().Where(el => el.Attributes().Any(IsNameAttribute)).ToList();
                var namedDestElements = xmlDest.DescendantsAndSelf().Where(el => el.Attributes().Any(IsNameAttribute)).ToList();
                int indent = 0;

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
                    var itemDest = namedDestElements.First(el => el.Attributes().Any(a => IsNameAttribute(a) && a.Value == itemSourceName));

                    AddState(itemSourceName, sb, itemSource, itemDest, indent);
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

    private static void AddState(string name, StringBuilder sb, XElement itemSource, XElement itemDest, int indentSize) {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(sb);
        ArgumentNullException.ThrowIfNull(itemSource);
        ArgumentNullException.ThrowIfNull(itemDest);

        if (itemSource.Name.LocalName is "VisualStateGroup" or "VisualState") {
            return;
        }
        string indent = Indent(indentSize + 1);
        foreach (var aSource in itemSource.Attributes() ?? Enumerable.Empty<XAttribute>()) {
            var aDest = itemDest.Attributes().FirstOrDefault(a => a.Name == aSource.Name);

            if (aDest == null) {
                // Verwijderede attributen verwijderen (op default zetten)
                var n = aSource.Name.ToString().Contains('.') ? aSource.Name.ToString() : $"{itemSource.Name.LocalName}.{aSource.Name}";
                var v = GetEmptyValue(n, aSource.Value, aSource.Name);
                sb.AppendLine($"{indent}<Setter Target=\"{name}.({n})\" Value=\"{v}\" />");
            } else if (aSource.Value != aDest.Value) {
                // Gewijzigde attributen zetten
                var n = aSource.Name.ToString().Contains('.') ? aSource.Name.ToString() : $"{itemSource.Name.LocalName}.{aSource.Name}";
                sb.AppendLine($"{indent}<Setter Target=\"{name}.({n})\" Value=\"{WebUtility.HtmlEncode(aDest.Value)}\" />");
            }
        }



        foreach (var aDest in itemDest.Attributes() ?? Enumerable.Empty<XAttribute>()) {
            var aSource = itemSource.Attributes().FirstOrDefault(a => a.Name == aDest.Name);

            if (aSource == null) {
                // Toegevoegde attributen toevoegen
                var n = aDest.Name.ToString().Contains('.') ? aDest.Name.ToString() : $"{itemDest.Name.LocalName}.{aDest.Name}";
                sb.AppendLine($"{indent}<Setter Target=\"{name}.({n})\" Value=\"{WebUtility.HtmlEncode(aDest.Value)}\" />");
            }
        }


        foreach (var subItemSource in itemSource?.Elements() ?? Enumerable.Empty<XElement>()) {
            //var t = itemSource.Elements().IndexOf(subItemSource);
            //var subItemDest = itemDest?.Elements().Where(el => el.Name == subItemSource.Name).ElementAtOrDefault(t);
            var subItemDest = itemDest?.Elements().FirstOrDefault(el => el.Name == subItemSource.Name);

            if (subItemDest == null) {
                //Foo($"{name}", sb, subItemSource, null);

                sb.AppendLine($"{indent}<Setter Target=\"{name}.({subItemSource.Name.LocalName})\" Value=\"{{x:Null}}\" />");

            } else {
                var s = name;
                if (subItemDest.Name.LocalName.Contains('.')) {
                    s += $".({subItemDest.Name.LocalName})";
                    AddState(s, sb, subItemSource, subItemDest, indentSize);
                }
            }
        }

        foreach (var subItemDest in itemDest?.Elements() ?? Enumerable.Empty<XElement>()) {
            var subItemSource = itemSource?.Elements().FirstOrDefault(el => el.Name == subItemDest.Name);

            if (subItemSource == null) {
                var s = name;
                if (subItemDest.Name.LocalName.Contains('.')) {
                    s += $".({subItemDest.Name.LocalName})";
                }
                AddState(s, sb, null, subItemDest, indentSize);
            }
        }

        if (!itemSource.HasElements && itemSource.Value != itemDest.Value) {
            sb.AppendLine($"{indent}<Setter Target=\"{name} Value=\"{WebUtility.HtmlEncode(itemDest.Value)}\" />");
        }
    }

    private static string Indent(int size) => string.Create(length: size * 4, ' ', (destination, c) => destination.Fill(c));

    private static string GetEmptyValue(string n, string value, XName name) {
        var pos = n.IndexOf('.');
        if (pos > -1) {
            // Read DefaultValue from the DependencyProperty

            // UWP
            var type = typeof(Control);
            var tn = $"Microsoft.UI.Xaml.Controls.{n[..pos]}";
            type = type.Assembly.GetTypes().FirstOrDefault(t => t.FullName == tn);
            if (type != null) {
                try {
                    var dpn = $"{n.Substring(pos + 1)}Property";
                    var pi = type.GetProperty(dpn, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
                    if (pi != null) {
                        var dp = pi.GetValue(null) as DependencyProperty;

                        var dv = dp.GetMetadata(type).DefaultValue.ToString();
                        return dv.Contains("System.__ComObject") ? "{x:Null}" : dv;
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

    private static bool IsNameAttribute(XAttribute a) => a.Name.NamespaceName == XamlNs && a.Name.LocalName == "Name";
}
