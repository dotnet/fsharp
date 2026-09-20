под# Modernizing the F# Interactive Window: Plan

Replace the legacy `FSharp.VS.FSI` tool window with the same REPL engine that powers **C# Interactive** and **Python Interactive** in Visual Studio: the `Microsoft.VisualStudio.InteractiveWindow` / `Microsoft.VisualStudio.VsInteractiveWindow` packages (source: [microsoft/vs-interactive-window](https://github.com/microsoft/vs-interactive-window)), with a structured JSON-RPC connection to the F# Interactive process instead of the current raw stdin/stdout text protocol.

Reference implementation studied: C# Interactive in Roslyn (`src/Interactive/*`, `src/EditorFeatures/Core/Interactive/*`, `src/VisualStudio/*/Impl/Interactive/*`).

---

## 0. Status

**Phase 1 is implemented and tested.** F# Interactive has a JSON-RPC server mode
(`--fsi-server-jsonrpc:<pipe name>`) in `src/fsi/{interactiveProtocol,fsiserver}.fs`, speaking
StreamJsonRpc over a header-delimited stream — the same transport Roslyn's interactive host uses.
The protocol types in `interactiveProtocol.fs` are shared with the host by linking the source, so
the two ends cannot drift apart. §2.2 below documents the protocol as it exists rather than as it
was proposed.

`tests/FSharp.Compiler.Interactive.Server.Tests` holds 35 tests: 23 drive a real fsi process over
the protocol and 12 cover the submission rule described in §2.4. The end-to-end ones are the point —
the handshake, the lifetime of the session, and the interaction between the control channel and the
output streams only exist across a process boundary.

**Phases 0 and 2 are partly implemented.** `vsintegration/src/FSharp.Interactive.Window` compiles
and contains:

- `InteractiveHost.fs` — the client that owns the fsi process and speaks the protocol;
- `FSharpInteractiveEvaluator.fs` — the `IInteractiveEvaluator` the window runs on;
- `SubmissionAnalysis.fs` — the rule deciding when Enter submits, tested;
- `FSharpVsInteractiveWindowProvider.fs` — the MEF component that creates the tool window through
  `IVsInteractiveWindowFactory.Create`, calls `SetLanguage` with the F# content type and language
  service so the input buffer is an F# editor buffer, and reads its options from the
  `SessionsProperties` the existing Tools, Options page already writes. No new options page is
  needed. The session starts in the open solution's folder, so `dotnet fsi` runs the SDK that
  folder's `global.json` resolves to.

Still to do before the window can be opened in Visual Studio:

- registering the tool window with the shell. The existing F# Interactive window is registered by
  `FSharpPackage` in `FSharp.Editor` rather than by a package of its own, and the same route is the
  cheaper one here: a `ProvideInteractiveWindow` attribute and an `IVsToolWindowFactory` hook that
  calls the provider, rather than a new package with its own GUID and pkgdef;
- the `Microsoft.VisualStudio.InteractiveWindow` prerequisite entry in the VSIX manifest, and the
  project's place in the VSIX itself. It is already in `VisualFSharp.slnx`, so it builds;
- commands: open the window, and retargeting Alt+Enter at the new window;
- the debugger attach/detach commands ported from the existing window. The session reports its own
  process id in the handshake, so the attach no longer has to guess which process to target.

Everything from Phase 3 onwards (IntelliSense in the input buffer) is untouched, with one
exception: the input and output buffers have lexical colour from a tokenizer-based classifier
scoped to the window's own buffers. Phase 3 replaces the input half with the editor's semantic
classification when submissions become workspace documents; the output half stays lexical, since
output is not a program. One protocol gap
belongs to that phase: an execution result reports the working directory but not the references and
opens the session has accumulated. F# can get further than C# without them, because the IDE resolves
script references itself through `GetProjectOptionsFromScript` rather than from a response file, but
a session that has run `#r` or `#I` will still drift from what the IDE believes until the result
carries them too.

---

## 1. What we have today (and why it must go)

`vsintegration/src/FSharp.VS.FSI` (~2,500 LOC, essentially unchanged since VS 2005-era IronPython sample code):

