using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Settings;

namespace FSharp.VisualStudio.Extension
{
#pragma warning disable VSEXTPREVIEW_SETTINGS // The settings API is currently in preview and marked as experimental

    internal static class FSharpExtensionSettings
    {
        public const string OLD = "old";
        public const string LSP = "lsp";
        public const string BOTH = "both";
        public const string UNSET = "unset";

        public static EnumSettingEntry[] ExtensionChoice { get; } =
        [
            new(OLD, "%FSharpSettings.Old%"),
            new(LSP, "%FSharpSettings.LSP%"),
            new(BOTH, "%FSharpSettings.Both%"),
            new(UNSET, "%FSharpSettings.Unset%"),
        ];


        [VisualStudioContribution]
        public static SettingCategory FSharpCategory { get; } = new("fsharp", "%F#%");

        [VisualStudioContribution]
        public static Setting.Enum GetDiagnosticsFrom { get; } = new(
            "getDiagnosticsFrom",
            "%FSharpSettings.GetDiagnosticsFrom%",
            FSharpCategory,
            ExtensionChoice,
            defaultValue: UNSET)
        {
            Description = "%Which extension should be used to provide diagnostics%",
        };

        [VisualStudioContribution]
        public static Setting.Enum GetSemanticHighlightingFrom { get; } = new(
            "getSemanticHighlightingFrom",
            "%FSharpSettings.GetSemanticHighlightingFrom%",
            FSharpCategory,
            ExtensionChoice,
            defaultValue: UNSET)
        {
            Description = "%Which extension should be used to provide semantic highlighting%",
        };

        public static Setting<string>[] AllStringSettings { get; } =
        [
            GetDiagnosticsFrom,
            GetSemanticHighlightingFrom,
        ];
    }
}
