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
open FSharp.Compiler.Text

// Add additional imports/constructs into this script text to verify that common scenarios
// for FCS script typechecking can be supported
let scriptSource = """
open System
let pi = Math.PI
"""

[<TestCase(true, false, [| "--targetprofile:mscorlib" |])>]
[<TestCase(false, true, [| "--targetprofile:netcore" |])>]
[<Test>]
let ``can generate options for different frameworks regardless of execution environment``(useDotNetFramework, useSdk, flags) =
    let path = Path.GetTempPath()
    let file = Path.GetTempFileName()
    let tempFile = Path.Combine(path, file)
    let (_, errors) =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, useDotNetFramework = useDotNetFramework, useSdkRefs = useSdk, otherFlags = flags)
        |> Async.RunSynchronously
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with useDotNetFramework:%b, useSdkRefs:%b, and otherFlags:%A:\n%A" useDotNetFramework useSdk flags errors

[<TestCase(true, false, [| "--targetprofile:mscorlib" |])>]
[<TestCase(false, true, [| "--targetprofile:netcore" |])>]
[<Test>]
let ``all default assembly references are system assemblies``(useDotNetFramework, useSdk, flags) =
    let path = Path.GetTempPath()
    let file = Path.GetTempFileName()
    let tempFile = Path.Combine(path, file)
    let (options, errors) =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, useDotNetFramework = useDotNetFramework, useSdkRefs = useSdk, otherFlags = flags)
        |> Async.RunSynchronously
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with assumeDotNetFramework:%b, useSdkRefs:%b, and otherFlags:%A:\n%A" useDotNetFramework useSdk flags errors
    for r in options.OtherOptions do 
        if r.StartsWith("-r:") then 
            let ref = Path.GetFullPath(r.[3..])
            let baseName = Path.GetFileNameWithoutExtension(ref)
            if not (FSharp.Compiler.FxResolver(Some useDotNetFramework).GetSystemAssemblies().Contains(baseName)) then
                printfn "Failing, printing options from GetProjectOptionsFromScript..."
                for opt in options.OtherOptions do
                    printfn "option: %s" opt
                failwithf "expected FSharp.Compiler.DotNetFrameworkDependencies.systemAssemblies to contain '%s' because '%s' is a default reference for a script, (assumeNetFx, useSdk, flags) = %A" baseName ref (useDotNetFramework, useSdk, flags) 
