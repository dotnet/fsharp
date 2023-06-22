#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.ScriptOptions
#endif

open NUnit.Framework
open System.IO
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.IO
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text
open TestFramework

// Add additional imports/constructs into this script text to verify that common scenarios
// for FCS script typechecking can be supported
let scriptSource = """
open System
let pi = Math.PI
"""

[<TestCase(false, false, [| "--targetprofile:mscorlib" |])>]
[<TestCase(false, true, [| "--targetprofile:mscorlib" |])>]
[<TestCase(true, true, [| "--targetprofile:mscorlib" |])>]
[<TestCase(false, false, [| "--targetprofile:netstandard" |])>]
[<TestCase(false, true, [| "--targetprofile:netstandard" |])>]
[<TestCase(true, true, [| "--targetprofile:netstandard" |])>]
[<TestCase(false, false, [| "--targetprofile:netcore" |])>]
[<TestCase(false, true, [| "--targetprofile:netcore" |])>]
[<TestCase(true, true, [| "--targetprofile:netcore" |])>]
[<Test>]
let ``can generate options for different frameworks regardless of execution environment - useSdkRefs = false``(assumeDotNetFramework, useSdkRefs, flags) =
    let path = Path.GetTempPath()
    let file = tryCreateTemporaryFileName () + ".fsx"
    let tempFile = Path.Combine(path, file)
    let _, errors =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, assumeDotNetFramework = assumeDotNetFramework, useSdkRefs = useSdkRefs, otherFlags = flags)
        |> Async.RunImmediate
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with otherFlags:%A:\n%A" flags errors

// Bugbug: https://github.com/dotnet/fsharp/issues/14781
//[<TestCase([| "--targetprofile:mscorlib" |])>]
[<TestCase([| "--targetprofile:netcore" |])>]
[<TestCase([| "--targetprofile:netstandard" |])>]
[<Test>]
let ``can resolve nuget packages to right target framework for different frameworks regardless of execution environment``(flags) =
    let path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
    let file = tryCreateTemporaryFileNameInDirectory(path) + ".fsx"
    let scriptFullPath = Path.Combine(path, file)
    let scriptSource = """
#r "nuget: FSharp.Data, 3.3.3"
open System
let pi = Math.PI
"""
    let options, errors =
        checker.GetProjectOptionsFromScript(file, SourceText.ofString scriptSource, assumeDotNetFramework = false, useSdkRefs = true, otherFlags = flags)
        |> Async.RunImmediate
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with assumeDotNetFramework:%b, useSdkRefs:%b, and otherFlags:%A:\n%A" false true flags errors
    let expectedReferenceText = match flags |> Array.tryFind(fun f -> f = "--targetprofile:mscorlib") with | Some _ -> "net45" | _ -> "netstandard2.0"
    let found = options.OtherOptions |> Array.exists (fun s -> s.Contains(expectedReferenceText) && s.Contains("FSharp.Data.dll"))
    Assert.IsTrue(found)
