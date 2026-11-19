# Visual Studio integration

Paths: `vsintegration/**`. Tests: `vsintegration/tests/**`.

Review `FSharpLanguageDebugInfoService` and `FSharpBreakpointResolutionService` as Visual Studio debugger services, keeping their host types outside `src/Compiler/Service` and `src/Compiler/Symbols`.

- Keep Visual Studio, MEF, shell, and Windows UI types in the host; do not push them into FCS, and do not move shared compiler sources into the VS tree.
- Do not block the UI thread, and do not call a UI-thread host service from a background callback.
- An editor feature must not let an exception reach the Visual Studio UI.
- Prefer the host's own option, search, tooltip, theme, and analysis-scope services over a private copy or a fixed delay.
- Interactive editor work must be cancellable and tied to the current document version or query.
- A cache entry must stay findable until it is evicted, and an idle editor cache must stay collectable.
- Editor behavior that depends on a language-rule relaxation must follow language version, not apply to every project.
- A green compiler suite does not prove a host feature; the editor test has to call the host provider and cover the shapes the feature claims.
- An editor callback must not call `Async.RunSynchronously`.
- A keystroke must not call `BackgroundCompiler.InvalidateConfiguration` when `FSharpProjectOptions.AreSameForChecking` is true.
- Separate cancellation from logged faults at IDE entry points, as `FSharpNavigation.TryGoToDefinition` and `RoslynHelpers.StartAsyncAsTask` do.

### UI thread affinity

A host call that requires the UI thread runs off it and deadlocks, or work that does not need the UI thread blocks it.

**CHECK:**
- A host call that requires the UI thread, including a project-site read or update, language-service creation, interactive-session startup, and options-page activation, must run on that thread.
- Keep calls guarded by `UIThread.MustBeCalledFromUIThread` on the UI thread and preserve `FSharpProjectNode`’s Opening-state `GetProjectSite` thread check.
- In `FSharpPackage.RegisterInitializeAsyncWork`, retain `AddTask(false, ...)` for non-UI setup and `AddTask(true, ...)` for FSI and UI-service initialization.
- Resolve `XMLMemberIndexService` in `FSharpAsyncQuickInfoSourceProvider.TryCreateQuickInfoSource`, not in `FSharpAsyncQuickInfoSource.GetQuickInfoItemAsync`.
- Move the UI-thread switch outside the per-project setup loop, and do not start every project load at once when each load posts workspace updates to the UI thread.
- Work that deadlocks off the UI thread, including interactive-session startup, must stay on the foreground init path. Background package initialization must not take that work with it.

**Severity:** critical.

**Hotspots:** `vsintegration/src/FSharp.Editor/Common/**`, `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.Editor/QuickInfo/**`, `vsintegration/src/FSharp.Editor/Options/**`, `vsintegration/src/FSharp.LanguageService/**`, `vsintegration/src/FSharp.ProjectSystem.FSharp/**`, `vsintegration/src/FSharp.ProjectSystem.Base/**`, `vsintegration/src/FSharp.VS.FSI/**`, `vsintegration/src/FSharp.UIResources/**`

### MEF composition and package load

The editor does not compose, another language loads F# visuals, or the host runs resources that were never embedded in the package.

