namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test
open FSharp.Compiler.Diagnostics

[<TestFixture>]
module CustomCollectionTests =
    [<Test>]
    let ``Custom collection with Item and GetReverseIndex should support reverse index mutation``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "--langversion:preview" |],
            """
open System
type foo() = 
    let mutable i = ""
    member this.GetReverseIndex(_x: int, y: string) = y + " "
    member _.Item with get (_x: string) = i and set idx value = i <- idx + value

let a = foo()
a[^"2"] <- "-1"

if a["2"] <> "2 -1" then failwithf "expected 2 -1 but got %A" a["2"]
             """)

    [<Test>]
    let ``Custom collection with GetSlice and GetReverseIndex should support reverse index set slicing``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "--langversion:preview" |],
            """
open System
type foo() = 
    let mutable i = ""
    member this.GetReverseIndex(_x: int, y: string) = y + " "
    member this.SetSlice(x1: string option, x2: string option, source: string) = i <- x1.Value + x2.Value + source
    member this.GetSlice(_: string option, _: string option) = i

let a = foo()
a[^"2"..^"1"] <- "-1"

if a["2".."1"] <> "2 1 -1" then failwithf "expected 2 1 -1 but got %A" a["2".."1"]           
            """)
 
    [<Test>]
    let ``Custom collection with Item and GetReverseIndex should support reverse index indexing``() =
        CompilerAssert.CompileExeAndRunWithOptions([| "--langversion:preview" |],
            """
open System

type foo() = 
    member this.GetReverseIndex(x: int, y: int) = 10 + x + y
    member this.Item(x: int) = x

let a = foo()

if a[^2] <> 12 then failwith "expected 12"
            """)

    [<Test>]
    let ``Custom collection with Item and GetReverseIndex should support n-rank reverse index mutation``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "--langversion:preview" |],
            """
open System

type foo() = 
    let mutable i = ""
    member this.GetReverseIndex(x: int, y: string) = x.ToString() + " " + y
    member _.Item with get (_x: string) = i and set (idx1, idx2) value = i <- idx1 + " " + idx2 + " " + value

let a = foo()
a[^"1",^"2"] <- "3"

if a[""] <> "0 1 1 2 3" then failwithf "expected 0 1 1 2 3 but got %A" a[""]
            """)

    [<Test>]
    let ``Custom collection with Item and GetReverseIndex should support n-rank reverse index indexing``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "--langversion:preview" |],
            """
open System

type foo() = 
    member this.GetReverseIndex(x: int, y: int) = 10 + x + y
    member this.Item(x: int, y:int) = x + y

let a = foo()

if a[^2,^1] <> 24 then failwithf "expected 23 but got %A" a[^2,^1]
            """)

    [<Test>]
    let ``Custom collection with Item and no GetReverseIndex should not support reverse index indexing``() =
        CompilerAssert.TypeCheckSingleErrorWithOptions
            [| "--langversion:preview" |]
            """
open System

type foo() = 
    member this.Item(x: int) = x

let a = foo()

if a.[^2] <> 12 then failwith "expected 12"
            """
            FSharpDiagnosticSeverity.Error
            39
            (9,7,9,9)
            "The type 'foo' does not define the field, constructor or member 'GetReverseIndex'."

    [<Test>]
    let ``Custom collection with GetSlice and GetReverseIndex should support reverse index slicing``() =
        CompilerAssert.CompileExeAndRunWithOptions(
            [| "--langversion:preview" |],
            """
open System

type foo() = 
    member this.GetSlice(x: int option, y: int option) = 
        match x, y with
        | Some(a), Some(b) -> a + b
        | _ -> failwith "not expected"

    member this.GetReverseIndex(x: int, y: int) = 10 + x + y

let a = foo()

if a[^2..1] <> 13 then failwith "expected 13"
            """)
 
    [<Test>]
    let ``Custom collection without GetReverseIndex should not support reverse index slicing``() =
        CompilerAssert.TypeCheckSingleErrorWithOptions [| "--langversion:preview" |]
            """
open System

type foo() = 
    member this.GetSlice(x: int option, y: int option) = 
        match x, y with
        | Some(a), Some(b) -> a + b
        | _ -> failwith "not expected"

let a = foo()

if a[^2..1] <> 13 then failwith "expected 13"
            """
            FSharpDiagnosticSeverity.Error
            39
            (12,6,12,8)
            "The type 'foo' does not define the field, constructor or member 'GetReverseIndex'."
