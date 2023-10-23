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
        
        let g () = f[1]
        """
        |> FSharp
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Information 3365
              Range = { StartLine = 8
                        StartColumn = 20
                        EndLine = 8
                        EndColumn = 24 }
              Message =
               "The syntax 'expr1[expr2]' is used for indexing. Consider adding a type annotation to enable indexing, or if calling a function add a space, e.g. 'expr1 [expr2]'." }
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
            c.MyFunc[42]
        """
        |> FSharp
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Information 3365
              Range = { StartLine = 11
                        StartColumn = 13
                        EndLine = 11
                        EndColumn = 25 }
              Message =
               "The syntax 'expr1[expr2]' is used for indexing. Consider adding a type annotation to enable indexing, or if calling a function add a space, e.g. 'expr1 [expr2]'." }
        ]
