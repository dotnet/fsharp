
#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module FSharp.Compiler.Service.Tests.FscTests
#endif


open System
open System.Diagnostics
open System.IO

open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Service.Tests.Common

open NUnit.Framework

#if FX_RESHAPED_REFLECTION
open ReflectionAdapters
#endif

exception 
   VerificationException of (*assembly:*)string * (*errorCode:*)int * (*output:*)string
   with override e.Message = sprintf "Verification of '%s' failed with code %d, message <<<%s>>>" e.Data0 e.Data1 e.Data2

exception 
   CompilationError of (*assembly:*)string * (*errorCode:*)int * (*info:*)FSharpErrorInfo []
   with override e.Message = sprintf "Compilation of '%s' failed with code %d (%A)" e.Data0 e.Data1 e.Data2

let runningOnMono = try System.Type.GetType("Mono.Runtime") <> null with e->  false        
let pdbExtension isDll = (if runningOnMono then (if isDll then ".dll.mdb" else ".exe.mdb") else ".pdb")

type PEVerifier () =

    static let expectedExitCode = 0
    static let runsOnMono = try System.Type.GetType("Mono.Runtime") <> null with _ -> false

    let verifierInfo =
#if NETCOREAPP2_0
        None
#else           
        if runsOnMono then
            Some ("pedump", "--verify all")
        else
            let peverifyPath configuration =
                Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "artifacts", "bin", "PEVerify", configuration, "net472", "PEVerify.exe")
            let peverify =
                if File.Exists(peverifyPath "Debug") then peverifyPath "Debug"
                else peverifyPath "Release"
            Some (peverify, "/UNIQUE /IL /NOLOGO")
#endif

    static let execute (fileName : string, arguments : string) =
        // Peverify may run quite a while some assemblies are pretty big.  Make the timeout 3 minutes just in case.
        let longtime = int (TimeSpan.FromMinutes(3.0).TotalMilliseconds)
        printfn "executing '%s' with arguments %s" fileName arguments
        let psi = new ProcessStartInfo(fileName, arguments)
        psi.UseShellExecute <- false
        //psi.ErrorDialog <- false
        psi.CreateNoWindow <- true
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true

        use proc = Process.Start(psi)
        let stdOut = proc.StandardOutput.ReadToEnd()
        let stdErr = proc.StandardError.ReadToEnd()
        proc.WaitForExit(longtime)
        proc.ExitCode, stdOut, stdErr

    member __.Verify(assemblyPath : string) =
        match verifierInfo with
        | Some (verifierPath, switches) -> 
            let id,stdOut,stdErr = execute(verifierPath, sprintf "%s \"%s\"" switches assemblyPath)
            if id = expectedExitCode && String.IsNullOrWhiteSpace stdErr then ()
            else
                printfn "Verification failure, stdout: <<<%s>>>" stdOut
                printfn "Verification failure, stderr: <<<%s>>>" stdErr
                raise <| VerificationException(assemblyPath, id, stdOut + "\n" + stdErr)
        | None -> 
           printfn "Skipping verification part of test because verifier not found"
            


type DebugMode =
    | Off
    | PdbOnly
    | Full

let checker = FSharpChecker.Create()

/// Ensures the default FSharp.Core referenced by the F# compiler service (if none is 
/// provided explicitly) is available in the output directory.
let ensureDefaultFSharpCoreAvailable tmpDir  =
#if NETCOREAPP2_0
    ignore tmpDir
#else
    // FSharp.Compiler.Service references FSharp.Core 4.3.0.0 by default.  That's wrong? But the output won't verify
    // or run on a system without FSharp.Core 4.3.0.0 in the GAC or in the same directory, or with a binding redirect in place.
    // 
    // So just copy the FSharp.Core 4.3.0.0 to the tmp directory. Only need to do this on Windows.
    if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then // file references only valid on Windows 
        File.Copy(fsCoreDefaultReference(), Path.Combine(tmpDir, Path.GetFileName(fsCoreDefaultReference())), overwrite = true)
#endif

