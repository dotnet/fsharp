// Allow-list audit of the TFM (#if) guards in src/FSharp.Core (the "#ifdef management" gate).
//
// Every TFM-discriminating #if/#elif in src/FSharp.Core/**/*.{fs,fsi} must be allow-listed below by
// (<file>, <normalized guard-expr>). A non-allow-listed guard fails the audit, forcing review. This
// catches the regressions that silently mistarget the shipped net TFM:
//   * bare  #if NETSTANDARD2_1   -> excludes net, dropping a feature there
//   * bare  #if !NET             -> drops a BCL polyfill on EVERY net (wrong floor)
// Standardized idioms: `#if NETSTANDARD2_1_OR_GREATER || NET` (feature on ns2.1 + all net) and
// `#if !NET5_0_OR_GREATER` / `#if !NET8_0_OR_GREATER` (polyfill below a BCL floor).
//
// Run:       dotnet fsi eng/tests/AuditFSharpCoreTfmGuards.fsx  [--self-test]
// Uses `git ls-files` + the .NET regex engine (not `git grep -P`, absent on macOS git builds).

open System
open System.Diagnostics
open System.Text.RegularExpressions

let scopeGlobs = [ "src/FSharp.Core/*.fs"; "src/FSharp.Core/*.fsi" ]

// The reviewed, allow-listed TFM guards after standardization. Key = (repo-relative path with '/',
// normalized guard expression). To add an entry: confirm the guard uses the idiom above, then list
// it here with a one-line justification in the PR.
let allowList : Set<string * string> =
    set [
        // Collection-expression support ([<CollectionBuilder>] + Create(ReadOnlySpan<_>)): present on
        // netstandard2.1 and every net TFM.
        "src/FSharp.Core/set.fs",          "NETSTANDARD2_1_OR_GREATER || NET"
        "src/FSharp.Core/set.fsi",         "NETSTANDARD2_1_OR_GREATER || NET"
        "src/FSharp.Core/prim-types.fs",   "NETSTANDARD2_1_OR_GREATER || NET"
        "src/FSharp.Core/prim-types.fsi",  "NETSTANDARD2_1_OR_GREATER || NET"
        // task { use! ... } over IAsyncDisposable (TryFinallyAsync / TaskBuilderBase.Using): ns2.1 + net.
        "src/FSharp.Core/tasks.fs",        "NETSTANDARD2_1 || NET"
        "src/FSharp.Core/tasks.fsi",       "NETSTANDARD2_1 || NET"
        // BCL polyfills that must be dropped once the BCL provides the type:
        //   DynamicallyAccessedMembers  -> floor at NET5
        //   CollectionBuilder/ScopedRef -> floor at NET8
        "src/FSharp.Core/prim-types.fs",   "!NET5_0_OR_GREATER"
        "src/FSharp.Core/prim-types.fsi",  "!NET5_0_OR_GREATER"
        "src/FSharp.Core/prim-types.fs",   "!NET8_0_OR_GREATER"
        "src/FSharp.Core/prim-types.fsi",  "!NET8_0_OR_GREATER"
    ]

// A guard expression is "TFM-discriminating" if it mentions any of these tokens.
let tfmToken =
    Regex(@"NETSTANDARD|NETCOREAPP|NETFRAMEWORK|NET\d|(?:^|[^0-9A-Za-z_])!?NET(?:[^0-9A-Za-z_]|$)",
          RegexOptions.Compiled)

let guardLine = Regex(@"^\s*#(?:if|elif)\s+(?<expr>.*\S)\s*$", RegexOptions.Compiled)

let normalize (s: string) = Regex.Replace(s, @"\s+", " ").Trim()

let runGit (args: string) =
    let psi = ProcessStartInfo("git", args, RedirectStandardOutput = true, UseShellExecute = false)
    use p = Process.Start psi
    let out = p.StandardOutput.ReadToEnd()
    p.WaitForExit()
    if p.ExitCode <> 0 then failwithf "git %s failed (exit %d)" args p.ExitCode
    out

let trackedFiles () =
    runGit ("ls-files -- " + String.Join(" ", scopeGlobs))
    |> fun s -> s.Split([| '\n'; '\r' |], StringSplitOptions.RemoveEmptyEntries)
    |> Array.map (fun p -> p.Trim())
    |> Array.filter (fun p -> p <> "")

