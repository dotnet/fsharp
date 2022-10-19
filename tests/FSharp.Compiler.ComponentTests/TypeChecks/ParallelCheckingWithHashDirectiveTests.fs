module FSharp.Compiler.ComponentTests.TypeChecks.ParallelCheckingWithHashDirectiveTests

open Xunit
open System
open FSharp.Test
open FSharp.Test.Compiler

let hashDirective = """#paralell_compilation_group "myIndependentTests" """

let independentSourceFilesDependingOnFirstFile =
    [0..3]
    |> List.map (fun i ->
        $"""{hashDirective}
namespace Tests
module MyModule{i} =
    let myFunc{i} = 5 + Test.FirstFile.y
    type MyType{i} = Test.FirstFile.CommonType -> int
    """)  
    |> List.mapi (fun i txt ->  SourceCodeFileKind.Create($"MyTest{i}.fs",source=txt))


[<Fact>]
let ``Parallel type checking without signature files can be used with special hash directive`` () =
    Fs """namespace Test
module FirstFile =
    let y = 14
    type CommonType = string
    printfn "Goodbye"
""" 
    |> withAdditionalSourceFiles independentSourceFilesDependingOnFirstFile
    |> withOptions [ "--test:ParallelCheckingWithSignatureFilesOn" ]
    |> asLibrary
    |> compile
    |> shouldSucceed


[<Fact>]
let ``Parallel type checking without signature files fails if files in reality depend on each other`` () =
    Fs $"""{hashDirective}
namespace Test
module FirstFile =
    let y = 14
    type CommonType = string
    printfn "Goodbye"
""" 
    |> withAdditionalSourceFiles independentSourceFilesDependingOnFirstFile
    |> withOptions [ "--test:ParallelCheckingWithSignatureFilesOn" ]
    |> asLibrary  
    |> compile
    |> shouldFail
    |> withErrorCodes [39] 
    |> withDiagnosticMessageMatches "The value, namespace, type or module 'Test' is not defined"
    |> fun compilationResult ->
        independentSourceFilesDependingOnFirstFile
        |> List.iter (fun source -> compilationResult |> withErrorCodesInSpecificFile source.GetSourceFileName [39] |> ignore)

[<Fact>]
let ``Parallel type checking w/o signature files can have final files that depend on the result of the parallelized group of files`` () =
    let finalFileSourceCode = """
module FinallCall
open Test.FirstFile
open Tests.MyModule0
open Tests.MyModule1
open Tests.MyModule2

type MyFinalType = CommonType -> MyType0 -> MyType1 -> MyType2

let mySuperValue = myFunc0 + myFunc1 + myFunc2
   
"""

    Fs """namespace Test
module FirstFile =
    let y = 14
    type CommonType = string
    printfn "Goodbye"
""" 
    |> withAdditionalSourceFiles independentSourceFilesDependingOnFirstFile
    |> withAdditionalSourceFile(SourceCodeFileKind.Create("FinalFile.fs", source = finalFileSourceCode))
    |> withOptions [ "--test:ParallelCheckingWithSignatureFilesOn" ]
    |> asLibrary
    |> compile
    |> shouldSucceed