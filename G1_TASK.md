# G1: decode the delta for the Giraffe stale-string edit

Worktree: /Users/nat/Projects/hot_reload_poc/fsharp-opt. Build with ./.dotnet/dotnet.
BUG under investigation: on the real Giraffe sample (module-level `let handler1: HttpHandler = fun
(_: HttpFunc) (ctx: HttpContext) -> ctx.WriteTextAsync "Hello World"`, eta-expanded to a static
method), a same-line string-literal edit produces a delta that the runtime applies successfully
(updatedMethods = the correct Program::handler1 row) but the endpoint keeps serving the OLD string.
The synthetic test apps show string edits correctly. Prime hypothesis H1: the updated method body's
ldstr token (0x70xxxxxx) resolves into the BASELINE #US heap (old string) instead of the delta's
appended #US entry (delta US offsets must be ABSOLUTE, i.e. baseline-heap-length-based, per the
delta format docs in this repo: HOT_RELOAD_SPEC / notes/delta_format may exist at the workspace root
one level up - do not rely on them, decode from first principles with SRM).

## Step 1: build a session-level repro test (scratch, in tests/FSharp.Compiler.Service.Tests/HotReload/)
Follow the existing session tests' shape (HotReloadSessionTests.fs: checker.Compile baseline to
disk, ResetSessionState, CreateHotReloadSession, AddProject, edit source, checker.Compile again OR
use the in-process flag - MATCH THE GIRAFFE TOPOLOGY: flag-off baseline compile, then EmitDelta with
FSHARP_HOTRELOAD_INPROCESS_COMPILE=1 so the fresh compile is in-process). Source shape mimicking
Giraffe (no ASP.NET dependency needed):
  module Sample.G1
  type HttpFunc = string -> string
  let handler1: (HttpFunc -> string -> string) = fun (_: HttpFunc) (ctx: string) -> "Hello World"
  let probe () = handler1 id "x"
Plus 2-3 more module-level closures AFTER handler1 (mimic the endpoints list: a list of closures)
so the file is closure-dense. Edit ONLY the literal "Hello World" -> "Hello World EDITED" (same
line, no line count change). EmitDelta must return Ok with exactly 1 updated method.

## Step 2: decode the delta
From the Ok delta (fields: Metadata: byte[], IL: byte[], Pdb; plus UpdatedMethods tokens):
1. Read the BASELINE assembly bytes (the on-disk baseline DLL): via System.Reflection.Metadata get
   the baseline #US heap SIZE and verify reading the old string at its offset.
2. Parse delta.Metadata with MetadataReaderProvider.FromMetadataImage: enumerate EncLog/EncMap, the
   MethodDef row(s) updated, and dump the delta #US heap bytes (raw) - what string(s) does it carry
   and at what ABSOLUTE offsets do they live (delta heap offsets are logically appended after the
   baseline heap)?
3. Parse delta.IL: locate the method body (tiny: header + IL). Extract the ldstr operand token(s)
   (opcode 0x72, little-endian 4-byte token, high byte 0x70). Compute offset = token & 0x00FFFFFF.
4. VERDICT: offset < baselineUsHeapSize means the ldstr points at the BASELINE heap (H1 CONFIRMED:
   which string does that offset decode to in the baseline? presumably the OLD "Hello World").
   offset >= baselineUsHeapSize means it points into the delta's appended US entries (H1 dead:
   decode which delta string it hits and whether that is the NEW string; then the bug is elsewhere -
   report what you see).
5. CONTRAST: run the same decode on the delta from an edit shape that WORKS (the existing session
   test's simple module function edit, e.g. reuse the flag test's libValue shape) and report its
   ldstr offset vs baseline heap size. The difference between the two is the bug's fingerprint.

## Step 3: report
G1_REPORT.md: repro test name + whether it reproduces (if the repro's delta decodes CORRECTLY, say
so explicitly and describe what you tried; do NOT force a repro), the decoded numbers for both
cases (baseline US size, delta US contents+offsets, ldstr operands), the verdict on H1, and if
confirmed, your analysis of WHERE the offset goes wrong (the user-string token calculators live in
src/Compiler/CodeGen/IlxDeltaStreams.fs UserStringTokenCalculator; the emitter's US remap trace is
under traceUserStringUpdates in IlxDeltaEmitter.fs - grep '[fsharp-hotreload][userstrings]').
Suites: run the HotReload service suite once at the end to prove no regressions from your scratch
test (it may stay in the tree as a regression test if it reproduces; name it for G1).

## Rules
No commits/pushes/git changes. No em dashes. No AI mentions. Keep the repro minimal.
