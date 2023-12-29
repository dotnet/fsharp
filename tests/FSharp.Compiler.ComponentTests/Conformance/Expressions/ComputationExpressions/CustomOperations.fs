namespace Conformance.Expressions.ComputationExpressions

open Xunit
open FSharp.Test.Compiler

module CustomOperations =

// it becomes increasingly difficult to use packaged fslib in tests.
#if !FSHARPCORE_USE_PACKAGE
    [<Fact>]
#endif
    let ``[<CustomOperation>] without explicit name is allowed, uses method name as operation name`` () =
        FSharp """
            module CustomOperationTest
            type CBuilder() =
                [<CustomOperation>]
                member this.Foo _ = "Foo"
                [<CustomOperation>]
                member this.foo _ = "foo"
                member this.Yield _ = ()
                member this.Zero _ = ()


            [<EntryPoint>]
            let main _ =
                let cb = CBuilder()

                let x = cb { Foo }
                let y = cb { foo }
                printfn $"{x}"
                printfn $"{y}"

                if x <> "Foo" then
                    failwith "not Foo"
                if y <> "foo" then
                    failwith "not foo"
                0
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed