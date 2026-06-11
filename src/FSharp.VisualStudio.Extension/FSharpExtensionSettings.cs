using System.Linq;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Settings;

namespace FSharp.VisualStudio.Extension
{
#pragma warning disable VSEXTPREVIEW_SETTINGS // The settings API is currently in preview and marked as experimental

    internal static class FSharpExtensionSettings
    {
        public const string OLD = "old";
        public const string LSP = "lsp";
        public const string UNSET = "unset";

        // Semantic classification can only come from one source at a time
        // (the two would fight over the same spans), so there is no "both".
        public static EnumSettingEntry[] ExtensionEitherChoice { get; } =
        [
            new(OLD, "%FSharpSettings.Old%"),
            new(LSP, "%FSharpSettings.LSP%"),
            new(UNSET, "%FSharpSettings.Unset%"),
        ];


        [VisualStudioContribution]
        public static SettingCategory FSharpCategory { get; } = new("fsharp", "%F#%");

        [VisualStudioContribution]
        public static Setting.Enum SemanticClassificationProvider { get; } = new(
            "semanticClassificationProvider",
            "%FSharpSettings.SemanticClassificationProvider%",
            FSharpCategory,
            ExtensionEitherChoice,
            defaultValue: UNSET)
        {
            Description = "%FSharpSettings.SemanticClassificationProvider.Description%",
        };

        // Each setting carries the concrete value to persist (and treat as effective)
        // the first time it is read as UNSET. Semantic classification defaults to
        // OLD so the in-proc classifier stays in charge until the user opts in
        // (or is flipped into the LSP path by a controlled rollout).
        public static (Setting<string> Setting, string DefaultIfUnset)[] AllSettingsWithDefaults { get; } =
        [
            (SemanticClassificationProvider, OLD),
        ];

        public static Setting<string>[] AllStringSettings { get; } =
            AllSettingsWithDefaults.Select(x => x.Setting).ToArray();
    }
}
