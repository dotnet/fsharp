namespace Conformance.Expressions.ComputationExpressions

open Xunit
open FSharp.Test.Compiler

module CustomOperations =

    [<Fact>]
    let ``[<CustomOperation>] without explicit name is allowed, uses method name as operation name`` () =
        FSharp """
            module CustomOperationTest
            type CBuilder() =
                [<CustomOperation>]
                member this.Foo _ = "Foo"
                [<CustomOperation>]
                member this.foo _ = "foo"
                [<CustomOperation("")>]
                member this.bar _ = "bar"
                member this.Yield _ = ()
                member this.Zero _ = ()


            [<EntryPoint>]
            let main _ =
                let cb = CBuilder()

                let x = cb { Foo }
                let y = cb { foo }
                let z = cb { bar }
                printfn $"{x}"
                printfn $"{y}"

                if x <> "Foo" then
                    failwith "not Foo"
                if y <> "foo" then
                    failwith "not foo"
                if z <> "bar" then
                    failwith "not bar"
                0
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed