module FSharp.Compiler.Service.Tests.CompletionEnumsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``OfSeveralModuleMembers`` () =
    let completeAt (expr: string) =
        Checker.getCompletionInfo (
            sprintf
                """module Module =
    let Constant = 5
    type Class = class
       end
    type Record = {AString:string}
    exception OutOfRange of string
    type Enum = Red = 0 | White = 1 | Blue = 2
    type DiscriminatedUnion = A | B | C
    type TupleType = int * int
    type FunctionType = unit->unit
    let (~+) x = -x
    type Interface =
        abstract MyMethod : unit->unit
    type Struct = struct
        end
    let Function x = 0
    let FunctionValue = fun x -> 0
    let Tuple = (0,2)
    module Submodule =
        let a = 0
    type ValueType = int
module AbbreviationModule =
    type StructAbbreviation = Module.Struct
    type InterfaceAbbreviation = Module.Interface
    type DiscriminatedUnionAbbreviation = Module.DiscriminatedUnion
    type RecordAbbreviation = Module.Record
    type EnumAbbreviation = Module.Enum
    type TupleTypeAbbreviation = Module.TupleType
let y = %s
let f x = 0"""
                expr)

    let moduleMembers = completeAt "Module.{caret}"

    assertHasItemWithNames
        [ "Constant"; "Class"; "Record"; "OutOfRange"; "Enum"; "DiscriminatedUnion"; "TupleType"
          "FunctionType"; "Interface"; "Struct"; "Function"; "FunctionValue"; "Tuple"; "Submodule"; "ValueType" ]
        moduleMembers

    for name, glyph in
        [ "A", FSharpGlyph.EnumMember
          "B", FSharpGlyph.EnumMember
          "C", FSharpGlyph.EnumMember
          "Enum", FSharpGlyph.Enum
          "DiscriminatedUnion", FSharpGlyph.Union
          "Interface", FSharpGlyph.Interface
          "Struct", FSharpGlyph.Struct
          "ValueType", FSharpGlyph.Struct
          "Class", FSharpGlyph.Class
          "Record", FSharpGlyph.Type
          "TupleType", FSharpGlyph.Class
          "FunctionType", FSharpGlyph.Delegate
          "Submodule", FSharpGlyph.Module
          "OutOfRange", FSharpGlyph.Exception
          "Function", FSharpGlyph.Method
          "FunctionValue", FSharpGlyph.Method
          "Constant", FSharpGlyph.Variable
          "Tuple", FSharpGlyph.Variable ] do
        assertItemGlyph name glyph moduleMembers

    let abbreviationMembers = completeAt "AbbreviationModule.{caret}"

    assertHasItemWithNames
        [ "StructAbbreviation"; "InterfaceAbbreviation"; "DiscriminatedUnionAbbreviation"
          "RecordAbbreviation"; "EnumAbbreviation"; "TupleTypeAbbreviation" ]
        abbreviationMembers

    for name, glyph in
        [ "EnumAbbreviation", FSharpGlyph.Enum
          "InterfaceAbbreviation", FSharpGlyph.Interface
          "StructAbbreviation", FSharpGlyph.Struct
          "RecordAbbreviation", FSharpGlyph.Type
          "DiscriminatedUnionAbbreviation", FSharpGlyph.Union
          "TupleTypeAbbreviation", FSharpGlyph.Class ] do
        assertItemGlyph name glyph abbreviationMembers

[<Fact>]
let ``EnumValue.Bug2449`` () =
    let info =
        Checker.getCompletionInfo
            """type E = | A = 1 | B = 2
let e = E.A
e.{caret}"""

    assertHasNoItemsWithNames [ "value__" ] info

[<Fact>]
let ``EnumValue.Bug4044`` () =
    let info =
        Checker.getCompletionInfo
            """open System.IO
let GetFileSize filePath = File.GetAttributes(filePath).{caret}"""

    assertHasNoItemsWithNames [ "value__" ] info

[<Fact>]
let ``SimpleTypes.Enum`` () =
    let info =
        Checker.getCompletionInfo
            """
                type weekday =
                    | Monday = 1
                    | Tuesday = 2
                    | Wednesday = 3
                    | Thursday = 4
                    | Friday = 5
                let typeenum = weekday.Friday
                typeenum.{caret}"""

    assertHasItemWithNames [ "GetType"; "ToString" ] info

[<Theory>]
[<InlineData("""
                namespace NS
                module longident =
                    type Direction =
                        | Left         = 1
                        | Right        = 2
                    type MoveCursor() =
                        member this.Direction = Direction.Left
                namespace NS2
                module test =
                    let cursor = new NS.longident.MoveCursor()
                    match cursor.{caret} with
                    | NS.longident.Direction.Left -> "move left"
                    | NS(*Mpatternmatch2*) -> "move right" """,
             "Direction;ToString")>]
[<InlineData("""
                namespace NS
                module longident =
                    type Direction =
                        | Left         = 1
                        | Right        = 2
                    type MoveCursor() =
                        member this.Direction = Direction.Left
                namespace NS2
                module test =
                    let cursor = new NS.longident.MoveCursor()
                    match cursor(*Mpatternmatch1*) with
                    | NS.longident.Direction.Left -> "move left"
                    | NS.{caret} -> "move right" """,
             "longident")>]
let ``LongIdent.PatternMatch.DefFromDiffNamespace`` (markedSource: string) (names: string) =
    let info = Checker.getCompletionInfo markedSource

    assertHasItemWithNames (names.Split(';') |> List.ofArray) info

[<Fact>]
let ``ReOpenNameSpace.EnumTypes`` () =
    let info =
        Checker.getCompletionInfo
            """
                // F# declared enum types:
                namespace A
                module Test =
                  type A = | Foo = 0
                namespace B
                open A
                open A
                Test.A.{caret}
                """

    assertHasItemWithNames [ "Foo" ] info