let compile isDll debugMode (assemblyName : string) (ext: string) (code : string) (dependencies : string list) (extraArgs: string list) =
    let tmp = Path.Combine(Path.GetTempPath(),"test"+string(hash (isDll,debugMode,assemblyName,code,dependencies)))
    try Directory.CreateDirectory(tmp) |> ignore with _ -> ()
    let sourceFile = Path.Combine(tmp, assemblyName + "." + ext)
    let outFile = Path.Combine(tmp, assemblyName + if isDll then ".dll" else ".exe")
    let pdbFile = Path.Combine(tmp, assemblyName + pdbExtension isDll)
    do File.WriteAllText(sourceFile, code)
    let args =
        [|
            // fsc parser skips the first argument by default;
            // perhaps this shouldn't happen in library code.
            yield "fsc.exe"

            if isDll then yield "--target:library"

            match debugMode with
            | Off -> () // might need to include some switches here
            | PdbOnly ->
                yield "--debug:pdbonly"
                if not runningOnMono then  // on Mono, the debug file name is not configurable
                    yield sprintf "--pdb:%s" pdbFile
            | Full ->
                yield "--debug:full"
                if not runningOnMono then // on Mono, the debug file name is not configurable
                    yield sprintf "--pdb:%s" pdbFile

            for d in dependencies do
                yield sprintf "-r:%s" d

            yield sprintf "--out:%s" outFile

            yield! extraArgs

            yield sourceFile

        |]

    ensureDefaultFSharpCoreAvailable tmp
        
    printfn "args: %A" args
    let errorInfo, id = checker.Compile args |> Async.RunSynchronously
    for err in errorInfo do 
       printfn "error: %A" err
    if id <> 0 then raise <| CompilationError(assemblyName, id, errorInfo)
    Assert.AreEqual (errorInfo.Length, 0)
    outFile

//sizeof<nativeint>
let compileAndVerify isDll debugMode assemblyName ext code dependencies =
    let verifier = new PEVerifier ()
    let outFile = compile isDll debugMode assemblyName ext code dependencies  []
    verifier.Verify outFile
    outFile

let compileAndVerifyAst (name : string, ast : Ast.ParsedInput list, references : string list) =
    let outDir = Path.Combine(Path.GetTempPath(),"test"+string(hash (name, references)))
    try Directory.CreateDirectory(outDir) |> ignore with _ -> ()

    let outFile = Path.Combine(outDir, name + ".dll")

    ensureDefaultFSharpCoreAvailable outDir

    let errors, id = checker.Compile(ast, name, outFile, references, executable = false) |> Async.RunSynchronously
    for err in errors do printfn "error: %A" err
    Assert.AreEqual (errors.Length, 0)
    if id <> 0 then raise <| CompilationError(name, id, errors)

    // copy local explicit references for verification
    for ref in references do 
        let name = Path.GetFileName ref
        File.Copy(ref, Path.Combine(outDir, name), overwrite = true)

    let verifier = new PEVerifier()

    verifier.Verify outFile

[<Test>]
let ``1. PEVerifier sanity check`` () =
    let verifier = new PEVerifier()

    let fscorlib = typeof<int option>.Assembly
    verifier.Verify fscorlib.Location

    let nonAssembly = Path.Combine(Directory.GetCurrentDirectory(), typeof<PEVerifier>.Assembly.GetName().Name + ".pdb")
    Assert.Throws<VerificationException>(fun () -> verifier.Verify nonAssembly |> ignore) |> ignore


[<Test>]
let ``2. Simple FSC library test`` () =
    let code = """
module Foo

    let f x = (x,x)

    type Foo = class end

    exception E of int * string

    printfn "done!" // make the code have some initialization effect
"""

    compileAndVerify true PdbOnly "Foo" "fs" code [] |> ignore

[<Test>]
let ``3. Simple FSC executable test`` () =
    let code = """
module Bar

    [<EntryPoint>]
    let main _ = printfn "Hello, World!" ; 42

"""
    let outFile = compileAndVerify false PdbOnly "Bar" "fs" code []

    use proc = Process.Start(outFile, "")
    proc.WaitForExit()
    Assert.AreEqual(proc.ExitCode, 42)



[<Test>]
let ``4. Compile from simple AST`` () =
    let code = """
module Foo

    let f x = (x,x)

    type Foo = class end

    exception E of int * string

    printfn "done!" // make the code have some initialization effect
"""
    let ast = parseSourceCode("foo", code) |> Option.toList
    compileAndVerifyAst("foo", ast, [])