| Area | Current implementation | Problem |
|---|---|---|
| Window | Hand-rolled `ToolWindowPane` over COM `IVsTextLines`/`IVsTextView` created via `ILocalRegistry.CreateInstance` (`fsiSessionToolWindow.fs`) | Pre-WPF-editor architecture; read-only region tracked by hand-managed markers; manual caret/scroll/undo management; every editor command (HOME, BACKSPACE, LEFT, RETURN, UP/DOWN) intercepted and reimplemented |
| Language service | MPF `LanguageService` subclass (`FsiLanguageService`) with an empty scanner, empty completions, empty tooltips, `EnableCodeSense <- false` | **Zero IntelliSense** in the REPL: no completion, no quick info, no colorization of input beyond one hardcoded "Keyword" colorable item |
| Process protocol | `dotnet fsi` / `fsiAnyCpu.exe` with redirected stdin/stdout/stderr; prompts detected by scraping the literal string `SERVER-PROMPT>`; a `# 1 "stdin"` line-directive hack per submission; `#silentCd`, `#interactiveprompt "hide"/"show"` magic strings | Fragile text-scraping; output/prompt/echo interleaving bugs; no structured results, no structured diagnostics |
| .NET Core support | First submission writes its own PID and TFM to a temp file (`File.WriteAllLines(pidfile, ...)`), IDE polls the file with `Thread.Sleep(200)` in a loop; the resulting `val it: unit = ()` junk output is skipped by counting lines | A hack on top of a hack; races on startup; breaks if the first output doesn't look as expected |
| Interrupt | Separate `CtrlBreakClient` channel + 1s `timeoutApp` wrapper spinning a threadpool thread with a `ManualResetEvent` | Works, but is a parallel bespoke IPC mechanism |
| Output pumping | 50 ms `System.Windows.Forms.Timer` batching stdout/stderr into the text buffer | WinForms timer in a WPF IDE; ordering between stdout and stderr only approximated |
| History | Hand-written `HistoryBuffer` (cmd.exe model) | The InteractiveWindow package provides history, multi-line submissions, prompt margins for free |
| Error reporting | `System.Windows.Forms.MessageBox.Show(...)` in ~10 places, including inside event handlers | |

What the old window does have that C# Interactive does **not** (must be preserved):

- **Debugging**: attach/detach the VS debugger to the FSI process, "Debug in Interactive" (`#dbgbreak`), debuggability check (`--debug+ --optimize-`) with a suppressible warning dialog. Roslyn's window has no debugging at all — this is an F# advantage to keep.
- Real script semantics: FSI executes actual `.fsx` interactions with `#load`/`#r`/`#i`, and `fsi` object, not a C#-script dialect.
- The .NET Framework hosts (`fsiAnyCpu.exe`, `fsi.exe`, `fsiArm64.exe`) stay with the old window; the new one is .NET-only, because the server mode exists only in the .NET fsi.

---

## 2. Target architecture

Five layers, copied from Roslyn's proven separation. Execution state lives **only** in the FSI process; the IDE keeps a *parallel* type-checking model used purely for IntelliSense; the two are synchronized by structured data flowing back after init and after every submission.

```
┌─ VS shell ────────────────────────────────────────────────────────────┐
│ FSharpVsInteractiveWindowPackage (AsyncPackage, IVsToolWindowFactory) │
│ FSharpVsInteractiveWindowProvider (MEF singleton, owns the window)    │
│ Commands: Open F# Interactive, Send to Interactive, Debug Selection,  │
│           Initialize Interactive with Project                         │
└──────────────────────────────┬────────────────────────────────────────┘
                               │ IVsInteractiveWindowFactory.Create(guid, id, title, evaluator)
                               │ + IVsInteractiveWindow.SetLanguage(FSharpLangServiceGuid, "F#" content type)
┌──────────────────────────────▼────────────────────────────────────────┐
│ REPL WINDOW UI — NOT OUR CODE                                         │
│ NuGet: Microsoft.VisualStudio.InteractiveWindow 4.x                   │
│        Microsoft.VisualStudio.VsInteractiveWindow 4.x                 │
│ (VS prerequisite component; same engine as C# and Python Interactive) │
│ Gives us: projection buffer (prompts + scrollback + editable input),  │
│ history, multiline editing, #help/#cls, IsRunning/IsResetting states, │
│ Enter-vs-newline dispatch via IInteractiveEvaluator.CanExecuteCode    │
└──────────────────────────────┬────────────────────────────────────────┘
                               │ implements IInteractiveEvaluator
┌──────────────────────────────▼────────────────────────────────────────┐
│ EVALUATOR + IDE-SIDE MODEL (in devenv)                                │
│ FSharpInteractiveEvaluator : IInteractiveEvaluator                    │
│ FSharpInteractiveSession — one work queue serializing everything      │
│ Submission documents in FSharp.Editor's workspace → full IntelliSense │
└──────────────────────────────┬────────────────────────────────────────┘
                               │ JSON-RPC 2.0 over a named pipe (control)
                               │ + redirected stdout/stderr (user output)
┌──────────────────────────────▼────────────────────────────────────────┐
│ EXECUTION HOST = fsi itself, in a new server mode                     │
│ dotnet fsi --fsi-server-jsonrpc:<pipe>  (the SDK global.json resolves) │
│ FsiEvaluationSession driven by RPC instead of the stdin ReadLine loop │
└───────────────────────────────────────────────────────────────────────┘
```

