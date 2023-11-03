namespace FSharp.Compiler.ComponentTests.ErrorMessages

open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

module ``Indexing Syntax`` =

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for SynExpr.Ident app`` () =
        """
namespace N

    module M =

        let f (a: int list) = a
        
        let g () = f [1] // should not warn
        
        let h () = f[1] // should warn
        """
        |> FSharp
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withResults
            [
                {
                    Error = Information 3365
                    Range =
                        {
                            StartLine = 10
                            StartColumn = 20
                            EndLine = 10
                            EndColumn = 24
                        }
                    Message =
                        "The syntax 'expr1[expr2]' is used for indexing. Consider adding a type annotation to enable indexing, or if calling a function add a space, e.g. 'expr1 [expr2]'."
                }
            ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for SynExpr.LongIdent app`` () =
        """
namespace N

    module N =

        type C () =
            member _.MyFunc (inputList: int list) = inputList

        let g () =
            let c = C()
            let _ = c.MyFunc [23]   // should not warn
            c.MyFunc[42]    // should warn
        """
        |> FSharp
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withResults
            [
                {
                    Error = Information 3365
                    Range =
                        {
                            StartLine = 12
                            StartColumn = 13
                            EndLine = 12
                            EndColumn = 25
                        }
                    Message =
                        "The syntax 'expr1[expr2]' is used for indexing. Consider adding a type annotation to enable indexing, or if calling a function add a space, e.g. 'expr1 [expr2]'."
                }
            ]