[<Test>]
let ``5. Compile from AST with explicit assembly reference`` () =
    let code = """
module Bar

    open FSharp.Compiler.SourceCodeServices

    let f x = (x,x)

    type Bar = class end

    exception E of int * string

    // depends on FSharp.Compiler.Service
    // note : mono's pedump fails if this is a value; will not verify type initializer for module
    let checker () = FSharpChecker.Create()

    printfn "done!" // make the code have some initialization effect
"""
    let serviceAssembly = typeof<FSharpChecker>.Assembly.Location
    let ast = parseSourceCode("bar", code) |> Option.toList
    compileAndVerifyAst("bar", ast, [serviceAssembly])


[<Test>]
let ``Check line nos are indexed by 1`` () =
    let code = """
module Bar
    let doStuff a b =
            a + b

    let sum = doStuff "1" 2

"""    
    try
        let outFile : string = compile false PdbOnly "Bar" "fs" code [] [] 
        ()
    with
    | :? CompilationError as exn  ->
            Assert.AreEqual(6,exn.Data2.[0].StartLineAlternate)
            Assert.True(exn.Data2.[0].ToString().Contains("Bar.fs (6,27)-(6,28)"))
    | _  -> failwith "No compilation error"

[<Test>]
let ``Check cols are indexed by 1`` () =
    let code = "let x = 1 + a"

    try
        let outFile : string = compile false PdbOnly "Foo" "fs" code [] []
        ()
    with
    | :? CompilationError as exn  ->
            Assert.True(exn.Data2.[0].ToString().Contains("Foo.fs (1,13)-(1,14)"))
    | _  -> failwith "No compilation error"


[<Test>]
let ``Check compile of bad fsx`` () =
    let code = """
#load "missing.fsx"
#r "missing.dll"
    """

    try
        let outFile : string = compile false PdbOnly "Foo" "fsx" code [] []
        ()
    with
    | :? CompilationError as exn  ->
            let errorText1 = exn.Data2.[0].ToString()
            let errorText2 = exn.Data2.[1].ToString()
            printfn "errorText1 = <<<%s>>>" errorText1
            printfn "errorText2 = <<<%s>>>" errorText2
            Assert.True(errorText1.Contains("Could not load file '"))
            Assert.True(errorText1.Contains("missing.fsx"))
            //Assert.True(errorText2.Contains("Could not locate the assembly \"missing.dll\""))
    | _  -> failwith "No compilation error"


[<Test>]
let ``Check compile of good fsx with bad option`` () =
    let code = """
let x = 1
    """

    try
        let outFile : string = compile false PdbOnly "Foo" "fsx" code []  ["-r:missing.dll"]
        ()
    with
    | :? CompilationError as exn  ->
            let contains (s1:string)  s2 = 
                Assert.True(s1.Contains(s2), sprintf "Expected '%s' to contain '%s'" s1 s2)
            contains (exn.Data2.[0].ToString()) "startup (1,1)-(1,1) parameter error"
            contains (exn.Data2.[0].ToString()) "missing.dll"
    | _  -> failwith "No compilation error"


#if STRESS
// For this stress test the aim is to check if we have a memory leak

module StressTest1 = 
    open System.IO

    [<Test>]
    let ``stress test repeated in-memory compilation``() =
      for i = 1 to 500 do
        printfn "stress test iteration %d" i
        let code = """
module M

type C() = 
    member x.P = 1

let x = 3 + 4
"""

        let outFile : string = compile true PdbOnly "Foo" "fs" code [] []
        ()

#endif

(*

[<Test>]
let ``Check read of mscorlib`` () =
    let options = FSharp.Compiler.AbstractIL.ILBinaryReader.mkDefault  FSharp.Compiler.AbstractIL.IL.EcmaILGlobals
    let options = { options with optimizeForMemory=true}
    let reader = FSharp.Compiler.AbstractIL.ILBinaryReader.OpenILModuleReaderAfterReadingAllBytes "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\mscorlib.dll" options
    let greg = reader.ILModuleDef.TypeDefs.FindByName "System.Globalization.GregorianCalendar"
    for attr in greg.CustomAttrs.AsList do 
        printfn "%A" attr.Method

*)


  