### 2.1 Why "fsi in server mode" instead of a separate InteractiveHost.exe

Roslyn ships dedicated `InteractiveHost64/32.exe` binaries because C# scripting is a library (`Microsoft.CodeAnalysis.CSharp.Scripting`) with no standalone process. F# already **has** the process — `fsi` — with the evaluation engine (`FsiEvaluationSession` in `FSharp.Compiler.Interactive.Shell`), an event-loop concept (`fsi.EventLoop`, WinForms/WPF pumping), `#r`/`#load`/`#i` handling, and SDK-based deployment (`dotnet fsi` always matches the user's SDK). Adding a JSON-RPC front-end to fsi:

- kills the `SERVER-PROMPT>` scraping, the PID-file hack, and the `# 1 "stdin"` directive juggling in one move;
- benefits every other fsi client (Ionide, VS Code, custom tooling) — the server mode is a compiler feature, not a VS-only one;
- keeps `dotnet fsi` as the host, resolved from the solution folder's `global.json`, so the window runs the same compiler bits as `dotnet build` there and Visual Studio and SDK versions are decoupled (nothing new to deploy; the desktop `fsiAnyCpu`/`fsiArm64` the VSIX ships stay with the old window).

### 2.2 The RPC protocol (as implemented)

JSON-RPC 2.0 with `Content-Length` framing — the Language Server Protocol wire format. Both ends use
`StreamJsonRpc` with its stock `HeaderDelimitedMessageHandler` and `JsonMessageFormatter`. Requests
are client→server only; the server registers no callbacks, as Roslyn's does not.

| Method | Parameters | Result |
|---|---|---|
| `fsi/initialize` | — (the host names itself with `--fsi-server-client-pid`) | `processId`, `frameworkDescription`, `processArchitecture`, `fsiVersion`, `workingDirectory`, `supportsInterrupt` |
| `fsi/execute` | `code`, optional `sourcePath` and `startLine` | execution result |
| `fsi/executeFile` | `path` | execution result |
| `fsi/setPaths` | `includePaths`, `workingDirectory` | execution result |
| `fsi/interrupt` | — | `interrupted` |
| `fsi/shutdown` | — | ends the session |

An execution result carries `success`, `cancelled`, `diagnostics` (severity, message, error number,
subcategory, file name, and a start/end line and column), `exception` (type, message, stack trace),
and `workingDirectory`.

Three details matter more than the shape:

- `processId` is the process **evaluating code**, which under `dotnet fsi` is not the process that
  was launched. Reporting it in the handshake is what removes the temporary-PID-file dance, and it
  is what a debugger must attach to.
- `sourcePath` and `startLine` make the session emit a line directive around the submission, so a
  selection executed from an editor reports its errors against the user's own file and line.
- `exception` is omitted when the interaction merely failed to compile. The diagnostics already
  describe that, and reporting the exception fsi raises to stop processing would say it twice.

Requests before `fsi/initialize` are refused with a session-not-ready error. Interactions are queued
onto a single worker so that they run in the order they arrived, while `fsi/interrupt` is served as
it arrives — an interrupt that waited its turn behind the interaction it is meant to stop would
never arrive.

User program output keeps flowing through the redirected standard output and error streams. The
prompt is suppressed in this mode, so nothing has to be filtered back out of that stream.

