module FSharp.Compiler.Service.Tests.ParameterInfoRecordsTests

open Xunit

[<Fact>]
let ``Single.RecordAndUnionType`` () =
    assertParameterInfoOverloads [ [ "Fruit"; "KeyValuePair" ] ] """
type Fruit = | Apple | Banana
type KeyValuePair = { Key : int; Value : float }
let print (x : Fruit, kvp : KeyValuePair) = System.Console.WriteLine(x); System.Console.WriteLine(kvp)
pri{caret}nt (Banana, {Key = 0; Value = 0.0})"""

[<Fact>]
let ``Multi.Function.WithRecordType`` () =
    assertParameterInfoOverloads [ ["int"; "Vector"] ] """
type Vector =
    { X : float; Y : float; Z : float }
let foo(x : int,v : Vector) = ()
fo{caret}o(12, { X = 10.0; Y = 20.0; Z = 30.0 })"""

[<Fact>]
let ``Multi.NoParameterInfo.OnValues`` () =
    assertNoParameterInfo """
type Foo = class
    val private size : int
    val private path : string
    new (s : int, p : string) = {size = s; path{caret} = p}
end"""
