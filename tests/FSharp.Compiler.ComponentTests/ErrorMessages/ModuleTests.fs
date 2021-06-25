// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Utilities.Compiler


module Modules =

    [<Fact>]
    let ``Public Module Abbreviation``() =
        FSharp "module public L1 = List"
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 536, Line 1, Col 1, Line 1, Col 7,
                                 "The 'Public' accessibility attribute is not allowed on module abbreviation. Module abbreviations are always private.")

    [<Fact>]
    let ``Private Module Abbreviation``() =
        FSharp "module private L1 = List"
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 536, Line 1, Col 1, Line 1, Col 7,
                                 "The 'Private' accessibility attribute is not allowed on module abbreviation. Module abbreviations are always private.")
                                 
    [<Fact>]
    let ``Internal Module Abbreviation``() =
        FSharp "module internal L1 = List"
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 536, Line 1, Col 1, Line 1, Col 7,
                                 "The 'Internal' accessibility attribute is not allowed on module abbreviation. Module abbreviations are always private.")

    [<Fact>]
    let ``Attribute to the left``() =
        FSharp """[<Experimental "Hello">] module L1 = List"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 535, Line 1, Col 1, Line 1, Col 32,
                                 "Ignoring attributes on module abbreviation")
                                 
    [<Fact>]
    let ``Attribute to the right``() =
        FSharp """module [<Experimental "Hello">] L1 = List"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 535, Line 1, Col 1, Line 1, Col 35,
                                 "Ignoring attributes on module abbreviation")
                                 
    [<Fact>]
    let ``Attribute to the right without preview (typecheck)``() =
        FSharp """module [<Experimental "Hello">] L1 = List"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 535, Line 1, Col 1, Line 1, Col 35,
                                 "Ignoring attributes on module abbreviation")
    [<Fact>]
    let ``Attribute to the right without preview (compile)``() =
        FSharp """module [<Experimental "Hello">] L1 = List"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            Error 3350, Line 1, Col 8, Line 1, Col 35, "Feature 'attributes to the right of the 'module' keyword' is not available in F# 5.0. Please use language version 'preview' or greater."
            Error 535, Line 1, Col 1, Line 1, Col 35, "Ignoring attributes on module abbreviation"
            Error 222, Line 1, Col 1, Line 1, Col 42, "Files in libraries or multiple-file applications must begin with a namespace or module declaration, e.g. 'namespace SomeNamespace.SubNamespace' or 'module SomeNamespace.SomeModule'. Only the last source file of an application may omit such a declaration."
        ]
                                 
    [<Fact>]
    let ``Attribute on both sides``() =
        FSharp """[<System.Obsolete "Hi">] module [<Experimental "Hello">] internal L1 = List"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 535, Line 1, Col 1, Line 1, Col 32,
                                 "Ignoring attributes on module abbreviation")
                                 
    [<Fact>]
    let ``Attributes applied successfully``() =
        Fsx """
[<System.Obsolete "Hi">] module [<Experimental "Hello">] rec L1 = type L2() = do ()
match typeof<L1.L2>.DeclaringType.GetCustomAttrubtes false with
| [|:? CompilationMappingAttribute as compilationMapping; :? System.ObsoleteAttribute as obsolete; :? ExperimentalAttribute as experimental|] ->
    if compilationMapping.SourceConstructFlags <> SourceConstructFlags.Module then failwithf "CompilationMapping attribute did not contain the correct SourceConstructFlags: %A" compilationMapping.SourceConstructFlags
    if obsolete.Message <> "Hi" then failwithf "Obsolete attribute did not contain the correct message: %s" obsolete.Message
    if experimental.Message <> "Hello" then failwithf "Experimental attribute did not contain the correct message: %s" experimental.Message
| t -> failwithf "Attribute list is not of length 3 and correct types: %A" t
         """
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed
    [<Fact>]
    let ``Attributes applied successfully 2``() =
        Fsx """
open System
open System.ComponentModel
[<Obsolete "Hi"; NonSerialized>] [<Bindable true; AmbientValue false>] module [<Experimental "Hello"; Category "Oi">][<DefaultValue 'H'; Description "Howdy">] private rec L1 = type L2() = do ()
match typeof<L1.L2>.DeclaringType.GetCustomAttrubtes false with
| [|:? CompilationMappingAttribute as compilationMapping
    :? ObsoleteAttribute as obsolete
    :? NonSerializedAttribute
    :? BindableAttribute as bindable
    :? AmbientValueAttribute as ambientValue
    :? ExperimentalAttribute as experimental
    :? CategoryAttribute as category
    :? DefaultValueAttribute as defaultValue
    :? DescriptionAttribute as description|] ->
    if compilationMapping.SourceConstructFlags <> SourceConstructFlags.Module then failwithf "CompilationMapping attribute did not contain the correct SourceConstructFlags: %O" compilationMapping.SourceConstructFlags
    if obsolete.Message <> "Hi" then failwithf "Obsolete attribute did not contain the correct message: %s" obsolete.Message
    if bindable.Bindable <> true then failwithf "Bindable attribute did not contain the correct flag: %b" bindable.Bindable
    if ambientValue.Value <> box false then failwithf "AmbientValue attribute did not contain the correct flag: %O" ambientValue.Value
    if experimental.Message <> "Hello" then failwithf "Experimental attribute did not contain the correct message: %s" experimental.Message
    if category.Category <> "Oi" then failwithf "Category attribute did not contain the correct category: %s" category.Category
    if defaultValue.Value <> box 'H' then failwithf "DefaultValue attribute did not contain the correct value: %O" defaultValue.Value
    if description.Description <> "Howdy" then failwithf "Description attribute did not contain the correct description: %s" description.Description
| t -> failwithf "Attribute list is not of length 3 and correct types: %A" t
         """
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed