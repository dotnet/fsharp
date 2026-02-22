// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalFiltering

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

module OffsideExceptions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/Basic/OffsideExceptions)
    //<Expects status="success"></Expects>
    [<Theory; FileInlineData("InfixTokenPlusOne.fs")>]
    let InfixTokenPlusOne compilation =
        compilation
        |> getCompilation 
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("RelaxWhitespace2.fs")>]
    let RelaxWhitespace2 compilation =
        compilation
        |> getCompilation 
        |> asFsx
        |> withLangVersion80
        |> withOptions ["--nowarn:25"] // Incomplete pattern matches on this expression.
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("RelaxWhitespace2.fs")>]
    let RelaxWhitespace2_Warning25 compilation =
        compilation
        |> getCompilation 
        |> asFsx
        |> withLangVersion80
        |> typecheck
        |> verifyBaseline

    [<Fact>]
    let RelaxWhitespace2_AllowedBefore1() =
        Fsx """
module A
    let a = (
     1
)
        """
        |> withLangVersion80
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    let RelaxWhitespace2_AllowedBefore2() =
        Fsx """
module A
    let a = id {| x = 1;
  y = 1
|}
        """
        |> withLangVersion80
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    let RelaxWhitespace2_AllowedBefore3() =
        Fsx """
module A
    let a = {|
     y = 1
|}
        """
        |> withLangVersion80
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    let RelaxWhitespace2_AllowedBefore4() =
        Fsx """
module A
    let a = {| y =
               1
|}
        """
        |> withLangVersion80
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    let RelaxWhitespace2_AllowedBefore5() =
        Fsx """
            let a = (
                fun () -> seq {
                    1
                }, 2 = 3
            )
            let b : (unit -> int seq) * bool = id a
        """
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    let RelaxWhitespace2_AllowedBefore6() =
        Fsx """
            let a = (
                function () -> seq {
                         1
                }, 2 = 3
            )
            let b : (unit -> int seq) * bool = id a
        """
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    let RelaxWhitespace2_AllowedBefore7() =
        Fsx """module M = begin type K = member _.G = 3 end;; module M2 = do begin end;; module M3 = do ()"""
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    let RelaxWhitespace2_AllowedBefore8() =
        Fsx """
module M = begin
    type K = member _.G = 3
end module M2 = do ignore begin 1
end module M3 = do ()
        """
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    let RelaxWhitespace2_AllowedBefore9() =
        Fsx """
type __() =
    let a =
        fun () -> (
                    1
        ), 2
        |> printfn "%O"
        in let () = ignore<(unit -> int) * unit> a
    let a' =
        fun () -> seq {
                    1
        }, 2
        |> printfn "%O"
        in let () = ignore<(unit -> seq<int>) * unit> a'
    let b =
        fun () -> (
                    1
        ), 2
        |> printfn "%O"
        in do ignore<(unit -> int) * unit> b
    let b' =
        fun () -> seq {
                    1
        }, 2
        |> printfn "%O"
        in do ignore<(unit -> seq<int>) * unit> b'
    let c =
        do ignore (
                    1
        ), 2 |> printfn "%O"
        in do ignore<unit * unit> c
    let c' =
        do ignore <| seq {
                    1
        }, 2 |> printfn "%O"
        in do ignore<unit * unit> c'
    member _.d() = seq {
        1
    }; static member e() = [
        1
    ]
        """
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    let RelaxWhitespace2_AllowedBefore10() =
        Fsx """
module [<
 Experimental "a"
 >] A = type [<
         Experimental "b"
         >] B() = let [<
                   Experimental "c"
                   >] c = 1
        """
        |> withLangVersion80
        |> typecheck
        |> shouldSucceed
        |> ignore
    [<Fact>]
    // https://github.com/dotnet/fsharp/pull/11772#issuecomment-888637013
    let RelaxWhitespace2_AllowedBefore11() =
        Fsx """
let f() = ()

begin match 1 with
| 1 -> ()
| 2 ->
  f()
| _ -> printfn ":)"
end
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let RelaxWhitespace2_Neg1() =
        Fsx """
module A
    let a = {|
    y = 1
|}
        """
        |> typecheck
        |> shouldFail
        |> withResult {
            Error = Error 58
            Range = { StartLine = 4
                      StartColumn = 5
                      EndLine = 4
                      EndColumn = 6 }
            Message =
             "Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (3:5). Try indenting this further.\nTo continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7."
        } |> ignore

    [<Fact>]
    let RelaxWhitespace2_Neg3() =
        Fsx """
