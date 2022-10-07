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

[<TestCase(true, false, [| "--targetprofile:mscorlib" |])>]
[<TestCase(false, false, [| "--targetprofile:netcore" |])>]
[<TestCase(false, true, [| "--targetprofile:netcore" |])>]
[<Test>]
let ``can generate options for different frameworks regardless of execution environment``(assumeNetFx, useSdk, flags) =
    let path = Path.GetTempPath()
    let file = tryCreateTemporaryFileName () + ".fsx"
    let tempFile = Path.Combine(path, file)
    let _, errors =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, assumeDotNetFramework = assumeNetFx, useSdkRefs = useSdk, otherFlags = flags)
        |> Async.RunImmediate
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with assumeDotNetFramework:%b, useSdkRefs:%b, and otherFlags:%A:\n%A" assumeNetFx useSdk flags errors

#if NETFRAMEWORK
// See https://github.com/dotnet/fsharp/pull/13994#issuecomment-1259663865
//
// .NET Core-based tooling can't resolve nuget packages to .NET Framework references
[<TestCase(true, false, [| "--targetprofile:mscorlib" |])>]
#endif
[<TestCase(false, true, [| "--targetprofile:netcore" |])>]
[<Test>]
let ``can resolve nuget packages to right target framework for different frameworks regardless of execution environment``(assumeNetFx, useSdk, flags) =
    let path = Path.GetTempPath()
    let file = tryCreateTemporaryFileName () + ".fsx"
    let tempFile = Path.Combine(path, file)
    let scriptSource = """
#r "nuget: FSharp.Data, 3.3.3"
open System
let pi = Math.PI
"""
    let options, errors =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, assumeDotNetFramework = assumeNetFx, useSdkRefs = useSdk, otherFlags = flags)
        |> Async.RunImmediate
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with assumeDotNetFramework:%b, useSdkRefs:%b, and otherFlags:%A:\n%A" assumeNetFx useSdk flags errors
    let expectedReferenceText = (if assumeNetFx then "net45" else "netstandard2.0")
    let found = options.OtherOptions |> Array.exists (fun s -> s.Contains(expectedReferenceText) && s.Contains("FSharp.Data.dll"))
    Assert.IsTrue(found)

// This test atempts to use a bad SDK number 666.666.666
//
// It's disabled because on CI server the SDK is still being resolved to 5.0.101 by `dotnet --version`.
//
// This must be because of some setting in the CI build scripts - e.g. an environment variable
// that allows SDK resolution to be overriden. I've tried to track this down by looking through
// https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet resolution rules
// and the F# and CI settings but can't find the setting that is causing this.
//
// Because of this the test has been manually verified by running locally.
//[<Test>]
let ``sdk dir with dodgy global json gives warning``() =
    let tempFile = tryCreateTemporaryFileName () + ".fsx"
    let tempPath = Path.GetDirectoryName(tempFile)
    let globalJsonPath = Path.Combine(tempPath, "global.json")
    FileSystem.OpenFileForWriteShim(globalJsonPath).Write("""{ "sdk": { "version": "666.666.666" } }""")
    let options, errors =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, assumeDotNetFramework = false, useSdkRefs = true, otherFlags = [| |])
        |> Async.RunImmediate
    FileSystem.FileDeleteShim(globalJsonPath)
    match errors with
    | [] ->
        printfn "Failure!"
        printfn "tempPath = %A" tempPath
        printfn "options = %A" options
        let fxResolver = FSharp.Compiler.FxResolver(false, tempPath, rangeForErrors=range0, useSdkRefs=true, isInteractive=false, sdkDirOverride=None)
        let result = fxResolver.TryGetDesiredDotNetSdkVersionForDirectory()
        printfn "sdkVersion = %A" result

        printfn "options = %A" options
        failwith "Expected error while parsing script"
    | errors ->
       for error in errors do
           // {C:\Users\Administrator\AppData\Local\Temp\tmp6F0F.tmp.fsx (0,1)-(0,1) The .NET SDK for this script could not be determined. If the script is in a directory using a 'global.json' ensure the relevant .NET SDK is installed. The output from 'C:\Program Files\dotnet\dotnet.exe --version' in the script directory was: '        2.1.300 [C:\Program Files\dotnet\sdk]
           Assert.AreEqual(3384, error.ErrorNumber)
           Assert.AreEqual(tempFile, error.FileName)