**CHECK:**
- Declare MEF exports in an assembly the component model already composes. Do not import a part into a package that is not itself composed, and do not export a host settings service that is not a MEF part.
- Restrict `WpfFSharpStyleFactory`’s `TypeConversion` metadata to `FSharpStyle` rather than relying on internal CLR visibility to restrict MEF activation.
- One-time color or theme setup belongs on the language-service composition root, not on a chain of parts that may never be instantiated. Set workspace options once when the language service initializes, not on every new text view.
- Keep `EditorOptions` and `SettingsStore.ISettingsStore` available through composition, with `SettingsStore.Create` selecting the persistent or in-memory store instead of a mutable static singleton.
- Load styles through `UIResources.NavStyles`. `AbstractOptionPage` must get settings from `SettingsStore.ISettingsStore`, not from `IVsSolution`.
- Do not dynamically load XAML resources, and do not enable a preserialized resource form the component merger cannot read as a blanket project property. A resource change must be embedded in the extension package, and the steps must work from a clean tree; one IDE build is not enough.
- Removing an unused host interface must leave the editor able to start. Shared sources used by more than one product stay with the compiler; a project file may retarget a link. Reuse the existing editor helper for the same lookup instead of a second copy.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/*.{fs,fsproj,resx}`, `vsintegration/src/FSharp.Editor/Common/**`, `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.Editor/Options/**`, `vsintegration/src/FSharp.Editor/xlf/**`, `vsintegration/src/FSharp.UIResources/**`, `vsintegration/src/FSharp.ProjectSystem.FSharp/**`, `vsintegration/Vsix/**`

### Solution and project lifetime

Project setup runs during solution load, a close leaves the previous solution cached, or a script or path is not the project the user loaded.

**CHECK:**
- Defer project setup until `LegacyProjectWorkspaceMap.OnAfterOpenSolution`; do not replace that callback with a delay.
- Do not set up projects while the solution is loading, on the first open or on a later open of the same solution, and do not run a document analyzer on a temporary project the host creates before the solution has loaded.
- Register projects as the solution reports them, after solution open, instead of waiting for a document to open.
- Clear owned caches in `FSharpSolutionEvents.OnAfterCloseSolution` and pending setups in `LegacyProjectWorkspaceMap.OnAfterCloseSolution`, without using delays to hide project-disposal races.
- Project-file synchronization must be atomic across a host callback that can fire twice, and a lookup in a concurrent project table must be one atomic try-get.
- Treat script files as ordinary projects, analyze a closed script in the solution, and take symbol information from the project system so navigation does not need a special path.
- Normalize a path that contains a parent segment before matching it to a project document. When the solution contains the source of a referenced framework assembly, other projects must bind the assembly they reference, not that source project.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.Editor/Build/**`, `vsintegration/src/FSharp.Editor/Diagnostics/**`, `vsintegration/src/FSharp.LanguageService/**`, `vsintegration/src/FSharp.ProjectSystem.FSharp/**`, `vsintegration/src/FSharp.ProjectSystem.Base/**`

### Stable editor caches

A cache key cannot be found again, so entries accumulate, or an idle cache stays rooted and cannot be collected.

**CHECK:**
- A cache key's hash and equality must stay stable for as long as the entry is stored. Hash a long-lived key from source identity that does not change until the user edits, not from type resolutions that are still being solved.
- A cache must be able to find the entry it stored. An eviction that cannot find the stored key is not eviction and must not be the only bound on the cache; keep a separate candidate list.
- A background eviction agent must be shared and must not retain the caches that post work to it. A cache must not own that agent's lifetime, and an idle cache must stay collectable without a finalizer.
- A shared editor cache must not recycle mutable entities across concurrent callers. A missed eviction must be its own observable failure, not a host hang timeout.
- Do not force one storage strategy onto both one-off compilation and incremental editor use, and an editor session must be able to override a language-feature gate that would disable the incremental path. Do not keep a separate no-eviction path for one-off compilation when one bounded cache with headroom covers both modes.
- Background cache maintenance must not use a concurrency primitive that exhausts threads, wait handles, or memory. An editor cache that reuses a compilation fix must keep that fix's gain and must not regress typical incremental or standalone builds.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Common/**`, `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.LanguageService/**`

### Quick info presentation

Quick info is unreadable, wraps through a fake divider, shows signature-help text, or is built from a Windows-only element.

**CHECK:**
- Keep Quick Info on `FSharpAsyncQuickInfoSource.TryGetToolTip` and hints on their existing symbol/type-formatting helpers without substituting signature-help parameter values.
- Use `FSharpStyle.Separator` through `QuickInfoViewProvider.stackWithSeparators` and `WpfFSharpStyleFactory`, retaining `XmlDocumentation.separatorText` only as the text fallback.
- Honor `QuickInfoOptions.ShowRemarks` and use `getLinkTooltip`’s shorter project-relative or available solution-relative path for navigation tooltips.
- Custom tooltip content must reuse the host quick-info controller so show and dismiss stay identical to the platform. Do not keep a session capture whose only job was to dismiss the tooltip on navigation once the host already does that.
- `QuickInfoViewProvider.provideContent` builds `Microsoft.VisualStudio.Text.Adornments.ClassifiedTextElement` content with Roslyn `ClassificationTypeNames`, not a custom Windows-only element.
- Pass `QuickInfoOptions.DescriptionWidth` to `TryGetToolTip` and preserve section `LineBreak` tokens and `ContainerElementStyle.Wrapped` in `QuickInfoViewProvider`.

**Severity:** medium.

Use `vsintegration/src/FSharp.Editor/{QuickInfo,DocComments,Hints,Options}/**` and `vsintegration/src/FSharp.UIResources/**` as the Quick Info presentation hotspots.

### Navigate to the loaded definition

Navigation opens a window itself, jumps to metadata or the wrong linked copy, or offers a link that goes nowhere.

**CHECK:**
- A definition provider returns a navigable item. `GoToDefinition.NavigateToItem` and `GoToDefinition.TryNavigateToTextSpan` open the document.
- Navigation must be able to return both the signature and the implementation. From an implementation, try the implementation and fall back to the signature span. A failed lookup must not leave a link that does nothing.
- When quick info is already on a definition, do not offer a link that navigates nowhere. A link that is shown must toggle between signature and implementation the same way go-to-definition does.
- Whether a tooltip symbol is navigable must be decided against the loaded workspace, not assumed from the symbol range alone. Navigation must reach types that are only inferred, and a symbol defined in another loaded project must resolve. A startup sentinel, identified by the existing range value rather than a copied filename, must not be offered as a target.
- Navigation to a file linked into several projects must select the document for the project that owns the symbol, and must not reuse an already-open document from a different project.
- Metadata-as-source must use a separate workspace, created directly, and must not depend on shell document services or the user's current workspace.
- Go-to-all and solution search must accept a container or namespace qualifier, keep fuzzy matching, and rank an exact dotted-name match ahead of a longer match without hiding either. Do not sort one language's results inside the language service when the host already ranks every language together. Use the host pattern matcher, and do not add match-span highlighting until the host navigation API can return those spans. Clear a transient navigation status when the request finishes, including when it fails.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Navigation/**`, `vsintegration/src/FSharp.Editor/QuickInfo/**`, `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.LanguageService/**`

### Completion filtering and commit

Completion offers items that cannot be declared there, removes items that are still legal, or commits an edit the host then overwrites.

**CHECK:**
- Auto-triggered completion must filter items by syntactic position. After a binding keyword, do not offer symbols that cannot be declared there, and do not auto-trigger where the user is declaring a new identifier.
- In a pattern position, completion must offer only constructs that are valid destructuring, not the general symbol list.
- In a pattern, rank items of the expected type first, but do not remove other items that are still legal to type. Remove only an item that cannot resolve in a pattern.
- `FSharpCompletionProvider.GetChangeAsync` commits `NameInCode`, including a generated override body when `GenerateBodyForOverriddenMethod` is enabled.
- Return every edit of a completion commit as the completion change for the document version the host will apply. A side edit is applied to a version the host then overwrites.
- Reuse the current `DeclarationListItem` values in `FSharpCompletionProvider`, displaying `NameInList` and committing `NameInCode` through `GetChangeAsync`.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Completion/**`, `vsintegration/src/FSharp.Editor/AutomaticCompletion/**`, `vsintegration/src/FSharp.Editor/CodeFixes/**`

### Diagnostics scope and language gates

A code fix matches a culture-specific message, a host analysis limit is never applied, or a language relaxation hits every project.

**CHECK:**
- A code fix must not identify a diagnostic by its displayed message. The diagnostic must carry structured data the fix can match in any culture.
- When a diagnostic payload already has more than one case, callers should read those fields directly rather than adding a separate type test.
- Keep `FSharpServiceTelemetry.otelExport` DEBUG-only and disabled unless `FSHARP_OTEL_EXPORT` has a nonempty value.
- Editor analyzers must follow the host's live-analysis scope setting. Do not invent a private active-document gate beside that setting.
- Apply `FSharpGlobalOptions.SetBackgroundAnalysisScope` from `FSharpPackage.RegisterInitializeAsyncWork` rather than placing the setting only in an unused initialization hook.
- A language-rule relaxation that editor tools depend on must be gated by language version and exposed in a form tools can read. An editor suggestion must not rewrite code into a form an older compiler warns about. A language feature that changes editing must be tryable behind a preview switch before it becomes the default.
- If a code-fix test fails because the diagnostic no longer reports that case, fix the diagnostic. A missed shape is not a reason to drop the test.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Diagnostics/**`, `vsintegration/src/FSharp.Editor/CodeFixes/**`, `vsintegration/src/FSharp.Editor/Refactor/**`, `vsintegration/src/FSharp.Editor/Options/**`

### Store each option on its own

Editor settings are persisted as one blob, so a new setting comes back as the zero value, or a setting cannot be found.

**CHECK:**
- A new field on an options record must survive `SettingsStore.SaveSettings` and `JsonConvert.PopulateObject` at its default.
- An options record bound by the settings page must have `[<CLIMutable>]`, and the markup must bind to public properties.
- An options refactor must keep each existing setting persisted. A setting that no longer round-trips is a regression even if the page still opens.
- An editor setting must have a resource string the options window can search. An optional numeric setting must accept an empty field as unset, and a control that is not wired to the checker must not be shown.
- Prefer a public host option when it exists. Do not build UI for a built-in host setting whose persistence is not public, and do not reference another language's internal options implementation or copy its page markup.
- Load editor settings once when they are registered, then update the cache from the persistence change event so running IDE instances stay in sync. Do not re-read on every access.
- Editor settings must not depend on a host-only service, or unit tests cannot set them. Supply a test store instead of scattering fallback values. Treat a stored options object as immutable outside the options dialog.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Options/**`, `vsintegration/src/FSharp.UIResources/**`, `vsintegration/src/FSharp.Editor/Common/**`

### Classification follows theme and role

A classification ignores the language role, or a baked-in foreground stays unreadable after the theme changes.

**CHECK:**
- In `getClassificationTypeName`, keep Function and Method mappings distinct while retaining the `IntrinsicFunction` and `ComputationExpression` keyword mappings.
- Punctuation that is still part of a string must keep the string classification. Classifying that range as a delimiter hides the string extent and needs a test for that range.
- Use `ThemeColors.GetColor` for custom format defaults and preserve the `fontAndColorStorage.GetItem` guard when applying theme updates.
- Classification colors must follow the active editor theme. A theme switch must keep the user's per-mode classification color customizations and must not overwrite them.
- Custom editor format definitions must be reapplied from the active theme at startup as well as on a theme switch, because the host persists the first instantiated defaults. A theme-specific foreground must not be baked into that default.

**Severity:** medium.

**Hotspots:** `vsintegration/src/FSharp.Editor/Classification/**`, `vsintegration/src/FSharp.Editor/Common/**`, `vsintegration/src/FSharp.LanguageService/**`

### Feature failures stay inside the feature

An exception from document check, Quick Info, analysis, or the options page reaches the Visual Studio UI.

**CHECK:**
- A failure while the editor checks a document must not reach the host. It needs a repro and a test; a green compiler suite does not cover that host failure.
- An uninitialized diagnostics logger must not escape as an exception from document check into editor features.
- Enabling an editor analysis feature must not let an exception reach the Visual Studio UI.
- Quick Info must not throw out to the shell. A failure while resolving a symbol, including a primitive, has to stay inside the feature.
- Opening the editor options page must not let a binding exception reach the Visual Studio UI. The settings type the page binds must stay in the shape that binding expects.
- Do not set a process-global resource assembly. Probe `FSharp.UIResources` XAML dictionaries with `ProvideBindingPath`. `ProvideCodeBase` does not load them.

**Severity:** critical.

**Hotspots:** `vsintegration/src/FSharp.Editor/Diagnostics/**`, `vsintegration/src/FSharp.Editor/QuickInfo/**`, `vsintegration/src/FSharp.Editor/Options/**`, `vsintegration/src/FSharp.UIResources/**`, `vsintegration/src/FSharp.Editor/xlf/**`, `vsintegration/src/FSharp.Editor/Common/**`

### Editor tests cover the host

An editor change is tested only through the service API, or only for a flat module-level shape, so the host failure stays invisible.

**CHECK:**
- An editor test must exercise the editor provider. A suite that only calls the service method does not cover the editor.
- Cover the declaration shapes the feature claims, including a binding nested in a type or in an expression, a dotted name in a nested pattern, and a union that is out of scope or requires qualified access, not only a flat module-level case.
- A known limitation of an editor feature may be recorded as a skipped test so the missing case stays visible. Do not leave that case with no test at all.
- Editor tests must reuse the cached `ExportProviderFactory` from `RoslynHelpers.fs`, not build a new composition per test.
- Put a test that needs host-internal types in the test project those internals are already visible to. Do not link a setup file into an editor test project that does not reference the utility project that makes that setup run.

**Severity:** medium.

**Hotspots:** `vsintegration/tests/FSharp.Editor.Tests/**`, `vsintegration/tests/UnitTests/**`, `vsintegration/tests/Salsa/**`

### Interactive work stays cancellable

A keystroke, search, or tooltip starts work that keeps running after the user has cancelled or moved on.

**CHECK:**
- Live-buffer edits must not start a non-cancellable parse for every keystroke. That work must be a debounced request for the current document version.
- User-started editor search must pass cancellation into the parallel scan. A parallel loop that ignores the request token keeps running after the search is abandoned.
- Solution search must stay cancellable, include operators as well as identifiers, and keep its cache for the solution lifetime, cleared on close, rather than a short-lived cache rebuilt on every query.
- Render icon, description, then documentation in `QuickInfoViewProvider.provideContent`, using `ClassifiedTextRun` links whose `FSharpNavigation.NavigateTo` fallback runs on lookup failure, never cancellation.
- Do not add a second cap on editor background parallelism when that work is already bounded. A second cap drifts from the bound callers already use, so cancellation and degree disagree.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Common/**`, `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.Editor/Navigation/**`, `vsintegration/src/FSharp.Editor/QuickInfo/**`, `vsintegration/src/FSharp.Editor/Diagnostics/**`


