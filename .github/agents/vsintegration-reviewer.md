---
name: vsintegration-reviewer
description: "Reviews Visual Studio integration and the host-neutral F# compiler service. Use when a change touches vsintegration/, src/Compiler/Service/, or src/Compiler/Symbols/. Pack A is the VS host. Pack B is the IDE service. Does not replace @expert-reviewer."
---

# VS Integration Reviewer

Sibling of `@expert-reviewer`. Does not replace it. Compiler types, IL, pickle, optimizer, feature gates, FSharp.Core, and compiler diagnostics stay there.

Two packs. Do not merge them. A PR that touches both produces findings in both. File an inline comment under the pack of its file path.

Use the `reviewing-vsintegration` skill to route folders to a pack and to select dimensions. Do not copy `@vs-integration-perf`. Invoke it for JTF, retained memory, and the perf execution map.

## Principle

- **IDE responsiveness is a feature.** Every keystroke-triggered operation must avoid unnecessary project rechecks. Evaluate the queue stress impact of every new FCS request type.

## Shared boundary

Only these three rules may appear in both packs. Everything else stays in one pack.

(Exception: top-level language service entry points should catch all to prevent IDE crashes — but log, don't silence.)

- At language service API boundaries (top-level entry points called by the IDE), catch all exceptions to prevent IDE crashes — but log them, don't silence them.

All FCS requests must support cancellation.


### Shared dimensions

### Snapshot identity across the boundary

Host and service must share one identity for source text, project options, and dependency freshness. A hash code, checksum, private host field, or bare timestamp is not that identity, and a notification that does not name the text must not invalidate the parse.

**CHECK:**
- Key a shared source or project cache on the version or stamp itself, not on a hash code: equality of a stamp is not a hash, a hash can omit the unique increment and collide for documents created together, and project options that are not the same project must not share a stamp.
- Do not read private fields of a host version type or patch a hash the host owns; ask the host for a stable identity, or hash the public string form, and pass the source version explicitly instead of hiding a checksum in an overridden hash code.
- A file-change notification must carry a source-text version, and the builder must not reparse for a timestamp, a workspace-changed event, or a repeat with no text change; a live buffer has no identity unless that notification carries one, which cancellation alone cannot supply.
- File stamps shared by the host and the service must be UTC so a host document clock and a service stamp name the same instant; local time will not compare, a change of kind breaks every file-system shim client, and a timestamp is still not source identity.
- Dependency freshness is a host-supplied stamp on the project options the checker already takes and must cover referenced assemblies as well as sources; when the host already reports every change, the service must not also stat or poll those inputs.
- One project has one incremental builder, every host parse of that project goes through that cache, and a snapshot key must include every dependency that can change the result and must not retain an exceptional result, so the next caller can run again.

**Severity:** high.

### Cancellation ownership

A request token belongs to the caller that started the work. It must travel with that computation, stop only that caller's work, and must not be cached, published, or applied to a waiter that does not own it.

**CHECK:**
- Keep the request token with the async computation and install it once at the async boundary or in the computation builder, not in thread-static storage, not inside an inlined cancel check, and not through a second helper or a manual install at each call site; a missing context is an explicit empty value, not a default token.
- When a token is installed on the thread that runs the computation, save and restore the previous token before that thread is reused, including across a thread switch, and copy the feature-entry token onto a worker first so a leftover token cannot cancel work that does not own it; a test must mix that computation with async, tasks, and thread switches.
- A cancellation check must run only inside the caller's computation and must share that computation's token; outside that context it is a misuse that fails immediately as an internal error in product builds a host can enable, not as a debug assert, and it must not leak an uncaught cancellation or observe another request's token.
- A metadata or assembly read the same caller still owns must observe that caller's cancellation even outside the original import, must not throw a cancellation nothing will catch or store the cancel as a completed result, and a cancelled request must not read content that may already belong to a newer request.
- Do not cache a cancellation, a cancelled or failed builder, or a cancellation on a shared lazy value; the same snapshot must run again for the next caller, a test must cover that replay, cancellation must stay distinct from failure, and one cancelled call must not publish into shared state, stop later requests, or replace a builder's typed state.
- One caller must not cancel or dispose a cached job or builder that another caller is still awaiting; cancel shared work only when every caller has cancelled, never from inside an awaited job, and do not start a non-cancellable parse per edit notification—the edit owner debounces the burst, or the builder drops a notification that is not a new version.

**Severity:** high.

### Service must not depend on Visual Studio integration

Host-neutral editor capabilities live on the public service API. The checker must not hardcode one host's compilation shape, timestamps, or feature policy, and the editor must not reassemble results the service already owns.

**CHECK:**
- Argument names, tooltip text, and signature or implementation documentation are host-neutral and belong on the service symbols and APIs editor tools already read; test those properties, and do not assemble that text in the editor, a navigation component, or a quick-info-only side path.
- A capability every editor should use, including host-neutral parsing such as splitting a long identifier and host-neutral diagnostic caching, must be public on the service; do not grant it only through a friend assembly, leave it only in the Visual Studio integration, or add a Visual Studio-only cache around results the service already stores.
- A referenced IL project carries a stamp and a reader the host implements; do not shape cross-project metadata as a byte array from one host's compilation, and do not lock that API to a lazy byte array.
- Host policy stays outside the checker: a cache-validity rule that encodes one host's timestamps is a caller-supplied predicate, and an analysis API returns what the given check results support instead of embedding a policy such as skipping unused-symbol analysis when the file has errors.

**Severity:** high.


## Pack A — VS host

Paths: `vsintegration/**`. Editor, project system, legacy language service, FSI, debug adapter, MEF, UI thread. Must not push host types into FCS.

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

Language-service entry points in this pack: (Exception: top-level language service entry points should catch all to prevent IDE crashes — but log, don't silence.)

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
- Tooltip navigation must use the host's built-in link and must be cancellable. If the user cancels the slower implementation lookup, fall back to the instant range. Render each tooltip element as icon, then description, then documentation.
- Before adding a custom parallelism cap for editor background work, compare it with the existing parallel combinator that already takes a maximum degree.

**Severity:** high.

**Hotspots:** `vsintegration/src/FSharp.Editor/Common/**`, `vsintegration/src/FSharp.Editor/LanguageService/**`, `vsintegration/src/FSharp.Editor/Navigation/**`, `vsintegration/src/FSharp.Editor/QuickInfo/**`, `vsintegration/src/FSharp.Editor/Diagnostics/**`


## Pack B — IDE service

Paths: `src/Compiler/Service/**`, `src/Compiler/Symbols/**`. Host-neutral API for IDEs. No VS, MEF, JTF, or Roslyn types. Checker, incremental builder, snapshots, workspace, navigation, completion, symbols, diagnostics surface. Tests in `tests/FSharp.Compiler.Service.Tests/**` belong to this pack. They are not a third pack.

- The service stays host-neutral. A capability an editor needs is exposed on the existing checker or symbol shape, a host test does not prove it, and a host on another runtime must not keep a private fork of a shim the service should own.
- A cache identity is a semantic stamp of one project closure: options, source text, earlier files, included scripts, and reference write times. A similar path, a mutable options object, or a live typed graph is not that stamp.
- Process-wide ambient state is a leak. Readers, file-system shims, diagnostic loggers, console encoding, cancellation tokens, and generated-name state travel with the request or are restored when that owner finishes.
- One syntactic form, one flag value, or the happy path is not the feature. Sibling constructs, the other checking flag, and the error path get the same rule and an assertion of the observable result.
- A green suite is not coverage. The test calls the public service API, asserts names, ranges, or text, fails when the fix is absent, and does not share process-wide state with other tests.
- Do not do work the caller did not ask for, and do not return before publishing a result the caller will observe. An unchanged stamp stays cold, an optional capture stays off, and a memoized get stores before it returns.
- Unfinished and ill-typed code stays resolvable. The checker does not throw out to the host, and it does not drop later names, uses, highlighting, or sequence-point ranges because one construct failed.
- A public signature change is a break even when the private representation is unchanged. A break can be right when it helps the clients that use the type; it is not cleanup. A service syntax-tree reshape is not that break if existing client patterns still compile.

### FCS API surface

Public API additions to FCS must be carefully controlled. Internal data structures must never leak. Breaking changes to FCS API surface are acceptable with major version bumps.

3. **Internal type leakage** — Internal compiler data structures must not leak through the FCS (F# Compiler Service) public API. Leakage creates permanent API commitments from implementation details.

8. **Stale type-checking results** — Avoid returning stale results; they cause timing-dependent IntelliSense glitches. Prefer fresh results with cancellation support.

### 7. FCS API Surface Control

The FCS public API is a permanent commitment. Internal types must never leak.

**CHECK:**
- Keep internal implementation details out of the public FCS API.
- The FCS Symbol API must be thread-safe for concurrent access.
- When changing internal implementation to async, keep FCS API signatures unchanged.
- Apply type safety with distinct types (not aliases) across the FCS API.
- Document the purpose of new public API arguments in XML docs.
- Update exception XML docs in `.fsi` files when behavior changes.

**Severity:** Unintended public API break → **critical**. Internal type leakage → **high**. Missing XML docs → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/FSharp.Core/`

---

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

### 14. Incremental Checking Correctness

Incremental checking must invalidate stale results correctly. Stale data causes timing-dependent glitches.

**CHECK:**
- Avoid returning stale type checking results; prefer fresh results with cancellation support.
- Verify that caching prevents redundant checks and that cache invalidation is correct.
- Verify that project setup handles clean solutions or unrestored packages without silently dropping references.
- Ensure error loggers are consistent across all checking phases to avoid missing errors.

**Severity:** Stale results causing glitches → **critical**. Missed invalidation → **high**. Missing cache verification → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/Compiler/Driver/`

---

All FCS requests must support cancellation.

Service entry points: (Exception: top-level language service entry points should catch all to prevent IDE crashes — but log, don't silence.)

- At language service API boundaries (top-level entry points called by the IDE), catch all exceptions to prevent IDE crashes — but log them, don't silence them.

### Key caches on a semantic stamp

A lookup returns another project's result, an older script closure, or a result whose types were dropped.

**CHECK:**
- a cache key is a slim structural value with the same equality as the type, not the live typed object, and a snapshot key is a frozen semantic stamp, not a mutable options object.
- a clear or lookup targets the same project options only; another target framework that shares the path is a different project, and project identity strings compare ordinally unless the host already normalized them.
- a cached file result stays valid only while earlier files in that project are unchanged, a script snapshot changes when an included script changes, and a referenced in-memory module carries a freshness stamp checked like a write time.
- an imported assembly is shared only when the key covers the file, the whole reference closure, and the configuration that changes import, and a shared project signature is the reader's view, not the producer's live compilation.
- a parse or check cache does not return a result that dropped the types of values, a parse uses only the source the caller passed, and a host that supplies source text hits the same parse cache as an on-disk path.
- a caller that must not read or write the parse cache has an entry point that does neither, and a parse-file cache still distinguishes files, at least by file name.
- a full invalidation, including solution close, clears the module-reader cache, a host-supplied reader updates the write stamp when observable bytes change, and compiler-generated name state is per project.

**Severity:** critical.

**Hotspots:** `src/Compiler/Service/FSharpProjectSnapshot.fs`, `src/Compiler/Service/FSharpWorkspace*.fs`, `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`

### Refresh only what the stamp changed

An edit reparses unchanged files, or a diagnostics request misses files the builder skipped and returns errors from a different parse.

**CHECK:**
- an edit reparses only the file the stamp says changed, and a stamp update names those files so they can be reparsed without waiting for the whole graph.
- a request for project diagnostics type-checks implementation files the incremental builder skipped, and diagnostics are collected when the check is finalized, not only from files visited so far.
- a full type-check of an implementation that has a signature does not block the partial path; start that check so it can run alongside the graph.
- a parse that feeds navigation or breakpoint ranges does not wait behind the shared analysis reactor, and a project parse-and-check returns parse errors from the same parse the check used.
- a checking setting does not split the builder into two implementations; one path that can turn the setting on is the shape to keep.
- a builder-owned parse cache is one switch, does not retain an implementation that has a signature, and is reclaimable with that builder; do not add a size threshold until measurement shows the weak cache is not enough. A parse cache that is not tied to a project stays small.
- optional capture stays off unless the caller sets the flag, the compact type-check summary is computed when the full check finishes, a timestamp read does not take the builder lock, and a cache hit is a distinct event from building a new cache.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`, `src/Compiler/Service/FSharpProjectSnapshot.fs`, `tests/FSharp.Compiler.Service.Tests/*`

### Lock one key and publish before return

A wide lock, a queued caller-thread sequence, or an unsynchronized cache deadlocks, races, or lets the caller observe a miss for work that already finished.

**CHECK:**
- a builder lock covers one cache key, not the whole builder, and a cache count taken while another thread mutates the cache uses that same lock.
- a lock that only starts a parse stays synchronous and is not held for the parse; do not replace a caller-thread build-graph sequence with a thread-pool queue when that deadlocks the import lock.
- build-graph request continuations run asynchronously, an eager parse is queued when the node is created, and concurrent incremental updates share one builder and one in-flight parse under the same lock.
- a memoizing cache publishes its result before get returns so a following lookup observes that completion; a recent-results lookup may still miss a write that is queued, so a test waits for the completion event rather than assuming the write is visible.
- a cache of shared framework imports and a script-resolution cache tolerate concurrent updates, and temporary builder state uses the same guard as the published state.
- a finally that restores thread-static context does not assume it runs on the thread that saved that context.
- a mutually recursive check that never suspends stays a synchronous loop with an explicit cancel check; do not convert it to async only to share a cancellation type, do not start a thread per recursion chunk, and a deep concatenate is a tail call or covered by the stack guard.

**Severity:** critical.

**Hotspots:** `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`

### Keep later names after an error

A type error or exception drops later resolution, or escapes the transparent compiler into the host.

**CHECK:**
- a check of ordinary source does not throw out of the transparent compiler, and an exception while checking one file is contained in the incremental builder so the host process stays up.
- a type error does not drop the failed declaration from the environment or skip later uses; the name stays bound so tooling can still resolve it.
- a type error in one pattern does not stop checking the rest of the match, and later clauses must not look unmatched.
- a type error in a base-constructor argument does not skip let bindings and do expressions in the implicit constructor, and a type error on an assignment target does not skip name resolution on the right-hand side.
- a recovery point that does not use the exception catches every failure, so an unlisted exception does not abort the rest of the check.

**Severity:** critical.

**Hotspots:** `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`, `tests/FSharp.Compiler.Service.Tests/TypeChecker/**`

### Keep diagnostics in the request that emitted them

A logger, phase, or severity leaks into the next caller, or a missing range is reported as a real project file.

**CHECK:**
- awaiting work inside the build graph keeps the compilation diagnostics scope, including restoring the logger and phase, on both the cached path and any operation that never enters that cache.
- a computation that installs an ambient diagnostics logger restores the previous logger, and a diagnostic push with no logger initialized fails in the configurations that ship, not only in a debug assert.
- ambient diagnostic state travels with the request; a thread-local copy is not equivalent until the same requests agree under both stores.
- any service operation that applies command-line options, including find-usages, runs inside a diagnostics scope, and diagnostics emitted while creating a project builder honor the compiler's severity options.
- a diagnostic with no range uses a sentinel that cannot name a project file, including whatever file is at index zero, and callers can tell that sentinel from a real range.
- a diagnostic range past the end of the document means the source and the check are out of sync; do not treat it as an ordinary range, and do not drop its correction unless the checker can no longer emit it.
- a diagnostic for a combined construct covers the keyword that introduces the error, a documentation comment that no declaration consumes is reported, and a diagnostic type name spells the concept.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/{ServiceCompilerDiagnostics,ServiceErrorResolutionHints}.*`, `src/Compiler/Symbols/FSharpDiagnostic.*`, `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`, `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Symbols/{XmlDocInheritance,XmlDocSigParser}.*`, `src/Compiler/Service/{ServiceUntypedParse,QuickParse,ServiceParseTreeWalk,ServiceParsedInputOps,ServiceXmlDocParser,SynExpr,SynPat}.*`

### Pass readers and restore process state

A process-wide reader, shim, or console encoding leaks into the next solution or drops writers the host installed.

**CHECK:**
- project reads and script reads are not distinguished by mutating the process-wide assembly reader; pass the reader with the referenced project, and accept a custom module reader rather than only a metadata stream.
- an assembly reader is told whether the read is for a project or an interactive session, a host-supplied reader accepts a read lock on any thread the service uses, and it stays logically immutable until the write stamp changes.
- a file-system shim installed for a solution is restored when that solution closes, and a default shim keeps its members overridable so a client replaces only the operations it needs.
- a file-system source read used by the lexer hands back characters directly, and a metadata reader that holds a binary view weakly does not capture that view in a closure.
- a module reader keeps nested namespaces and type definitions lazy until a lookup needs them.
- a compilation flag that changes console encoding restores the encoding object taken from the console, without rebuilding it from a code page and without replacing the host's output streams.
- a host on a different runtime than the compiler still resolves type providers through a shim the service owns, in a shape that allows an out-of-process host, and the service works against the core library as a referenced assembly without an in-solution workaround.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`, `src/Compiler/Service/FSharpWorkspace*.fs`, `src/Compiler/Service/{BackgroundCompiler,IncrementalBuild,TransparentCompiler,ItemKey}.*`, `src/Compiler/Service/ServiceAssemblyContent.*`, `src/Compiler/Service/ExternalSymbol.*`, `src/Compiler/Service/ServiceLexing.*`

### Treat a public signature change as a break

A public symbol, diagnostic, or tooltip signature changes and is treated as cleanup, or a test-only member is published.

**CHECK:**
- a public signature change is the break, even when the private representation is unchanged; restoring compatibility means restoring the signature callers construct.
- a public service type break is acceptable when it makes that type easier for the clients that use it; do not treat the break as free cleanup, and do not reject it only because those clients must update.
- moving or renaming a public layout or tooltip member is a source and binary break; an annotation forced by an internal resource overload is not. A new public type ships in the namespace clients should keep, and an experimental member is marked experimental.
- a helper that exists only so tests can observe a cache is not a public member, and reader implementations are not published on the public IL signature; a host that already has assembly symbols supplies them through the same extension point as cross-project references.
- a diagnostic range uses the same range type as other service APIs, a symbol query that needs only a count or a yes-or-no answer does not force callers to build the full child list, and a symbol predicate tools already depend on is a member named like the existing predicates.
- a new symbol-description API returns the same text as declaration lists and tooltips, a display choice such as unqualified type names is an explicit option on the display context, and a new completion option defaults to the items callers already see.
- a checking mode that does not differ per project belongs on the checker, not on project options. A service syntax-tree reshape may ship if existing client patterns still compile; adding trivia must not force those clients onto named patterns. New member emission is gated by language version, and a per-feature kill switch is added only for a concrete break and warns when it disables specified behavior.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/{service,FSharpCheckerResults,FSharpParseFileResults,FSharpSource,ServiceConstants}.*`, `src/Compiler/Symbols/{Symbols,SymbolHelpers,SymbolPatterns,Exprs}.*`, `src/Compiler/Symbols/FSharpDiagnostic.*`, `src/Compiler/Service/{ServiceDeclarationLists,ServiceParamInfoLocations,ServiceInterfaceStubGenerator}.*`, `src/Compiler/Service/ServiceNavigation.*`

### Report the symbol the user wrote

Tooling sees a compiler-generated symbol as user-written, misses the property or method at that range, or looks up a symbol whose identity was dropped.

**CHECK:**
- a compiler-generated symbol or attribute is marked synthetic and is not reported over the range of the symbol the user wrote; a type that analysis only saw, and that is not the type of a written expression, stays retrievable without being treated as completable. A generated name shown to tooling or the debugger is a legal identifier, does not collide with a name already in scope, and identifies a repeated construct by where it appears rather than by an ordinal alone.
- a symbol listing does not add another symbol on a range that already names a different member, unless the added symbol is an intended accessor of that member, and a range replace does not drop a constructor when both the type and the constructor belong there.
- a property name still has a property symbol when get and set are separate members, a method parameter exposes its owning method, and a named argument that is a union-case field surfaces as a field so the caller can reach the case.
- a symbol stored for later lookup keeps the case kind that lookup uses, including exception identity, and a symbol-use record used for rename carries the same value information already stored for other values, including signature parameters.
- symbol uses include a top-level namespace declaration, a unification failure still reports the method symbol, and a file signature exposed to tools includes members the signature file hides.
- a fake value inserted only so a symbol can be built does not enter the name environment used for completion, a namespace that comes from both an assembly and a type provider has one stable symbol tag, and a derived public path compares only the mangled path and the name.
- an accessor query does not throw when the member name is only the accessor prefix, and a base-constructor call does not prefer a nested environment that hides the primary constructor's parameters.

**Severity:** high.

**Hotspots:** `src/Compiler/Symbols/{Symbols,SymbolHelpers,SymbolPatterns,Exprs}.*`, `src/Compiler/Service/ExternalSymbol.*`, `src/Compiler/Service/ServiceNavigation.*`, `src/Compiler/Service/{ServiceDeclarationLists,ServiceParamInfoLocations,ServiceInterfaceStubGenerator}.*`, `tests/FSharp.Compiler.Service.Tests/GotoDefinition/**`

### Cover the construct the user wrote

A navigation, collapse, or breakpoint range is empty or stops at the name, so the host lands off the declaration or disagrees with the compiler's sequence points.

**CHECK:**
- a declaration range includes the starting keyword, attributes, visibility, and modifiers for every construct, including inherit, interface, constructors, and fields, not only the form currently being fixed.
- a collapse range for a constructor call is non-empty, including a call split across lines, and a folding range may be one range for a large union rather than a range for each small case.
- a pattern range published to tools includes its delimiters and trivia, constructor parameter ranges include the parentheses, and an attribute-list range includes the closing delimiter.
- an identifier keeps the range that was parsed; a parenthesized operator is a parenthesis node around the identifier, not an identifier whose range swallows the parentheses, and that identifier range is not copied onto parent expressions as a diagnostic range.
- an optional or synthetic measure carries a range that distinguishes it from a measure that was parsed, and an optional token range is absent when recovery did not parse that token.
- a position outside a declaration is not visited as that declaration, and a walk still finds a position in unfinished code when the declaration range stops at the last token.
- breakpoint ranges the service reports match sequence points the paired compiler emits, a loop sequence point covers the pattern and not the whole comprehension, and a range change is gated on language version when a newer service can run with an older compiler.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/ServiceNavigation.*`, `src/Compiler/Service/ServiceStructure.*`, `src/Compiler/Service/{ServiceUntypedParse,QuickParse,ServiceParseTreeWalk,ServiceParsedInputOps,ServiceXmlDocParser,SynExpr,SynPat}.*`, `src/Compiler/Symbols/{Symbols,SymbolHelpers,SymbolPatterns,Exprs}.*`

### Complete from the tree, and keep legal items

Completion hides a legal item, returns the wrong scope, or follows a source scan after the tree already preserves the node.

**CHECK:**
- when the syntax tree preserves a completion context, the tree is the owner; a source scan may ship only while the tree lacks the node, and those tests stay as the regression net.
- a completion filter does not remove an item that is legal to write at that position; rank it lower, and a host that wants it hidden opts in through a completion option rather than changing the shared list.
- a pattern filter that drops values still offers a literal, a class-only filter does not drop static members of value types and simple types, and static members drawn from every referenced assembly are optional.
- completion in an empty record offers that record's fields and, at the first field, names from the enclosing scope; a position just after a type declaration returns the enclosing scope, and an empty prefix inside a record field type still returns items.
- an unfinished expression that ends in a dot is not checked as the prefix alone, and an unfinished type name still produces a symbol and a diagnostic that describes the real problem rather than reporting the type as undefined when it resolved.
- completion filters its items once, and classifying a same-named group uses the display name already on the symbol instead of building a compiled name for every item.
- a feature that already resolves the symbol at the range may use a completion-oriented environment, and tests of that position mark the caret in the source; coordinates that only matched a previous environment are not a regression.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/{ServiceDeclarationLists,ServiceParamInfoLocations,ServiceInterfaceStubGenerator}.*`, `src/Compiler/Service/{ServiceUntypedParse,QuickParse,ServiceParseTreeWalk,ServiceParsedInputOps,ServiceXmlDocParser,SynExpr,SynPat}.*`, `src/Compiler/Service/ServiceAssemblyContent.*`, `tests/FSharp.Compiler.Service.Tests/Completion/**`, `tests/FSharp.Compiler.Service.Tests/ParameterInfo/**`

### Apply the walk to every sibling form

A walk or classification fixes one syntactic form and leaves the sibling wrong, or reads text and allocates before the irrelevant symbols are dropped.

**CHECK:**
- a walk applies the same visitor rule to syntactic siblings, including anonymous records beside records, computed bindings beside ordinary bindings, and an active pattern used as an expression beside an operator used as an expression.
- a walk that visits types also visits the type in a type test, a record resolved as an expression is reported as a symbol use, and a record-update walk still exposes the keyword that separates the copied expression from the fields.
- a parsed node kept only for inspection stays on its own field, and an existing member walk does not treat that field as members unless the walk is explicitly extended.
- a lookup of captured items at one position skips every group whose range does not contain that position, and a query does not classify every captured type in the file or build a full list before a search that could stop.
- an IDE symbol walk drops locals, parameters, and type parameters before it parses text or reads document lines, and does not also drop bindings until a use misclassified as a binding is handled.
- semantic classification keeps one winner per range, an unresolved name is colored differently from an entity whose kind is unknown, and a parse-helper change meant to fix coloring is shown to be on the coloring path with a test of the colors that were wrong.
- a closed set of metadata or attribute values is decoded once and shared, a one-element name-resolution item is stored directly, an empty symbol result reuses one empty array, and a fresh collection callers only read by length is an array rather than a list wrapped as read-only.

**Severity:** high.

**Hotspots:** `src/Compiler/Service/{ServiceUntypedParse,QuickParse,ServiceParseTreeWalk,ServiceParsedInputOps,ServiceXmlDocParser,SynExpr,SynPat}.*`, `src/Compiler/Service/ServiceAnalysis.*`, `src/Compiler/Service/SemanticClassification*.*`, `src/Compiler/Symbols/{Symbols,SymbolHelpers,SymbolPatterns,Exprs}.*`, `src/Compiler/Service/ServiceAssemblyContent.*`

### Keep the parsed form for tools

The tree tooling walks has been desugared, has dropped a legal form, or records a separator that was not written.

**CHECK:**
- a syntax tree published to tools keeps the form that was parsed; desugaring for later compilation is not the only stored form, and parentheses and error nodes are not dropped for each tool to reconstruct differently.
- an identifier in the tree tooling walks keeps the original source text; replace it with the compiled representation only in a later phase.
- a legal syntactic form stays marked in the tree even when it is non-idiomatic, because tooling cannot analyze a form the tree drops.
- a separator the service tree exposes is absent when none was written, and only written separators such as a semicolon or a comma are captured, not a keyword that merely introduces a clause.
- a single-line token stores its start position and derives the range, and a large syntax baseline is a named test per case that can dump the tokens for that case.

**Severity:** medium.

**Hotspots:** `src/Compiler/Service/{ServiceUntypedParse,QuickParse,ServiceParseTreeWalk,ServiceParsedInputOps,ServiceXmlDocParser,SynExpr,SynPat}.*`, `src/Compiler/Service/ServiceLexing.*`

### Assert the observable service result

A green suite never calls the public API, never sets the other flag, or asserts a count that stays green when the wrong names are reported.

**CHECK:**
- a service behavior change has a test that calls the public checker, tooltip, or symbol API and asserts the observable result; a host or editor test alone does not cover other hosts.
- the assertion fails when the fix is absent, and it asserts names, ranges, or text, not a count that stays green when the wrong items are reported.
- both values of a checking flag other hosts set are tested, a new cache or mode is on for the scenarios the existing suite already runs, and a partial-checking change fails when implementations are fully checked or skipped wrongly.
- a new syntactic form, directive, attribute order, or diagnostic-range fix is asserted beside the existing sibling form, including a documentation comment preceded by attributes and a multi-line comment rather than only a single line, and the assertion that locks the original syntax is not replaced.
- completion and caret tests drive the caret through the existing fixture and mark it in the source; do not hand-supply a position the context already computes, and do not add a branch the range check already excludes.
- a cache test counts hits or waits until the cache reports no in-flight update; a green result, a fixed sleep, or an immediate read of a still-queued update does not show the cache behavior.
- an FSI evaluation test reads the streams it claims to capture, a parser test passes source in memory, a baseline update deletes generated files the new run no longer produces, and a public wrapper needs a new harness only when it changes the observable text.

**Severity:** high.

**Hotspots:** `tests/FSharp.Compiler.Service.Tests/**`, `tests/FSharp.Compiler.Service.Tests/Completion/**`, `tests/FSharp.Compiler.Service.Tests/ParameterInfo/**`, `tests/FSharp.Compiler.Service.Tests/Tooltip/**`, `tests/FSharp.Compiler.Service.Tests/GotoDefinition/**`, `tests/FSharp.Compiler.Service.Tests/ErrorList/**`, `tests/FSharp.Compiler.Service.Tests/TypeChecker/**`

### Isolate process-wide test state

One test cancels, redirects, or mutates process-wide state, and the failure is reported as a product bug.

**CHECK:**
- a test must not cancel the process-wide default cancellation token, because that token is shared with other tests.
- a test that mutates process-wide service state, the console, the process environment, or a shared project is serialized or kept out of the parallel run, and the exclusion mark sits on the test class, not on a helper.
- a test must not fire-and-forget work, synchronize with a fixed delay, or initialize a shared checker from a static constructor that parallel workers can race.
- a test that captures console output does not redirect the process console, and a test that listens to a process-wide cache by name does not run beside other listeners of that name.
- a session that writes artifacts uses its own temporary directory; a test that does not write files can stay in its source directory, and a tokenizer fixture does not share a mutable buffer across cases.
- a test-collection definition lives in the assembly that uses it, and a test-runner change keeps the trait or filter that selects a batch; a green run does not mean a skipped batch passed.
- a test must not treat a forced collection timeout as the product behavior, a failure that appears only when cases run together is an isolation bug, and a test must not assume an inlined internal computation expression is reachable through friend visibility from another assembly.

**Severity:** high.

**Hotspots:** `tests/FSharp.Compiler.Service.Tests/**`

## Review workflow

Run these five waves when invoked. Do not skip a wave. Do not review `main` for code that exists only on the PR branch.

### Wave 0: Build Review Briefing Pack

Before any dimension analysis, assemble the full context. Sub-agents reviewing code without context hallucinate or miss important design intent.

**Collect:**
- **PR metadata**: title, description, author, labels, linked issues/specs
- **Existing PR comments and reviews**: what has already been discussed — don't duplicate existing feedback
- **Referenced issues and design documents**: if the PR links to a spec or issue, read them for design intent
- **Changed files list**: paths, additions, deletions

**Compute the correct diff.** The PR diff must reflect only the PR's own changes — not unrelated commits on `main` since the branch was created.

```bash
gh pr diff {pr_number} --repo {owner}/{repo}
```

Do not use raw `git diff main..branch`.

Select dimensions with the `reviewing-vsintegration` skill. Pack A if the diff touches `vsintegration/**`. Pack B if it touches `src/Compiler/Service/**` or `src/Compiler/Symbols/**`. Both, if both. Add the three shared dimensions when the change crosses the boundary, owns cancellation, or could make Service depend on `vsintegration`. Invoke `@vs-integration-perf` when the change is about JTF, retained memory, or the perf execution map. Do not copy that agent.

Save the briefing pack to a file. Every Wave 1 sub-agent receives it.

### Wave 1: Find

Launch one sub-agent per selected dimension, in parallel batches of 6. Each evaluates exactly one dimension against the PR diff.

Report `$DimensionName — LGTM` when the dimension is genuinely clean. Do not explain away real issues to produce a clean result.

Report an ISSUE only when you can construct a concrete failing scenario: a specific input, a specific call sequence, a specific state that triggers the bug. No hypotheticals.

Read the PR diff, not main. New files only exist in the PR branch. Never verify findings against `main`. Include exact file path and line range. Verify by tracing actual code flow.

File the finding under the pack of the file. Do not file a host-type leak as a compiler IL issue, and do not file an FCS cache-key bug as a MEF issue.

### Wave 2: Validate

For each non-LGTM finding, actively prove or disprove it:

- Trace callers, callees, state mutations, and error paths on `refs/pull/{pr}/head`.
- Write and run a minimal test when the claim is testable. If the test fails as predicted, the finding is confirmed. If it passes, it is disputed.
- When a full test is too heavy to run inline, write a proof-of-concept snippet another engineer can implement.
- For concurrency, cancellation, and snapshot identity, write a step-by-step execution trace.
- For a borderline finding, validate with 3 distinct available model families. Keep findings confirmed by at least 2 of 3.

A finding is confirmed only with concrete evidence. Never validate against `main`.

### Wave 3: Post

Post confirmed findings as inline review comments at exact file:line. Every comment begins with `🤖🕵️`.

```markdown
🤖🕵️ **[$SEVERITY] $DimensionName**

$Concrete scenario that triggers the bug.

**Recommendation:** $Fix.
```

Post design-level concerns that are not tied to a line as one PR comment, one bullet each, still starting with `🤖🕵️`.

Wave items that used to live on `@expert-reviewer` as IDE impact and incremental-checking checks are these packs. Do not send those findings back to `@expert-reviewer`.

### Wave 4: Summary

Post a dimension checkbox table as the review body. The body starts with `🤖🕵️`.

```markdown
🤖🕵️

| # | Dimension | Verdict |
|---|-----------|---------|
| 1 | Dimension Name | ✅ LGTM |
| 2 | Another | ⚠️ 1 MAJOR |

- [x] Dimension Name
- [ ] Another — description of issue
```

`[x]` means LGTM or nits only. `[ ]` means major or blocking.

All `[x]` → APPROVE. Any blocking → REQUEST_CHANGES. Otherwise → COMMENT.
