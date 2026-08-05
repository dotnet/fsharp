module FSharp.Compiler.Service.Tests.ParameterInfoByrefSpansTests

open Xunit

[<Fact(Skip = "FSharp1.0:2394")>]
let ``Single.DotNet.ParameterByReference`` () =
    assertParameterInfoOverloads [ ["s: string"; "result: int byref"]; ["s"; "style"; "provider"; "result"] ] """
let s = "1"
let _ = System.Int32.TryParse(s,{caret}"""

[<Fact>]
let ``Single.Locations.OperatorTrick3`` () =
    assertHasParameterInfo """
open System.Threading
let mutable n = null
let aaa = Interlocked.Excha{caret}nge<obj>(&n, new obj())"""

let multiGenericExchangeCases: obj[] seq =
    [
        [| box [ "byref<int>"; "int" ]; box "System.Threading.Interlocked.Excha{caret}nge<int>(123," |]
        [| box [ "byref<float>"; "float" ]; box "System.Threading.Interlocked.Excha{caret}nge(12.0," |]
        [| box [ "byref<obj>"; "obj" ]; box "System.Threading.Interlocked.Excha{caret}nge<_> (obj," |]
    ]

[<Theory; MemberData(nameof multiGenericExchangeCases)>]
let ``Multi.Generic.Exchange`` (expected: string list) (source: string) =
    assertParameterInfoContains expected source
