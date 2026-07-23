// Because of script closure cache.
[<FSharp.Test.RunTestCasesInSequence>]
module FSharp.Compiler.Service.Tests.ScriptOptionsTests

open Xunit
open System.IO
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open TestFramework

// Add additional imports/constructs into this script text to verify that common scenarios
// for FCS script typechecking can be supported
let scriptSource = """
open System
let pi = Math.PI
"""

[<Theory>]
[<InlineData(false, false, "--targetprofile:mscorlib")>]
[<InlineData(false, true, "--targetprofile:mscorlib")>]
[<InlineData(true, true, "--targetprofile:mscorlib")>]
[<InlineData(false, false, "--targetprofile:netstandard")>]
[<InlineData(false, true, "--targetprofile:netstandard")>]
[<InlineData(true, true, "--targetprofile:netstandard")>]
[<InlineData(false, false, "--targetprofile:netcore")>]
[<InlineData(false, true, "--targetprofile:netcore")>]
[<InlineData(true, true, "--targetprofile:netcore")>]
let ``can generate options for different frameworks regardless of execution environment - useSdkRefs = false``(assumeDotNetFramework, useSdkRefs, flag) =
    let path = Path.GetTempPath()
    let file = getTemporaryFileName () + ".fsx"
    let tempFile = Path.Combine(path, file)
    let _, errors =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, assumeDotNetFramework = assumeDotNetFramework, useSdkRefs = useSdkRefs, otherFlags = [| flag |])
        |> Async.RunImmediate
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with otherFlags:%A:\n%A" [| flag |] errors

[<Theory>]
// Bugbug: https://github.com/dotnet/fsharp/issues/14781
//[<InlineData("--targetprofile:mscorlib")>]
[<InlineData("--targetprofile:netcore")>]
[<InlineData("--targetprofile:netstandard")>]
let ``can resolve nuget packages to right target framework for different frameworks regardless of execution environment`` flag =
    let dir = DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
    let file = (getTemporaryFileNameInDirectory dir) + ".fsx"
    let scriptFullPath = Path.Combine(dir.FullName, file)
    let scriptSource = """
#r "nuget: FSharp.Data, 3.3.3"
open System
let pi = Math.PI
"""
    let options, errors =
        checker.GetProjectOptionsFromScript(file, SourceText.ofString scriptSource, assumeDotNetFramework = false, useSdkRefs = true, otherFlags = [|flag|])
        |> Async.RunImmediate
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with assumeDotNetFramework:%b, useSdkRefs:%b, and otherFlags:%A:\n%A" false true [|flag|] errors
    let expectedReferenceText = match [| flag |] |> Array.tryFind(fun f -> f = "--targetprofile:mscorlib") with | Some _ -> "net45" | _ -> "netstandard2.0"
    let found = options.OtherOptions |> Array.exists (fun s -> s.Contains(expectedReferenceText) && s.Contains("FSharp.Data.dll"))
    Assert.True(found)

/// `SourceFiles` is exactly the single script; the fsi default-reference injection for a missing `#load`
/// is a host-layout detail (not product behaviour) and is intentionally not asserted.
/// Desktop-only: it asserts .NET Framework GAC assemblies (`System.Runtime.Remoting`/`System.Transactions`)
/// resolve, which is not possible on a .NET-Core-only host.
#if !NETCOREAPP
[<Fact>]
let ``Fsx.ScriptClosure.SurfaceOrderOfHashes`` () =
    let scriptSource =
        String.concat "\n"
            [ "#r \"System.Runtime.Remoting\""
              "#r \"System.Transactions\""
              "#load \"Load1.fs\""
              "#load \"Load2.fsx\"" ]
    let tempFile = Path.Combine(Path.GetTempPath(), getTemporaryFileName () + ".fsx")
    let options, _errors =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource)
        |> Async.RunImmediate
    let containsPartial (needle: string) = options.OtherOptions |> Array.exists (fun o -> o.Contains needle)
    Assert.True(containsPartial "--noframework", "OtherOptions should contain --noframework")
    Assert.True(containsPartial "System.Runtime.Remoting.dll", "OtherOptions should resolve System.Runtime.Remoting.dll")
    Assert.True(containsPartial "System.Transactions.dll", "OtherOptions should resolve System.Transactions.dll")
    Assert.Equal(1, options.SourceFiles.Length)
    Assert.Equal(tempFile, options.SourceFiles.[0])
#endif

/// A no-crash test: the invalid meta-command filenames must be processed WITHOUT crashing the script
/// options (a single source file, the `--noframework` closure flag) without throwing — the invalid
/// references surface as non-fatal resolution diagnostics, never an assert.
[<Fact>]
let ``Fsx.InvalidMetaCommandFilenames`` () =
    let scriptSource =
        String.concat "\n"
            [ "#r @\"\""
              "#load @\"\""
              "#I @\"\""
              "#r @\"*\""
              "#load @\"*\""
              "#I @\"*\""
              "#r @\"?\""
              "#load @\"?\""
              "#I @\"?\""
              "#r @\"C:\\path\\does\\not\\exist.dll\"  " ]
    let tempFile = Path.Combine(Path.GetTempPath(), getTemporaryFileName () + ".fsx")
    let options, _errors =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource)
        |> Async.RunImmediate
    Assert.Equal(1, options.SourceFiles.Length)
    Assert.Equal(tempFile, options.SourceFiles.[0])
    Assert.Contains("--noframework", options.OtherOptions)
