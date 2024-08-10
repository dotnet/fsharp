namespace EmittedIL

open Microsoft.FSharp.Core
open Xunit
open FSharp.Test.Compiler

module VeryLargeClasses =

    let classWithManyMethods n =
        let methods =
            let mutable source = ""
            for i = 0 to n - 1 do
                source <- source + $"""
                static member Method%05x{i}() = () """
            source

        FSharp
            $"""
            namespace VeryLargeClassesTestcases

            type Type1 ={methods}
                """

    [<Fact(Skip="Running for too long, causing CI timeouts")>]
    let ``More than 64K Methods -- should fail`` () =
        classWithManyMethods (1024 * 64 + 1)
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3864, Line 1, Col 1, Line 1, Col 1, "The type 'VeryLargeClassesTestcases.Type1' has too many methods. Found: '65537', maximum: '65520'")
        ]

    [<Fact(Skip="Running for too long, causing CI timeouts")>]
    let ``Exactly (0xfff0) Methods -- should succeed`` () =
        FSharp
            """
module MyMain
open System
open System.Reflection
do printfn $"location: {typeof<VeryLargeClassesTestcases.Type1>.Assembly.Location}"
let asm = Assembly.LoadFrom(typeof<VeryLargeClassesTestcases.Type1>.Assembly.Location)
printfn $"asm: {asm}"
let types = asm.GetTypes()
printfn "length: {types.Length}"
"""
        |> withReferences [ classWithManyMethods 0xfff0 |> asLibrary ]
        |> asExe
        |> compileAndRun
        |> shouldSucceed

