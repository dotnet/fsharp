module ErrorMessages.FSharpDiagnosticTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Test.Assert
open FSharp.Test.Compiler
open Xunit

let checkDiagnostics diagnostics (checkResults: FSharpCheckFileResults) =
    checkResults.Diagnostics
    |> Array.map (fun e ->
        {| Number = e.ErrorNumber
           Severity = e.Severity
           DefaultSeverity = e.DefaultSeverity |})
    |> shouldEqual diagnostics

[<Fact>]
let ``FSharpDiagnostic: warning default severity`` () =
    FSharp "
module Test
5
"
    |> typecheckResults
    |> checkDiagnostics [|
        {| Number = 20
           Severity = FSharpDiagnosticSeverity.Warning
           DefaultSeverity = FSharpDiagnosticSeverity.Warning |}
       |]

[<Fact>]
let ``FSharpDiagnostic: warning as error default severity`` () =
    FSharp "
module M
5
"
    |> withOptions [ "--warnaserror+" ]
    |> typecheckResults
    |> checkDiagnostics [|
        {| Number = 20
           Severity = FSharpDiagnosticSeverity.Error
           DefaultSeverity = FSharpDiagnosticSeverity.Warning |}
       |]

[<Fact>]
let ``FSharpDiagnostic: error default severity`` () =
    FSharp "
module M
x
"
    |> typecheckResults
    |> checkDiagnostics [|
        {| Number = 39
           Severity = FSharpDiagnosticSeverity.Error
           DefaultSeverity = FSharpDiagnosticSeverity.Error |}
       |]
