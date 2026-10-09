module FSharp.Compiler.Service.Tests.CompletionRecordsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact(Skip = "Should be enabled after fixing 279738")>]
let ``Records.DotCompletion.ConstructingRecords1`` () =
    let assertOffers (should: string list) (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames should info
        assertHasNoItemsWithNames [ "abs" ] info

    assertOffers
        [ "XX" ]
        """type OuterRec = {XX : int; YY : string}
let _ =  (* MARKER*) {X{caret}"""

    assertOffers
        [ "OuterRec" ]
        """type OuterRec = {XX : int; YY : string}
let _ = {XX = 1; (* MARKER*)O{caret}"""

    assertOffers
        [ "XX"; "YY" ]
        """type OuterRec = {XX : int; YY : string}
let _ = {XX = 1; (* MARKER*)OuterRec.{caret}"""

[<Fact>]
let ``Records.DotCompletion.ConstructingRecords2`` () =
    let check (should: string list) (shouldNot: string list) (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames should info
        assertHasNoItemsWithNames shouldNot info

    let info1 =
        Checker.getCompletionInfo
            """module Mod = 
   type Rec = {XX : int; YY : string}
let _ = (* MARKER*){X{caret}  }"""

    assertHasNoItemsWithNames [ "XX" ] info1

    check
        [ "XX"; "YY" ]
        [ "System" ]
        """module Mod = 
   type Rec = {XX : int; YY : string}
let _ = {(* MARKER*)Mod.{caret} = 1; O"""

    check
        [ "XX"; "YY" ]
        [ "System" ]
        """module Mod = 
   type Rec = {XX : int; YY : string}
let _ = {(* MARKER*)Mod.Rec.{caret} """

    check
        [ "Mod" ]
        [ "XX"; "abs" ]
        """module Mod = 
   type Rec = {XX : int; YY : string}
let _ = (* MARKER*){Mod.XX = 1; {caret} }"""

[<Fact>]
let ``Records.CopyOnUpdate`` () =
    let assertFields (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames [ "a"; "b" ] info
        assertHasNoItemsWithNames [ "abs" ] info

    assertFields
        """module SomeOtherPath =
   type r = { a: int; b : int }
let f1 x = { x with SomeOtherPath.{caret} = 3 }"""

    assertFields
        """module SomeOtherPath =
   type r = { a: int; b : int }
let f2 x = { x with SomeOtherPath.r.{caret} = 3 }"""

    assertFields
        """module SomeOtherPath =
   type r = { a: int; b : int }
let f3 (x : SomeOtherPath.r) = { x with {caret}}"""

[<Fact>]
let ``Records.CopyOnUpdate.NoFieldsCompletionBeforeWith`` () =
    let info =
        Checker.getCompletionInfo
            """type T = {AAA : int}
let r = {AAA = 5}
let b = {r {caret} with }"""

    assertHasNoItemsWithNames [ "AAA" ] info

[<Fact>]
let ``Records.Constructors1`` () =
    let assertOffers (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames [ "field1"; "field2" ] info
        assertHasNoItemsWithNames [ "abs" ] info

    assertOffers
        """type X =
   val field1: int
   val field2: string
    new() = { f{caret}}"""

    assertOffers
        """type X =
   val field1: int
   val field2: string
    new() = { field1; {caret}}"""

    assertOffers
        """type X =
   val field1: int
   val field2: string
    new() = { field1 = 5; {caret}}"""

    assertOffers
        """type X =
   val field1: int
   val field2: string
    new() = { field1 = 5; f{caret} }"""

[<Fact>]
let ``Records.Constructors2.UnderscoresInNames`` () =
    let assertOffers (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames [ "_field1"; "_field2" ] info
        assertHasNoItemsWithNames [ "abs" ] info

    assertOffers
        """type X =
   val _field1: int
   val _field2: string
    new() = { _{caret}}"""

    assertOffers
        """type X =
   val _field1: int
   val _field2: string
    new() = { _field1; {caret}}"""

[<Fact>]
let ``Records.NestedRecordPatterns`` () =
    let info = Checker.getCompletionInfo "[1..({contents = 5}).{caret}]"
    assertHasItemWithNames [ "Value"; "contents" ] info
    assertHasNoItemsWithNames [ "CompareTo" ] info

[<Fact>]
let ``Records.Separators1`` () =
    let assertOffers (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames [ "abs" ] info
        assertHasNoItemsWithNames [ "AAA"; "BBB" ] info

    assertOffers
        """type X = { AAA : int; BBB : string}
let r = {AAA = 5 {caret}; }"""

    assertOffers
        """type X = { AAA : int; BBB : string}
let r = {AAA = 5 ; }
let b = {r with AAA = 5 {caret}; }"""

[<Fact>]
let ``Records.Separators2`` () =
    let assertOffers (should: string list) (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames should info
        assertHasNoItemsWithNames [ "abs" ] info

    assertOffers
        [ "AAA"; "BBB" ]
        """type X = { AAA : int; BBB : string}
let r =
       {
          AAA = 5;
(*MARKER*)   {caret}    
       }"""

    assertOffers
        [ "AAA"; "BBB"; "CCC" ]
        """type X = { AAA : int; BBB : string; CCC : int}
let r =
       {
          AAA = 5; {caret}
          CCC = 5
       }"""

[<Fact>]
let ``Records.Separators2.OffsideRule`` () =
    let info =
        Checker.getCompletionInfo
            """type X = { AAA : int; BBB : string}
let r =
       {
          AAA = 5
(*MARKER*){caret}     
       }"""

    assertHasItemWithNames [ "AAA"; "BBB" ] info
    assertHasNoItemsWithNames [ "abs" ] info

[<Fact>]
let ``Records.Inherits`` () =
    let info =
        Checker.getCompletionInfo
            """type A = class end
type B = 
   inherit A
   val f1: int
   val f2: int
   new() = { inherit A(); {caret}}"""

    assertHasItemWithNames [ "f1"; "f2" ] info
    assertHasNoItemsWithNames [ "abs" ] info

[<Fact>]
let ``Records.Inherits.AfterInheritNewLine`` () =
    let info =
        Checker.getCompletionInfo
            """type A = class end
type B = 
   inherit A
   val f1: int
   val f2: int
   new() = { inherit A()
        (*M*){caret}
           }"""

    assertHasItemWithNames [ "f1"; "f2" ] info
    assertHasNoItemsWithNames [ "abs" ] info

[<Fact>]
let ``Records.MissingBindings`` () =
    let assertOffersR (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames [ "R" ] info
        assertHasNoItemsWithNames [ "abs" ] info

    let assertOffersFields (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames [ "AAA"; "BBB" ] info
        assertHasNoItemsWithNames [ "abs" ] info

    assertOffersR
        """type R = {AAA : int; BBB : bool}
let _ = {A = 1; _;{caret}  }"""

    assertOffersR
        """type R = {AAA : int; BBB : bool}
let _ = {A = 1; _=;{caret} }"""

    assertOffersFields
        """type R = {AAA : int; BBB : bool}
let _ = {A = 1; R.{caret} }"""

    assertOffersFields
        """type R = {AAA : int; BBB : bool}
let _ = {A = 1; _; R.{caret} }"""

[<Fact>]
let ``Records.WRONG.ErrorsInFirstBinding`` () =
    let assertNoFields (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasNoItemsWithNames [ "field1"; "field2" ] info

    assertNoFields
        """type X =
   val field1: int
   val field2: string
    new() = { field1 =; {caret}}"""

    assertNoFields
        """type X =
   val field1: int
   val field2: string
    new() = { field1 =; f{caret}}"""

[<Fact>]
let ``Records.InferByFieldsInPriorMethodArguments`` () =
    let assertOffers (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        assertHasItemWithNames [ "Left"; "Top"; "Width"; "Height" ] info

    assertOffers
        """type T() =
    new (left: float32, top: float32) = T()
    new (left: float32, top: float32, width: float32, height: float32) = T()

type Rect =
    { Left: float32
      Top: float32
      Width: float32
      Height: float32 }
let toT(original) = T(original.Left, (* MARKER*)original.{caret})"""

    assertOffers
        """type T() =
    new (left: float32, top: float32) = T()
    new (left: float32, top: float32, width: float32, height: float32) = T()

type Rect =
    { Left: float32
      Top: float32
      Width: float32
      Height: float32 }
let toT(original) = T(original.Left, original.Height, (* MARKER*)original.{caret})"""

    assertOffers
        """type T() =
    new (left: float32, top: float32) = T()
    new (left: float32, top: float32, width: float32, height: float32) = T()

type Rect =
    { Left: float32
      Top: float32
      Width: float32
      Height: float32 }
let toT(original) = T(original.Left, original.Height, original.Width, (* MARKER*)original.{caret})"""

    assertOffers
        """type T() =
    new (left: float32, top: float32) = T()
    new (left: float32, top: float32, width: float32, height: float32) = T()

type Rect =
    { Left: float32
      Top: float32
      Width: float32
      Height: float32 }
let toT(original) = T(original.Left, original.Height, (* MARKER*)original.{caret}, original.Width)"""

[<Fact>]
let ``Expression.RecordPattern`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Rec =
                    { X : int}
                    member this.Value = 42
                { X = 1 }.{caret}
                """

    assertHasItemWithNames [ "Value"; "ToString" ] info

[<Fact>]
let ``SimpleTypes.Record`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Person = { Name: string; DateOfBirth: System.DateTime }
                let typrecord = { Name = "Bill"; DateOfBirth = new System.DateTime(1962,09,02) }
                typrecord.{caret}"""

    assertHasItemWithNames [ "DateOfBirth"; "Name" ] info

[<Fact>]
let ``LongIdent.Record.AsField`` () =
    let info =
        Checker.getCompletionInfo
            """
                module MyModule =
                    type person =
                        { name: string;
                        dateOfBirth: System.DateTime; }
                module MyModule2 =
                    let x = {MyModule.{caret} = 32}"""

    assertHasItemWithNames [ "person" ] info

[<Fact>]
let ``Identifier.InRecord.WithoutDef`` () =
    let info = Checker.getCompletionInfo """type Rec =  { X.{caret} : int }"""
    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Regression1911.Expression.InMatchStatement`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Thingy = { A : bool; B : int }
                let test = match (List.head [{A = true; B = 0}; {A = false; B = 1}]).{caret}"""

    assertHasItemWithNames [ "A"; "B" ] info

[<Fact(Skip = "aspirational: FCS-portable record copy-update member completion (SomeOtherPath.a) blocked on VS completion bug — https://github.com/dotnet/fsharp/issues/65731")>]
let ``AutoComplete.Bug65731_A`` () =
    let info =
        Checker.getCompletionInfo
            """module SomeOtherPath =
    type r = { a: int; b : int }
let f1 x = { x with SomeOtherPath.{caret}a = 3 } // a"""

    assertHasItemWithNames [ "a" ] info

[<Fact(Skip = "aspirational: FCS-portable record copy-update member completion (SomeOtherPath.r.a) blocked on VS completion bug — https://github.com/dotnet/fsharp/issues/65731")>]
let ``AutoComplete.Bug65731_B`` () =
    let info =
        Checker.getCompletionInfo
            """module SomeOtherPath =
    type r = { a: int; b : int }
let f2 x = { x with SomeOtherPath.r.{caret}a = 3 } // a"""

    assertHasItemWithNames [ "a" ] info
