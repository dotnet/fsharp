# Visual Studio integration

Paths: `vsintegration/**`. Tests: `vsintegration/tests/**`.

Editor, project system, legacy language service, FSI, debug adapter, MEF, and the UI thread. Do not push Visual Studio, MEF, JTF, or Roslyn types into `src/Compiler/Service` or `src/Compiler/Symbols`.

- Keep Visual Studio, MEF, shell, and Windows UI types in the host; do not push them into FCS, and do not move shared compiler sources into the VS tree.
- Do not block the UI thread, and do not call a UI-thread host service from a background callback.
- An editor feature must not let an exception reach the Visual Studio UI.
- Prefer the host's own option, search, tooltip, theme, and analysis-scope services over a private copy or a fixed delay.
- Interactive editor work must be cancellable and tied to the current document version or query.
- A cache entry must stay findable until it is evicted, and an idle editor cache must stay collectable.
- Editor behavior that depends on a language-rule relaxation must follow language version, not apply to every project.
- A green compiler suite does not prove a host feature; the editor test has to call the host provider and cover the shapes the feature claims.

### 10. IDE Responsiveness

The compiler service must respond to editor keystrokes without unnecessary recomputation.

**CHECK:**
- Use fully async code in the language service; avoid unnecessary `Async.RunSynchronously`.
- Verify changes do not trigger endless project rechecks.
- Evaluate the queue stress impact of every new FCS request type — each request blocks the service queue while running, so expensive requests delay all other IDE features.
- Caching must prevent duplicate work per project.
- Test IDE changes on large solutions before merging.

