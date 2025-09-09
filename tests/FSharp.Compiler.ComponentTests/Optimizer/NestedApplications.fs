namespace FSharp.Compiler.ComponentTests.Optimizer

open System.Text
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.Utilities

module private Gen =
    let nestedLetApps depth =
        // Builds: let v1 = id 0 in let v2 = id v1 in ... in ignore vN
        let sb = StringBuilder()
        sb.AppendLine("module M")       |> ignore
        sb.AppendLine("let id x = x")   |> ignore
        sb.AppendLine("let run () =")   |> ignore
        for i in 1 .. depth do
            if i = 1 then
                sb.Append("    let v1 = id 0") |> ignore
            else
                sb.Append(" in let v").Append(i).Append(" = id v").Append(i-1) |> ignore
        sb.AppendLine(" in ()") |> ignore
        sb.ToString()

    let nestedDirectApps depth =
        // Builds: let res = id(id(id(...(0)))) in ignore res
        let sb = StringBuilder()
        sb.AppendLine("module N")       |> ignore
        sb.AppendLine("let id x = x")   |> ignore
        sb.Append("let run () = let res = ") |> ignore
        for _ in 1 .. depth do
            sb.Append("id (") |> ignore
        sb.Append("0") |> ignore
        for _ in 1 .. depth do
            sb.Append(")")   |> ignore
        sb.AppendLine(" in ignore res") |> ignore
        sb.ToString()

[<Collection(nameof NotThreadSafeResourceCollection)>]
type ``Nested application optimizer``() =

    // Moderate depths to keep CI stable while still exercising the quadratic shapes
    [<Theory>]
    [<InlineData(100)>]
    [<InlineData(1000)>]
    let ``let-chains of nested apps compile under --optimize+`` depth =
        let src = Gen.nestedLetApps depth
        FSharp src
        |> withOptions [ "--optimize+"; "--times" ]
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory>]
    [<InlineData(100)>]
    [<InlineData(1000)>]
    let ``direct nested application compiles under --optimize+`` depth =
        let src = Gen.nestedDirectApps depth
        FSharp src
        |> withOptions [ "--optimize+"; "--times" ]
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed