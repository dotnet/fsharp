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
open FSharp.Compiler.Range

// Add additional imports/constructs into this script text to verify that common scenarios
// for FCS script typechecking can be supported
let scriptSource = """
open System
let pi = Math.PI
"""

[<TestCase(true, false, [| "--targetprofile:mscorlib" |])>]
[<TestCase(false, true, [| "--targetprofile:netcore" |])>]
[<Test>]
let ``can generate options for different frameworks regardless of execution environment``(assumeDotNetFramework, useSdk, flags) =
    let path = Path.GetTempPath()
    let file = Path.GetTempFileName()
    let tempFile = Path.Combine(path, file)
    let (_, errors) =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, assumeDotNetFramework = assumeDotNetFramework, useSdkRefs = useSdk, otherFlags = flags)
        |> Async.RunSynchronously
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with assumeDotNetFramework:%b, useSdkRefs:%b, and otherFlags:%A:\n%A" assumeDotNetFramework useSdk flags errors

[<TestCase(true, false, [| "--targetprofile:mscorlib" |])>]
[<TestCase(false, true, [| "--targetprofile:netcore" |])>]
[<Test>]
let ``all default assembly references are system assemblies``(assumeDotNetFramework, useSdk, flags) =
    let tempFile = Path.GetTempFileName() + ".fsx"
    let (options, errors) =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, assumeDotNetFramework = assumeDotNetFramework, useSdkRefs = useSdk, otherFlags = flags)
        |> Async.RunSynchronously
    match errors with
    | [] -> ()
    | errors -> failwithf "Error while parsing script with assumeDotNetFramework:%b, useSdkRefs:%b, and otherFlags:%A:\n%A" assumeDotNetFramework useSdk flags errors
    for r in options.OtherOptions do 
        if r.StartsWith("-r:") then 
            let ref = Path.GetFullPath(r.[3..])
            let baseName = Path.GetFileNameWithoutExtension(ref)
            let projectDir = System.Environment.CurrentDirectory
            if not (FSharp.Compiler.FxResolver(Some assumeDotNetFramework, projectDir, range0).GetSystemAssemblies().Contains(baseName)) then
                printfn "Failing, printing options from GetProjectOptionsFromScript..."
                for opt in options.OtherOptions do
                    printfn "option: %s" opt
                failwithf "expected FSharp.Compiler.DotNetFrameworkDependencies.systemAssemblies to contain '%s' because '%s' is a default reference for a script, (assumeNetFx, useSdk, flags) = %A" baseName ref (assumeDotNetFramework, useSdk, flags) 

[<Test>]
let ``sdk dir with dodgy global.json gives error``() =
    let tempFile = Path.GetTempFileName() + ".fsx"
    let tempPath = Path.GetDirectoryName(tempFile)
    let globalJsonPath = Path.Combine(tempPath, "global.json")
    File.WriteAllText(Path.Combine(tempPath, "global.json"), """{ "sdk": { "version": "666.666.666" } }""")
    let (options, errors) =
        checker.GetProjectOptionsFromScript(tempFile, SourceText.ofString scriptSource, assumeDotNetFramework = false, useSdkRefs = true, otherFlags = [| |])
        |> Async.RunSynchronously
    File.Delete(globalJsonPath)
    match errors with
    | [] -> failwith "Expected error while parsing script" 
    | errors -> 
       for error in errors do 
           // {C:\Users\Administrator\AppData\Local\Temp\tmp6F0F.tmp.fsx (0,1)-(0,1) The .NET SDK for this script could not be determined. If the script is in a directory using a 'global.json' ensure the relevant .NET SDK is installed. The output from 'C:\Program Files\dotnet\dotnet.exe --version' in the script directory was: '        2.1.300 [C:\Program Files\dotnet\sdk]
           Assert.AreEqual(3384, error.ErrorNumber)
           Assert.AreEqual(tempFile, error.FileName) 
