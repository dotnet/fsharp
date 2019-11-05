namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module CustomCollections =
    [<Test>]
    let ``Custom collection with Item and GetReverseIndex should support reverse index indexing``() =
        CompilerAssert.CompileExeAndRun
            """
open System

type foo() = 
    member this.GetReverseIndex(x: int, y: int) = 10 + x + y
    member this.Item(x: int) = x

let a = foo()

if a.[^2] <> 12 then failwith "expected 12"
            """

    [<Test>]
    let ``Custom collection with Item and no GetReverseIndex should not support reverse index indexing``() =
        CompilerAssert.TypeCheckSingleError
            """
open System

type foo() = 
    member this.Item(x: int) = x

let a = foo()

if a.[^2] <> 12 then failwith "expected 12"
            """
            FSharpErrorSeverity.Error
            39
            (9,7,9,8)
            "The field, constructor or member 'GetReverseIndex' is not defined."


    [<Test>]
    let ``Custom collection with GetSlice and GetReverseIndex should support reverse index slicing``() =
        CompilerAssert.CompileExeAndRun
            """
open System

type foo() = 
    member this.GetSlice(x: int option, y: int option) = 
        match x, y with
        | Some(a), Some(b) -> a + b
        | _ -> failwith "not expected"

    member this.GetReverseIndex(x: int, y: int) = 10 + x + y

let a = foo()

if a.[^2..1] <> 13 then failwith "expected 13"
            """
 
    [<Test>]
    let ``Custom collection without GetReverseIndex should not support reverse index slicing``() =
        CompilerAssert.TypeCheckSingleError
            """
open System

type foo() = 
    member this.GetSlice(x: int option, y: int option) = 
        match x, y with
        | Some(a), Some(b) -> a + b
        | _ -> failwith "not expected"

let a = foo()

if a.[^2..1] <> 13 then failwith "expected 13"
            """
            FSharpErrorSeverity.Error
            39
            (12,7,12,8)
            "The field, constructor or member 'GetReverseIndex' is not defined."
