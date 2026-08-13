// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Diagnostics

open Xunit
open FSharp.Test
open FSharp.Test.Assert
open FSharp.Compiler.Text

/// Checks the classification of diagnostic messages that were converted to rich text.
/// See docs/rich-diagnostics.md.
module RichDiagnosticTests =

    let private singleDiagnostic source =
        match CompilerAssert.TypeCheckWithOptions [||] source with
        | [| diagnostic |] -> diagnostic
        | diagnostics -> failwith $"Expected a single diagnostic, got:\n%A{diagnostics}"

    let private diagnostic number source =
        let diagnostics = CompilerAssert.TypeCheckWithOptions [||] source

        match diagnostics |> Array.tryFind (fun d -> d.ErrorNumber = number) with
        | Some diagnostic -> diagnostic
        | None -> failwith $"Expected a diagnostic FS%04d{number}, got:\n%A{diagnostics}"

    let private assertMessageParts expected source =
        (singleDiagnostic source).RichMessage |> assertRichTextParts expected

    let private assertMessagePartsOf number expected source =
        (diagnostic number source).RichMessage |> assertRichTextParts expected

    [<Fact>]
    let ``Undefined value name is classified`` () =
        "let _ = someUndefinedValue"
        |> assertMessageParts
            [ TextTag.Text, "The value or constructor '"
              TextTag.UnresolvedName, "someUndefinedValue"
              TextTag.Text, "' is not defined." ]

    [<Fact>]
    let ``Undefined type name is classified`` () =
        "let _: SomeUndefinedType = ()"
        |> assertMessageParts
            [ TextTag.Text, "The type '"
              TextTag.UnresolvedName, "SomeUndefinedType"
              TextTag.Text, "' is not defined." ]

    [<Fact>]
    let ``Undefined name suggestions are classified`` () =
        """
let frobnicate = 1
let _ = frobnicatf
"""
        |> assertMessageParts
            [ TextTag.Text, "The value or constructor '"
              TextTag.UnresolvedName, "frobnicatf"
              TextTag.Text, "' is not defined. Maybe you want one of the following:"
              TextTag.LineBreak, System.Environment.NewLine
              TextTag.Text, "   "
              TextTag.UnknownEntity, "frobnicate" ]

    [<Fact>]
    let ``Message of an unconverted diagnostic is a single part`` () =
        // FS0067 carries no arguments, so there is nothing in it to classify
        let diagnostic =
            diagnostic 67 "let _ = System.Collections.Generic.Dictionary<obj, obj>() :?> System.Collections.IDictionary"

        diagnostic.RichMessage.Parts.Length |> shouldEqual 1
        diagnostic.RichMessage.Text |> shouldEqual diagnostic.Message

    [<Fact>]
    let ``Type of an ignored result is classified`` () =
        "1 + 1"
        |> assertMessagePartsOf
            20
            [ TextTag.Text, "The result of this expression has type '"
              TextTag.Struct, "int"
              TextTag.Text, "' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'." ]

    [<Fact>]
    let ``Type of an unexpected function value is classified`` () =
        """
let f x = x + 1
let _: int = f
"""
        |> assertMessagePartsOf
            1
            [ TextTag.Text, "This expression was expected to have type\n    '"
              TextTag.Struct, "int"
              TextTag.Text, "'    \nbut here has type\n    '"
              TextTag.Struct, "int"
              TextTag.Space, " "
              TextTag.Punctuation, "->"
              TextTag.Space, " "
              TextTag.Struct, "int"
              TextTag.Text, "'    " ]

    [<Fact>]
    let ``Type of a sealed coercion source is classified`` () =
        "let _ = 1 :?> string"
        |> assertMessageParts
            [ TextTag.Text, "The type '"
              TextTag.Struct, "int"
              TextTag.Text, "' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion." ]

    [<Fact>]
    let ``Types of a mismatch are classified`` () =
        "let _: int = \"\""
        |> assertMessagePartsOf
            1
            [ TextTag.Text, "This expression was expected to have type\n    '"
              TextTag.Struct, "int"
              TextTag.Text, "'    \nbut here has type\n    '"
              TextTag.Alias, "string"
              TextTag.Text, "'    " ]

    [<Fact>]
    let ``Types of a mismatch in a list element are classified`` () =
        "let _ = [ 1; \"\" ]"
        |> assertMessagePartsOf
            1
            [ TextTag.Text, "All elements of a list must be implicitly convertible to the type of the first element, which here is '"
              TextTag.Struct, "int"
              TextTag.Text, "'. This element has type '"
              TextTag.Alias, "string"
              TextTag.Text, "'." ]

    [<Fact>]
    let ``Type of a missing else branch is classified`` () =
        "let _ = if true then 1"
        |> assertMessagePartsOf
            1
            [ TextTag.Text, "This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type '"
              TextTag.Struct, "int"
              TextTag.Text, "'." ]

    [<Fact>]
    let ``Types of a downcast used instead of an upcast are classified`` () =
        """
open System.Collections.Generic
let orig = Dictionary<obj,obj>()
let _ = orig :?> IDictionary<obj,obj>
"""
        |> assertMessagePartsOf
            3198
            [ TextTag.Text, "The conversion from "
              TextTag.Class, "Dictionary"
              TextTag.Punctuation, "<"
              TextTag.Alias, "obj"
              TextTag.Punctuation, ","
              TextTag.Alias, "obj"
              TextTag.Punctuation, ">"
              TextTag.Text, " to "
              TextTag.Interface, "IDictionary"
              TextTag.Punctuation, "<"
              TextTag.Alias, "obj"
              TextTag.Punctuation, ","
              TextTag.Alias, "obj"
              TextTag.Punctuation, ">"
              TextTag.Text, " is a compile-time safe upcast, not a downcast. Consider using the :> (upcast) operator instead of the :?> (downcast) operator." ]

    /// Every part of a type is classified on its own, not just the type as a whole
    [<Fact>]
    let ``Parts of a tuple type are classified`` () =
        "let _: int * int = 1, 2, 3"
        |> assertMessagePartsOf
            1
            [ TextTag.Text, "Type mismatch. Expecting a tuple of length 2 of type\n    "
              TextTag.Struct, "int"
              TextTag.Space, " "
              TextTag.Punctuation, "*"
              TextTag.Space, " "
              TextTag.Struct, "int"
              TextTag.Text, "    \nbut given a tuple of length 3 of type\n    "
              TextTag.Struct, "int"
              TextTag.Space, " "
              TextTag.Punctuation, "*"
              TextTag.Space, " "
              TextTag.Struct, "int"
              TextTag.Space, " "
              TextTag.Punctuation, "*"
              TextTag.Space, " "
              TextTag.Struct, "int"
              TextTag.Text, "    \n" ]