module A
    let a = if {| x = 1;
    y = 1
|}                            |> isNull then ()
        """
        |> typecheck
        |> shouldFail
        |> withResults [
             { Error = Error 10
               Range = { StartLine = 5
                         StartColumn = 1
                         EndLine = 5
                         EndColumn = 3 }
               Message =
                "Incomplete structured construct at or before this point in expression" };
             { Error = Error 589
               Range = { StartLine = 3
                         StartColumn = 13
                         EndLine = 3
                         EndColumn = 15 }
               Message =
                "Incomplete conditional. Expected 'if <expr> then <expr>' or 'if <expr> then <expr> else <expr>'." };
             { Error = Error 10
               Range = { StartLine = 5
                         StartColumn = 31
                         EndLine = 5
                         EndColumn = 33 }
               Message = "Unexpected infix operator in implementation file" }
        ] |> ignore



    [<Fact>]
    let RelaxWhitespace2_Neg7() =
        Fsx """
type R = { x : int
}
function
| {
    x = 1
} -> ()
        """
        |> typecheck
        |> shouldFail
        |> withResult {
            Error = Error 10
            Range = { StartLine = 7
                      StartColumn = 1
                      EndLine = 7
                      EndColumn = 2 }
            Message =
                "Incomplete structured construct at or before this point in pattern matching. Expected '->' or other token."
        } |> ignore
    [<Fact>]
    let RelaxWhitespace2_Neg8() =
        Fsx """
type R = { x : int }
function
{
    x = 1
} -> ()
        """
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 25
              Range = { StartLine = 3
                        StartColumn = 1
                        EndLine = 3
                        EndColumn = 9 }
              Message =
               "Incomplete pattern matches on this expression. For example, the value '{ x=0 }' may indicate a case not covered by the pattern(s)." };
            { Error = Warning 193
              Range = { StartLine = 3
                        StartColumn = 1
                        EndLine = 6
                        EndColumn = 8 }
              Message =
               "This expression is a function value, i.e. is missing arguments. Its type is R -> unit." }
        ] |> ignore
    [<Fact>]
    let RelaxWhitespace2_Neg9() =
        Fsx """
let _ = <@ type Foo(x : int) =
              member this.Length = x @>

raise (new Exception("exit 1"))
        """
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Error 10
              Range = { StartLine = 2
                        StartColumn = 12
                        EndLine = 2
                        EndColumn = 16 }
              Message =
               "Incomplete structured construct at or before this point in quotation literal" };
            { Error = Error 602
              Range = { StartLine = 2
                        StartColumn = 9
                        EndLine = 2
                        EndColumn = 11 }
              Message = "Unmatched '<@ @>'" };
            { Error = Error 10
              Range = { StartLine = 2
                        StartColumn = 12
                        EndLine = 2
                        EndColumn = 16 }
              Message =
               "Unexpected keyword 'type' in binding. Expected incomplete structured construct at or before this point or other token." };
            { Error = Error 3118
              Range = { StartLine = 2
                        StartColumn = 1
                        EndLine = 2
                        EndColumn = 4 }
              Message =
               "Incomplete value or function definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword." };
            { Error = Error 10
              Range = { StartLine = 3
                        StartColumn = 38
                        EndLine = 3
                        EndColumn = 40 }
              Message =
               "Unexpected end of quotation in expression. Expected incomplete structured construct at or before this point or other token." }
            { Error = Error 3567
              Range = { StartLine = 5
                        StartColumn = 30
                        EndLine = 5
                        EndColumn = 31 }
              Message = "Expecting member body" }
            { Error = Error 3113
              Range = { StartLine = 6
                        StartColumn = 9
                        EndLine = 6
                        EndColumn = 9 }
              Message = "Unexpected end of input in type definition" }
        ] |> ignore

    [<Fact>]
    let ``RelaxedWhitespaces2 - match expression regression (https://github.com/dotnet/fsharp/issues/12011)`` () =
        Fsx """
match
    match "" with
    | null -> ""
    | s -> s
    with
| "" -> ""
| _ -> failwith ""
        """
        |> withLangVersion80
        |> withOptions ["--nowarn:20"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``RelaxedWhitespaces2 - try expression regression (https://github.com/dotnet/fsharp/issues/12011)`` () =
        Fsx """
try
    match true with
    | true -> "true"
    | false -> "false"
    with
    | ex -> ex.Message
        """
        |> withLangVersion80
        |> withOptions ["--nowarn:20"]
        |> typecheck
        |> shouldSucceed
        |> ignore