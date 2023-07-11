// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module NameResolutionTests =

    [<Fact>]
    let FieldNotInRecord () =
        FSharp """
type A = { Hello:string; World:string }
type B = { Size:int; Height:int }
type C = { Wheels:int }
type D = { Size:int; Height:int; Walls:int }
type E = { Unknown:string }
type F = { Wallis:int; Size:int; Height:int; }

let r:F = { Size=3; Height=4; Wall=1 }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1129, Line 9, Col 31, Line 9, Col 35, "The record type 'F' does not contain a label 'Wall'. Maybe you want one of the following:" + System.Environment.NewLine + "   Wallis")
            (Error 764, Line 9, Col 11, Line 9, Col 39, "No assignment given for field 'Wallis' of type 'Test.F'")
        ]

    [<Fact>]
    let RecordFieldProposal () =
        FSharp """
type A = { Hello:string; World:string }
type B = { Size:int; Height:int }
type C = { Wheels:int }
type D = { Size:int; Height:int; Walls:int }
type E = { Unknown:string }
type F = { Wallis:int; Size:int; Height:int; }

let r = { Size=3; Height=4; Wall=1 }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 9, Col 29, Line 9, Col 33, "The record label 'Wall' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   Walls" + System.Environment.NewLine + "   Wallis")
            (Error 764, Line 9, Col 9, Line 9, Col 37, "No assignment given for field 'Wallis' of type 'Test.F'")
        ]

    let multipleRecdTypeChoiceWarningWith1AlternativeSource = """
namespace N

module Module1 =

    type OtherThing = 
        { Name: string }

module Module2 =

    type Person = 
        { Name: string
          City: string }

module Lib =

    open Module2
    open Module1

    let F thing = 
        let x = thing.Name
        thing.City
"""

    [<Fact>]
    let MultipleRecdTypeChoiceWarningWith1AlternativeLangPreview () =
        FSharp multipleRecdTypeChoiceWarningWith1AlternativeSource
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3566, Line 22, Col 9, Line 22, Col 19, "Multiple type matches were found:\n    N.Module1.OtherThing\n    N.Module2.Person\nThe type 'N.Module1.OtherThing' was used. Due to the overlapping field names\n    Name\nconsider using type annotations or change the order of open statements.")
            (Error 39, Line 22, Col 15, Line 22, Col 19, "The type 'OtherThing' does not define the field, constructor or member 'City'.")
        ]

    [<Fact>]
    let MultipleRecdTypeChoiceWarningWith1AlternativeLang7 () =
        FSharp multipleRecdTypeChoiceWarningWith1AlternativeSource
        |> withLangVersion70
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Information 3566, Line 22, Col 9, Line 22, Col 19, "Multiple type matches were found:\n    N.Module1.OtherThing\n    N.Module2.Person\nThe type 'N.Module1.OtherThing' was used. Due to the overlapping field names\n    Name\nconsider using type annotations or change the order of open statements.")
            (Error 39, Line 22, Col 15, Line 22, Col 19, "The type 'OtherThing' does not define the field, constructor or member 'City'.")
        ]

    let multipleRecdTypeChoiceWarningWith2AlternativeSource = """
namespace N

module Module1 =

    type OtherThing = 
        { Name: string
          Planet: string }

module Module2 =

    type Person = 
        { Name: string
          City: string
          Planet: string }

module Module3 =

    type Cafe = 
        { Name: string
          City: string
          Country: string
          Planet: string }

module Lib =

    open Module3
    open Module2
    open Module1

    let F thing = 
        let x = thing.Name
        thing.City
"""

    [<Fact>]
    let MultipleRecdTypeChoiceWarningWith2AlternativeLangPreview () =
        FSharp multipleRecdTypeChoiceWarningWith2AlternativeSource
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3566, Line 33, Col 9, Line 33, Col 19, "Multiple type matches were found:\n    N.Module1.OtherThing\n    N.Module2.Person\n    N.Module3.Cafe\nThe type 'N.Module1.OtherThing' was used. Due to the overlapping field names\n    Name\n    Planet\nconsider using type annotations or change the order of open statements.")
            (Error 39, Line 33, Col 15, Line 33, Col 19, "The type 'OtherThing' does not define the field, constructor or member 'City'.")
        ]

    [<Fact>]
    let MultipleRecdTypeChoiceWarningWith2AlternativeLang7 () =
        FSharp multipleRecdTypeChoiceWarningWith2AlternativeSource
        |> withLangVersion70
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Information 3566, Line 33, Col 9, Line 33, Col 19, "Multiple type matches were found:\n    N.Module1.OtherThing\n    N.Module2.Person\n    N.Module3.Cafe\nThe type 'N.Module1.OtherThing' was used. Due to the overlapping field names\n    Name\n    Planet\nconsider using type annotations or change the order of open statements.")
            (Error 39, Line 33, Col 15, Line 33, Col 19, "The type 'OtherThing' does not define the field, constructor or member 'City'.")
        ]

    let multipleRecdTypeChoiceWarningNotRaisedWithCorrectOpenStmtsOrderingSource = """
namespace N

module Module1 =

   type OtherThing = 
       { Name: string
         Planet: string }

module Module2 =

   type Person = 
       { Name: string
         City: string
         Planet: string }

module Module3 =

   type Cafe = 
       { Name: string
         City: string
         Country: string
         Planet: string }

module Lib =

   open Module3
   open Module1
   open Module2

   let F thing = 
       let x = thing.Name
       thing.City
"""

    [<Fact>]
    let MultipleRecdTypeChoiceWarningNotRaisedWithCorrectOpenStmtsOrderingLangPreview () =
        FSharp multipleRecdTypeChoiceWarningNotRaisedWithCorrectOpenStmtsOrderingSource
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let MultipleRecdTypeChoiceWarningNotRaisedWithCorrectOpenStmtsOrderingLang7 () =
        FSharp multipleRecdTypeChoiceWarningNotRaisedWithCorrectOpenStmtsOrderingSource
        |> withLangVersion70
        |> typecheck
        |> shouldSucceed

    let multipleRecdTypeChoiceWarningNotRaisedWithoutOverlapsSource = """
namespace N

module Module1 =

    type OtherThing = 
        { NameX: string
          Planet: string }

module Module2 =

    type Person = 
        { Name: string
          City: string
          Planet: string }

module Module3 =

    type Cafe = 
        { NameX: string
          City: string
          Country: string
          Planet: string }

module Lib =

    open Module3
    open Module2
    open Module1

    let F thing = 
        let x = thing.Name
        thing.City
"""
    
    [<Fact>]
    let MultipleRecdTypeChoiceWarningNotRaisedWithoutOverlapsLangPreview () =
        FSharp multipleRecdTypeChoiceWarningNotRaisedWithoutOverlapsSource
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
    
    [<Fact>]
    let MultipleRecdTypeChoiceWarningNotRaisedWithoutOverlapsLang7 () =
        FSharp multipleRecdTypeChoiceWarningNotRaisedWithoutOverlapsSource
        |> withLangVersion70
        |> typecheck
        |> shouldSucceed

    let multipleRecdTypeChoiceWarningNotRaisedWithTypeAnnotationsSource = """
        namespace N
        
        module Module1 =
        
            type OtherThing = 
                { NameX: string
                  Planet: string }
        
        module Module2 =
        
            type Person = 
                { Name: string
                  City: string
                  Planet: string }
        
        module Module3 =
        
            type Cafe = 
                { NameX: string
                  City: string
                  Country: string
                  Planet: string }
        
        module Lib =
        
            open Module3
            open Module2
            open Module1
        
            let F (thing: Person) = 
                let x = thing.Name
                thing.City
        """
    
    [<Fact>]
    let MultipleRecdTypeChoiceWarningNotRaisedWithTypeAnnotationsLangPreview () =
        FSharp multipleRecdTypeChoiceWarningNotRaisedWithTypeAnnotationsSource
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
    
    [<Fact>]
    let MultipleRecdTypeChoiceWarningNotRaisedWithTypeAnnotationsLang7 () =
        FSharp multipleRecdTypeChoiceWarningNotRaisedWithTypeAnnotationsSource
        |> withLangVersion70
        |> typecheck
        |> shouldSucceed
