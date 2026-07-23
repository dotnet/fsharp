module FSharp.Compiler.Service.Tests.ParameterInfoNamespacesTests

open Xunit

[<Fact>]
let ``Single.Locations.WithNamespace`` () =
    assertHasParameterInfo "let a = System.Threading.Interlocked.Exchange({caret}"

[<Fact>]
let ``ParameterInfo.Locations.WithoutNamespace`` () =
    assertHasParameterInfo "open System.Threading\nlet a = Interlocked.Exchange({caret}"