**Severity:** Endless recheck loop → **critical**. UI thread block → **high**. Missing trace verification → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/FSharp.Compiler.LanguageServer/`, `vsintegration/`

---

(Exception: top-level language service entry points should catch all to prevent IDE crashes — but log, don't silence.)

- At language service API boundaries (top-level entry points called by the IDE), catch all exceptions to prevent IDE crashes — but log them, don't silence them.

Cancellation hotspot: `vsintegration/`.

### UI thread affinity

A host call that requires the UI thread runs off it and deadlocks, or work that does not need the UI thread blocks it.

**CHECK:**
- A host call that requires the UI thread, including a project-site read or update, language-service creation, interactive-session startup, and options-page activation, must run on that thread.
- A newer host version is not evidence that project-system initialization left the UI thread. Keep that work on the UI thread until the host contract says otherwise.
- Package initialization and workspace updates that do not touch UI services must not take the UI thread, and a workspace project context whose host factory is built for the background must not stay on the UI thread.
- Resolve a UI-thread service when the editor part is created, on the main thread, through one main-thread or async lookup. Do not fetch it from a background tooltip or command callback, which deadlocks even when the result is unused.
- Move the UI-thread switch outside the per-project setup loop, and do not start every project load at once when each load posts workspace updates to the UI thread.
- Work that deadlocks off the UI thread, including interactive-session startup, must stay on the foreground init path. Background package initialization must not take that work with it.

**Severity:** critical.

**Hotspots:** `vsintegration/src/FSharp.Editor/Common/**`, `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.Editor/QuickInfo/**`, `vsintegration/src/FSharp.Editor/Options/**`, `vsintegration/src/FSharp.LanguageService/**`, `vsintegration/src/FSharp.ProjectSystem.FSharp/**`, `vsintegration/src/FSharp.ProjectSystem.Base/**`, `vsintegration/src/FSharp.VS.FSI/**`, `vsintegration/src/FSharp.UIResources/**`

### MEF composition and package load

The editor does not compose, another language loads F# visuals, or the host runs resources that were never embedded in the package.

**CHECK:**
- Declare MEF exports in an assembly the component model already composes. Do not import a part into a package that is not itself composed, and do not export a host settings service that is not a MEF part.
- A MEF view factory must not be visible to other languages. Keep custom visuals on an internal type so another language's tooltip does not load the F# editor.
- One-time color or theme setup belongs on the language-service composition root, not on a chain of parts that may never be instantiated. Set workspace options once when the language service initializes, not on every new text view.
- Editor options must be a composed part that features can import. Expose as little of the editor to MEF as possible: a small factory should choose the host or test store instead of exporting the whole options implementation, and do not keep a static singleton the language service initializes by hand.
- Assign a process-global resource assembly once, from a composition singleton. Assigning it again for each view throws. The options page must open with no solution loaded.
- Do not dynamically load XAML resources, and do not enable a preserialized resource form the component merger cannot read as a blanket project property. A resource change must be embedded in the extension package, and the steps must work from a clean tree; one IDE build is not enough.
- Removing an unused host interface must leave the editor able to start. Shared sources used by more than one product stay with the compiler; a project file may retarget a link. Reuse the existing editor helper for the same lookup instead of a second copy.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/*.{fs,fsproj,resx}`, `vsintegration/src/FSharp.Editor/Common/**`, `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.Editor/Options/**`, `vsintegration/src/FSharp.Editor/xlf/**`, `vsintegration/src/FSharp.UIResources/**`, `vsintegration/src/FSharp.ProjectSystem.FSharp/**`, `vsintegration/Vsix/**`

### Solution and project lifetime

Project setup runs during solution load, a close leaves the previous solution cached, or a script or path is not the project the user loaded.

**CHECK:**
- Defer project setup until the host signals that the solution UI is ready. Do not replace that signal with a custom wait or a fixed delay.
- Do not set up projects while the solution is loading, on the first open or on a later open of the same solution, and do not run a document analyzer on a temporary project the host creates before the solution has loaded.
- Register projects as the solution reports them, after solution open, instead of waiting for a document to open.
- On solution close, run the same cleanup as document close and release documents that are still open, because the workspace does not raise a close for them. Fix a project-close race by ordering disconnect against in-flight work, not by delaying the call.
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
- Quick info and inline hints must show the compiler's existing tooltip text, must not show a parameter value that belongs to signature help, and must not ship a debug tooltip that shows an internal layout tag.
- A section separator must have a fixed length. Do not size it by repeating glyphs or draw it as ASCII art, and do not treat a hand-drawn divider as wrapping. Break long documentation explicitly.
- Quick-info maximum width must scale with the editor font. Documentation may match the signature width only up to a fixed cap, alignment must follow the document reading direction, and documentation must not be fully justified.
- A quick-info section that other languages expose as a setting must be configurable, not always shown. A secondary tooltip must follow the active theme, and a path it displays must be relative to the solution.
- Custom tooltip content must reuse the host quick-info controller so show and dismiss stay identical to the platform. Do not keep a session capture whose only job was to dismiss the tooltip on navigation once the host already does that.
- Quick-info content must stay a view model another host can translate. Do not build that model out of a Windows-only element. Record and value text shown in tools should keep a single space between braces and fields.
- Do not add a width control that fights the host's own tooltip wrapping. If a setting does not always apply, mark it experimental and explain the effect. Quick info must keep line breaks between sections, and long documentation must still wrap after a host API change.

**Severity:** medium.

**Hotspots:** `vsintegration/src/FSharp.Editor/QuickInfo/**`, `vsintegration/src/FSharp.Editor/Hints/**`, `vsintegration/src/FSharp.Editor/Options/**`, `vsintegration/src/FSharp.UIResources/**`

### Navigate to the loaded definition

Navigation opens a window itself, jumps to metadata or the wrong linked copy, or offers a link that goes nowhere.

**CHECK:**
- A definition request must return a navigation item and must not open a window itself. Hover must not jump to decompiled metadata because the pointer is over an external symbol.
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
- Completion must not invent a method body. Override completion must accept the member name first and the body in a later step, and must not insert the whole override, including a generated body, on one accept.
- Return every edit of a completion commit as the completion change for the document version the host will apply. A side edit is applied to a version the host then overwrites.
- A name suggestion should reuse the completion items for that context when that lookup is cheap. Names the editor shows must stay separate from the representation that guides compiled form.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Completion/**`, `vsintegration/src/FSharp.Editor/AutomaticCompletion/**`, `vsintegration/src/FSharp.Editor/CodeFixes/**`

### Diagnostics scope and language gates

A code fix matches a culture-specific message, a host analysis limit is never applied, or a language relaxation hits every project.

**CHECK:**
- A code fix must not identify a diagnostic by its displayed message. The diagnostic must carry structured data the fix can match in any culture.
- When a diagnostic payload already has more than one case, callers should read those fields directly rather than adding a separate type test.
- A diagnostics exporter must not stay enabled on the analysis path. Left running, it adds overhead that shows up as a compile-time regression.
- Editor analyzers must follow the host's live-analysis scope setting. Do not invent a private active-document gate beside that setting.
- A host limit or setting must be applied on the override or callback the host actually invokes. A limit that lives only in an initialization method the host no longer calls is never applied.
- A language-rule relaxation that editor tools depend on must be gated by language version and exposed in a form tools can read. An editor suggestion must not rewrite code into a form an older compiler warns about. A language feature that changes editing must be tryable behind a preview switch before it becomes the default.
- If a code-fix test fails because the diagnostic no longer reports that case, fix the diagnostic. A missed shape is not a reason to drop the test.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Diagnostics/**`, `vsintegration/src/FSharp.Editor/CodeFixes/**`, `vsintegration/src/FSharp.Editor/Refactor/**`, `vsintegration/src/FSharp.Editor/Options/**`

### Store each option on its own

Editor settings are persisted as one blob, so a new setting comes back as the zero value, or a setting cannot be found.

**CHECK:**
- Declare, store, and retrieve each editor setting on its own so a newly added setting keeps its default. Do not persist the whole settings type as one reflected blob.
- An options type the settings page binds must keep the marker that makes its properties settable, and options-page markup must bind to public properties.
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
- Classify a name by its language role. A function must not be colored as a keyword, and a method and a function value must not share a classification when their calling rules differ.
- Punctuation that is still part of a string must keep the string classification. Classifying that range as a delimiter hides the string extent and needs a test for that range.
- Do not assign a fixed foreground on a classification format. Leave it unset so the host theme supplies a readable color and the user can still customize it.
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
- Do not use a process-global resource assembly to locate one extension's dictionaries. Another extension assigning that same property causes a runtime failure. Resolve resources from this extension's install location.

**Severity:** critical.

**Hotspots:** `vsintegration/src/FSharp.Editor/Diagnostics/**`, `vsintegration/src/FSharp.Editor/QuickInfo/**`, `vsintegration/src/FSharp.Editor/Options/**`, `vsintegration/src/FSharp.UIResources/**`, `vsintegration/src/FSharp.Editor/xlf/**`, `vsintegration/src/FSharp.Editor/Common/**`

### Editor tests cover the host

An editor change is tested only through the service API, or only for a flat module-level shape, so the host failure stays invisible.

**CHECK:**
- An editor test must exercise the editor provider. A suite that only calls the service method does not cover the editor.
- Cover the declaration shapes the feature claims, including a binding nested in a type or in an expression, a dotted name in a nested pattern, and a union that is out of scope or requires qualified access, not only a flat module-level case.
- A known limitation of an editor feature may be recorded as a skipped test so the missing case stays visible. Do not leave that case with no test at all.
- Editor tests must reuse one MEF composition instead of building a new export provider for every test. Tests must not share a process-wide options part across cases. If desktop tests share one host, a throw from a background thread must still name the case.
- Put a test that needs host-internal types in the test project those internals are already visible to. Do not link a setup file into an editor test project that does not reference the utility project that makes that setup run.

**Severity:** medium.

**Hotspots:** `vsintegration/tests/FSharp.Editor.Tests/**`, `vsintegration/tests/UnitTests/**`, `vsintegration/tests/Salsa/**`

### Interactive work stays cancellable

A keystroke, search, or tooltip starts work that keeps running after the user has cancelled or moved on.

**CHECK:**
- Live-buffer edits must not start a non-cancellable parse for every keystroke. That work must be a debounced request for the current document version.
- User-started editor search must pass cancellation into the parallel scan. A parallel loop that ignores the request token keeps running after the search is abandoned.
- Solution search must stay cancellable, include operators as well as identifiers, and keep its cache for the solution lifetime, cleared on close, rather than a short-lived cache rebuilt on every query.
- Tooltip navigation must use the host's built-in link and must be cancellable. If the user cancels the slower implementation lookup, fall back to the instant range. Render each tooltip element as icon, then description, then documentation, because the host link parser reads that order.
- Do not add a second cap on editor background parallelism when that work is already bounded. A second cap drifts from the bound callers already use, so cancellation and degree disagree.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Common/**`, `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.Editor/Navigation/**`, `vsintegration/src/FSharp.Editor/QuickInfo/**`, `vsintegration/src/FSharp.Editor/Diagnostics/**`


