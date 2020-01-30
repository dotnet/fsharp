#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.ScriptOptions
#endif

open NUnit.Framework
open FsUnit
open System
open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Service.Tests.Common

// Add additional imports/constructs into this script text to verify that common scenarios
// for FCS script typechecking can be supported
let scriptSource = """
open System
let pi = Math.PI
"""

[<TestCase(true, false, [| "--targetprofile:mscorlib" |])>]
[<TestCase(false, true, [| "--targetprofile:netcore" |])>]
[<Test>]
let ``can generate options for different frameworks regardless of execution environment``(assumeNetFx, useSdk, flags) =
    let path = IO.Path.GetTempPath()
    let file = IO.Path.GetTempFileName()
    let tempFile = IO.Path.Combine(path, file)
    let (options, errors) =
        checker.GetProjectOptionsFromScript(tempFile, Text.SourceText.ofString scriptSource, assumeDotNetFramework = assumeNetFx, useSdkRefs = useSdk, otherFlags = flags)
        |> Async.RunSynchronously
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with assumeDotNetFramework:%b, useSdkRefs:%b, and otherFlags:%A:\n%A" assumeNetFx useSdk flags errors