// Returns the TFM guards found in the given lines as (normalizedExpr) values.
let tfmGuardsIn (lines: string[]) =
    lines
    |> Array.choose (fun ln ->
        let m = guardLine.Match ln
        if m.Success then
            let expr = normalize m.Groups.["expr"].Value
            if tfmToken.IsMatch expr then Some expr else None
        else None)

let auditRepo () =
    let violations =
        [ for file in trackedFiles () do
            let path = file.Replace('\\', '/')
            for expr in tfmGuardsIn (IO.File.ReadAllLines file) do
                if not (allowList.Contains(path, expr)) then
                    yield path, expr ]
    if violations.IsEmpty then
        printfn "FSharp.Core TFM #if guard audit: OK (%d allow-listed guards)." allowList.Count
        0
    else
        eprintfn "FSharp.Core TFM #if guard audit: FAILED. Non-allow-listed TFM guard(s):"
        for (path, expr) in violations do
            eprintfn "  %s:  #if %s" path expr
        eprintfn ""
        eprintfn "Use the standardized idiom (see eng/tests/AuditFSharpCoreTfmGuards.fsx header):"
        eprintfn "  * feature on ns2.1 + all net:  #if NETSTANDARD2_1_OR_GREATER || NET"
        eprintfn "  * polyfill below a BCL floor:   #if !NET8_0_OR_GREATER  (or !NET5_0_OR_GREATER)"
        eprintfn "A bare '#if NETSTANDARD2_1' or '#if !NET' is almost always wrong for the shipped net TFM."
        eprintfn "If the new guard is a legitimate use of the idiom, add its <file>:<expr> to the allow-list."
        1

// Self-test: prove the detector actually bites on the known anti-patterns and passes the good idioms.
let selfTest () =
    let mutable ok = true
    let check desc (expr: string) shouldBeTfm =
        // Drive the REAL extractor (guardLine -> normalize -> tfmToken) via a synthetic #if line, so a
        // drift in the #if/#elif line parser cannot silently green this self-test while the audit goes blind.
        let detected = tfmGuardsIn [| sprintf "#if %s" expr |] |> Array.isEmpty |> not
        if detected <> shouldBeTfm then
            ok <- false
            eprintfn "  self-test FAIL: %s -> detected=%b, expected %b" desc detected shouldBeTfm
    // These MUST be recognized as TFM guards (and, not being allow-listed for a fake file, would fail):
    check "bare NETSTANDARD2_1" "NETSTANDARD2_1" true
    check "bare !NET" "!NET" true
    check "NET5_0_OR_GREATER" "NET5_0_OR_GREATER" true
    check "NETCOREAPP" "NETCOREAPP" true
    check "good idiom || NET" "NETSTANDARD2_1_OR_GREATER || NET" true
    // These are NOT TFM guards and must be ignored by the audit:
    check "DEBUG" "DEBUG" false
    check "FX_NO_SOMETHING" "FX_NO_SOMETHING" false
    // The line parser must recognize #elif, not only #if.
    if tfmGuardsIn [| "#elif NETSTANDARD2_1_OR_GREATER || NET" |] |> Array.isEmpty then
        ok <- false; eprintfn "  self-test FAIL: #elif TFM guard not detected"
    // Allow-list containment behaves as expected:
    if not (allowList.Contains("src/FSharp.Core/tasks.fs", "NETSTANDARD2_1 || NET")) then
        ok <- false; eprintfn "  self-test FAIL: known-good pair not in allow-list"
    if allowList.Contains("src/FSharp.Core/tasks.fs", "NETSTANDARD2_1") then
        ok <- false; eprintfn "  self-test FAIL: bare NETSTANDARD2_1 unexpectedly allow-listed"
    if ok then
        printfn "FSharp.Core TFM #if guard audit self-test: OK."; 0
    else
        eprintfn "FSharp.Core TFM #if guard audit self-test: FAILED."; 1

let args = Environment.GetCommandLineArgs()
let exitCode = if Array.contains "--self-test" args then selfTest () else auditRepo ()
exit exitCode