User program output (`printfn`, `Console.*`) keeps flowing through redirected **stdout/stderr**, pumped by two reader threads into `IInteractiveWindow.OutputWriter`/`ErrorOutputWriter` (exactly Roslyn's split: RPC = control plane, std streams = data plane). Encoding pinned to UTF-8 as today (fsi already supports `--fsi-server-output-codepage`).

Process lifecycle (copy Roslyn):

- `LazyRemoteService`-style async-lazy start with cancellation; retry once on startup failure.
- Reset = kill + respawn (no graceful shutdown protocol needed); auto-restart with a "process exited with code N" message on unexpected death.
- Host watches client PID and exits when devenv dies.

### 2.3 IntelliSense in the REPL buffer

The single most valuable user-facing change. Mechanism (Roslyn's, adapted):

1. `SetLanguage(FSharpLanguageServiceGuid, FSharpContentType)` makes every input buffer an F# editor buffer — classification, brace matching and the whole editor command chain light up via the existing `FSharp.Editor` MEF exports.
2. On `SubmissionBufferAdded`, the session creates a document `Submission{N}.fsx` in the Roslyn workspace that `FSharp.Editor` already populates (F# in VS runs on the Roslyn workspace model), opens it against the live `ITextBuffer` (`OpenDocument(docId, buffer.AsTextContainer())`), and gives the editor `ITextDocument` the same synthetic path for future LSP addressability.
3. **Submission chaining for the checker.** F# has no `isSubmission`/`previousSubmission` compilation model exposed in FCS, so phase it:
   - **Phase A (concatenated prelude):** the checker sees one logical `.fsx` = concatenation of all *successfully executed* submissions + current input; diagnostics/completions positions are offset-mapped back to the input buffer. Semantics match fsi closely (shadowing works; it is "as if you retyped the whole session as one script"). Project options come from `GetProjectOptionsFromScript` seeded with the references/opens reported by `InitializationResult` and updated per `ExecutionResult`. Mitigate long-session cost with FCS incremental checking (only the tail changes) and an optional cap.
   - **Phase B (real chaining, upstream FCS work):** expose fsi's own incremental typechecker state (`FsiDynamicCompiler` accumulates a tcState exactly like the IDE needs) through a first-class FCS API — "check this interaction against this accumulated state". This is the F# analogue of Roslyn's `isSubmission: true` + project-reference chain and removes the concatenation model. Design with FCS maintainers; not a blocker for shipping A.
4. Chain only from the last **successful** submission (failed input must not poison later IntelliSense).
5. Handle the init race: the window creates the first input buffer before the host finishes starting → queue pending buffers, drain on process-initialized (Roslyn's `_pendingBuffers`).
6. Freeze classification of executed submissions before reset clears the model (Roslyn's `InertClassifierProvider` trick: snapshot `IClassificationSpan`s into buffer properties, replay forever) so scrollback keeps its colors.
7. Interactive-specific service overrides (keyed on an interactive workspace kind, as Roslyn does): navigation projects spans into the window's surface buffer instead of opening files; rename/refactorings disabled; code fixes only on the active, idle buffer; global undo maps to the window's undo history.
8. REPL command completion: offer `#help`, `#cls`, `#reset`, plus F# directives (`#r`, `#load`, `#I`, `#time`, `#quit`) when the caret is at the start of an interaction.

### 2.4 Submission semantics: `;;` and Enter

`IInteractiveEvaluator.CanExecuteCode` decides Enter-submits vs Enter-inserts-newline:

- Text ending in `;;` (outside strings/comments) → submit (traditional fsi muscle memory preserved).
- Otherwise → submit iff the text is a syntactically complete interaction (FCS parse with `ScriptParseInfo`; incomplete constructs — open `let`, unclosed paren/string — return false). This matches `dotnet fsi`'s modern multiline behavior and C#'s `SyntaxFactory.IsCompleteSubmission`.
- The evaluator appends `;;` before sending to the host if absent; prompts: `> ` primary, `. ` (or `- `) continuation via `GetPrompt()`.

### 2.5 Host selection

The window is .NET-only. The server mode exists in the .NET fsi alone, and the desktop matrix (`fsiAnyCpu.exe`, `fsi.exe`, `fsiArm64.exe`, `#reset` platform arguments, the shadow-copy switch) stays with the old window until that is retired.

The session is started in the open solution's folder, with the `dotnet` a shell there would run (`DOTNET_HOST_PATH`, then `PATH`, then the machine-wide install). The host resolves the SDK from that folder's `global.json` exactly as `dotnet fsi` typed in a shell would, so the window runs the same compiler bits as `dotnet build`, and Visual Studio and SDK versions are decoupled. Nothing in the window re-implements SDK resolution. `FSHARP_INTERACTIVE_PATH` names another fsi for development, until an SDK ships the protocol; the window prints which fsi answered, on what runtime and in which directory, when a session comes up.

### 2.6 Debugging (parity + improvement over C#)

Preserved from the old window, re-hosted on the new one:

- Attach/Detach debugger commands on the window toolbar; attach targets the host PID **which the RPC handshake now reports reliably** (no PID file, no 2-second polling).
- Debuggability check (`--debug+ --optimize-` arg inspection) + suppressible warning — port as-is.
- "Debug in Interactive" (`#dbgbreak` before the selection) — port as-is.

---

## 3. Work plan

### Phase 0 — Spike: new window shell (1–2 weeks) — partly done

Goal: de-risk the InteractiveWindow dependency before touching the compiler.

- Add `Microsoft.VisualStudio.InteractiveWindow` + `VsInteractiveWindow` package references; declare the `Microsoft.VisualStudio.InteractiveWindow` **Prerequisite** in `vsintegration/Vsix/VisualFSharpFull/Source.extension.vsixmanifest` (Roslyn: `[4.0.0.0,5.0.0.0)`).
- New project `vsintegration/src/FSharp.Interactive.Window/` (C# or F#; Roslyn's is C# — C# recommended for MEF attribute ergonomics and to crib code directly).
- Minimal `FSharpInteractiveEvaluator : IInteractiveEvaluator` that wraps the **existing** `Session.FsiSessions` stdin/stdout machinery: `ExecuteCodeAsync` → `SendInput`, output events → `CurrentWindow.OutputWriter`.
- `FSharpVsInteractiveWindowProvider` + `FSharpVsInteractiveWindowPackage` (copy Roslyn's `VsInteractiveWindowProvider`/`VsInteractiveWindowPackage` shape, incl. `IVsToolWindowFactory` for layout persistence).
- `SetLanguage` with the F# content type → confirm classification appears in the input buffer for free.
- Exit criterion: a working "F# Interactive (New)" window behind an experimental feature flag, coexisting with the old one.

### Phase 1 — fsi JSON-RPC server mode (compiler side) — DONE

- `src/Compiler/Interactive/` + `src/fsi/`: new `--fsi-server-jsonrpc:<pipeName>` mode. The dispatch loop runs on a background thread while the main thread drives the event loop; interactions are evaluated via `FsiEvaluationSession.EvalInteractionNonThrowing` marshalled through `EventLoopInvoke`, exactly as the standard input path already does, so GUI scripts behave as they do at the console.
- DTOs + protocol per §2.2; `StreamJsonRpc` dependency for fsi (or a minimal hand-rolled header-delimited JSON layer if adding the dependency to the compiler tree is contentious — decide early with upstream).
- Named pipe with proper ACLs (current-user only; Roslyn reuses its compiler-server `NamedPipeUtil` — fsi can do the same with `FSharp.Compiler`'s pipe helpers or a copy).
- Interrupt over RPC calling the existing ctrl-break machinery; retire `CtrlBreakClient` usage from the VS side.
- Orphan detection: watch client PID, exit on client death.
- Tests: end-to-end host tests modeled on Roslyn's `src/Interactive/HostTest` (init, execute, crash/restart, culture, interrupt, orphaning) — runnable without VS.

### Phase 2 — Evaluator + session on the new protocol — partly done

- `FSharpInteractiveSession`: single `AsyncBatchingWorkQueue`-style queue serializing init/execute/set-paths (reset preempts); `LazyRemoteService` lifecycle port; auto-restart; pending-buffer queue.
- Full `IInteractiveEvaluator`: `CanExecuteCode` (§2.4), `GetPrompt`, `ResetAsync`, `InitializeAsync`, `AbortExecution` → RPC `Interrupt` (note: this makes F# *better* than C# Interactive, whose `AbortExecution` is an unimplemented TODO).
- Structured diagnostics from `ExecutionResult` rendered as error-classified output.
- Delete the stdin/stdout path from the new window (old window untouched).

### Phase 3 — IntelliSense (3–5 weeks, partially parallel with Phase 2)

- Submission documents in the workspace + open-buffer wiring + synthetic `Submission{N}.fsx` paths (§2.3 items 1–2).
- Concatenated-prelude checking model with position offset mapping (§2.3 Phase A); completion/quick info/signature help/diagnostics in the input buffer.
- Init-race pending buffers; chain-from-last-successful; inert classification on reset.
- Interactive workspace-kind service overrides (navigation/rename/fixes/undo).
- REPL command + hash-directive completion.
- File an FCS design issue for §2.3 Phase B (exposing fsi's incremental tcState) — long-lead upstream conversation.

### Phase 4 — Commands, options, project integration (2–3 weeks)

- Rewire `MenusAndCommands.vsct` targets: Alt+Enter Send Selection/Line, "Execute in Interactive", "Debug in Interactive" → new window (`window.SubmitAsync`, preserving the no-selection→current-line + caret-advance behavior; consider Roslyn's syntax-aware selection expansion from `SendToInteractiveSubmissionProvider`).
- `AddReferences` (Solution Explorer "Send project references to F# Interactive") → `#r` submissions.
- Optional: "Initialize Interactive with Project" parity — build project, reset, `SetPaths`, `#r` output assembly + references, `open` default namespaces (Roslyn's `ResetInteractive` flow).
- Port `FsiPropertyPage` (Tools → Options → F# Tools → F# Interactive): args, langversion preview, debug mode. The platform default and shadow copy belong to the desktop fsi and stay with the old window.
- F1 help keyword.

### Phase 5 — Debugging parity (1–2 weeks)

- Port attach/detach commands, debuggability check + registry-backed suppression, `#dbgbreak` flow onto the new window/host (PID now from handshake).

### Phase 6 — Cutover and deletion (1–2 weeks)

- Flip the feature flag default; one release of coexistence if desired.
- Delete `FSharp.VS.FSI` (all of `fsiSessionToolWindow.fs`, `fsiLanguageService.fs`, `fsiTextBufferStream.fs`, `sessions.fs` stdin machinery, MPF `Microsoft.VisualStudio.Package.LanguageService.15.0` dependency), the `SERVER-PROMPT` support code in fsi (after a deprecation window — external tools may scrape it), `ITestVFSI` (replace tests with InteractiveWindow-based test host, cf. Roslyn's `InteractiveWindowTestHost`).
- Localization: migrate `VFSIstrings` still in use; drop the rest.
- Update docs; announce the fsi server mode publicly (Ionide et al. will want it).

Total: roughly 3–4 months of focused work; Phases 1 and 3 carry the technical risk.

---

## 4. Risks and open questions

1. **vs-interactive-window is in maintenance mode.** Acceptable: VS ships it as a prerequisite component and C#/Python depend on it; API surface is stable. Fallback exists (fork is MIT).
2. ~~**StreamJsonRpc in the compiler tree**~~ — **resolved: both ends use it.** The transport is
   `StreamJsonRpc` over a `HeaderDelimitedMessageHandler`, the same combination Roslyn's interactive
   host uses, at both ends. Nothing about JSON or its framing is written by hand.

   The version is pinned to 2.26.10, which is what Roslyn's packages already force into
   `vsintegration`. Pinning it to anything else is what would turn a Roslyn bump into a downgrade
   conflict, so the central version must follow Roslyn rather than lead it.

   The cost is real and worth stating: fsi's output gains eight assemblies, about 3 MB —
   `StreamJsonRpc`, `Newtonsoft.Json`, `MessagePack` (and its annotations), `Nerdbank.MessagePack`,
   `Nerdbank.Streams`, `Microsoft.VisualStudio.Threading` and `Microsoft.VisualStudio.Validation`.
   In exchange, both ends of the protocol rest on one battle-tested implementation instead of on
   framing and dispatch maintained here. On the Visual Studio side there is no cost at all: Visual
   Studio already loads the library, so the reference is compile-time only.
3. **Concatenated-prelude checker cost** on very long sessions — measure; FCS caches aggressively for single-file edits, and the prelude prefix is immutable between submissions. Phase B removes the concern structurally.
4. **`it` and value printing**: keep fsi's stdout printing as the source of truth (don't reformat in the IDE) — avoids divergence with `fsi.PrintDepth`/formatters users set in scripts.
5. **`#quit`** must terminate cleanly in server mode (host exits → window prints exit message, next Enter restarts — matches current behavior).
6. **Upstreaming**: split PRs — (1) fsi server mode (compiler repo, no VS dependency, independently testable), (2) VS window (vsintegration). The plan intentionally keeps the seam clean.
7. **LSP future**: submission documents with real paths + workspace registration keep the door open for serving REPL buffers over LSP later (Roslyn already registers its interactive workspace with LSP), aligning with the ongoing LSP work in this fork.
