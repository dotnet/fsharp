// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Conformance.BasicGrammarElements.AutoPropsWithModifierBeforeGetSet

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let verifyCompile compilation =
    compilation
    |> asExe
    |> withOptions ["--nowarn:988"]
    |> compile

let verifyCompileAndRun compilation =
    compilation
    |> asExe
    |> withOptions ["--nowarn:988"]
    |> compileAndRun

[<Fact>]
let ``Instance Properties Test`` () =
    Fs """type InstancePropertiesTest() =
    member val B1: int = 0 with get
    // should failed
    member val B2: int = 0 with set
    member val B3: int = 0 with get, set
    member val B4: int = 0 with internal get
    // should failed
    member val B5: int = 0 with internal set
    member val B6: int = 0 with internal get, internal set
    member val B7: int = 0 with internal get, set
    member val B8: int = 0 with get, internal set
    // should failed
    member val internal B11: int = 0 with internal get, set
    // should failed
    member val internal B12: int = 0 with internal get
    // should failed
    member val internal B13: int = 0 with internal set
    member val internal B14: int = 0 with get, set
    member val internal B15: int = 0 with get
    // should failed
    member val internal B16: int = 0 with set""" 
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 3135, Line 4, Col 33, Line 4, Col 36, "To indicate that this property can be set, use 'member val PropertyName = expr with get,set'.")
        (Error 3135, Line 8, Col 42, Line 8, Col 45, "To indicate that this property can be set, use 'member val PropertyName = expr with get,set'.")
        (Error 0558, Line 13, Col 43, Line 13, Col 51, "When the visibility for a property is specified, setting the visibility of the set or get method is not allowed.")
        (Error 0558, Line 15, Col 43, Line 15, Col 51, "When the visibility for a property is specified, setting the visibility of the set or get method is not allowed.")
        (Error 3135, Line 17, Col 52, Line 17, Col 55, "To indicate that this property can be set, use 'member val PropertyName = expr with get,set'.")
        (Error 0558, Line 17, Col 43, Line 17, Col 51, "When the visibility for a property is specified, setting the visibility of the set or get method is not allowed.")
        (Error 3135, Line 21, Col 43, Line 21, Col 46, "To indicate that this property can be set, use 'member val PropertyName = expr with get,set'.")
    ]

[<Fact>]
let ``Static Properties Test`` () =
    Fs """type StaticPropertiesTest() =
    static member val B1: int = 0 with get
    // should failed
    static member val B2: int = 0 with set
    static member val B3: int = 0 with get, set
    static member val B4: int = 0 with internal get
    // should failed
    static member val B5: int = 0 with internal set
    static member val B6: int = 0 with internal get, internal set
    static member val B7: int = 0 with internal get, set
    static member val B8: int = 0 with get, internal set
    // should failed
    static member val internal B11: int = 0 with internal get, set
    // should failed
    static member val internal B12: int = 0 with internal get
    // should failed
    static member val internal B13: int = 0 with internal set
    static member val internal B14: int = 0 with get, set
    static member val internal B15: int = 0 with get
    // should failed
    static member val internal B16: int = 0 with set""" 
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 3135, Line 4, Col 40, Line 4, Col 43, "To indicate that this property can be set, use 'member val PropertyName = expr with get,set'.")
        (Error 3135, Line 8, Col 49, Line 8, Col 52, "To indicate that this property can be set, use 'member val PropertyName = expr with get,set'.")
        (Error 0558, Line 13, Col 50, Line 13, Col 58, "When the visibility for a property is specified, setting the visibility of the set or get method is not allowed.")
        (Error 0558, Line 15, Col 50, Line 15, Col 58, "When the visibility for a property is specified, setting the visibility of the set or get method is not allowed.")
        (Error 3135, Line 17, Col 59, Line 17, Col 62, "To indicate that this property can be set, use 'member val PropertyName = expr with get,set'.")
        (Error 0558, Line 17, Col 50, Line 17, Col 58, "When the visibility for a property is specified, setting the visibility of the set or get method is not allowed.")
        (Error 3135, Line 21, Col 50, Line 21, Col 53, "To indicate that this property can be set, use 'member val PropertyName = expr with get,set'.")
    ]

[<Fact>]
let ``Abstract Properties Test: access modifiers are not allowed`` () =
    Fs """type ``Abstract Properties Test`` =
    abstract member B1: int with get, set
    abstract member B2: int with get
    abstract member B3: int with set
    // should failed
    abstract member B4: int with internal get, set
    // should failed
    abstract member B5: int with get, internal set
    // should failed
    abstract member B6: int with internal get, internal set
    // should failed
    abstract member B7: int with internal get
    // should failed
    abstract member B8: int with internal set""" 
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 0561, Line 6, Col 5, Line 6, Col 51, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        (Error 0561, Line 8, Col 5, Line 8, Col 51, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        (Error 0561, Line 10, Col 5, Line 10, Col 60, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        (Error 0561, Line 12, Col 5, Line 12, Col 46, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        (Error 0561, Line 14, Col 5, Line 14, Col 46, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
    ]

[<Fact>]
let ``Signature File Test: no access modifiers before getter and setter`` () =
    Fsi """module Program

type A =
    new: unit -> A
    member internal B: int
    member internal C: int with get, set    
    // will warning
    member D: int with internal get, private set
    abstract E: int with get, set
    // will warning
    abstract F: int with get, private set""" 
    |> withLangVersionPreview
    |> verifyCompile
    |> shouldFail
    |> withDiagnostics [
        (Warning 3866, Line 8, Col 24, Line 8, Col 32, "The modifier will be ignored because accessible modifiers before getters and setters are not allowed in signature file.")
        (Warning 3866, Line 8, Col 38, Line 8, Col 45, "The modifier will be ignored because accessible modifiers before getters and setters are not allowed in signature file.")
        (Warning 3866, Line 11, Col 31, Line 11, Col 38, "The modifier will be ignored because accessible modifiers before getters and setters are not allowed in signature file.")
        (Error 240, Line 1, Col 1, Line 11, Col 42, "The signature file 'Program' does not have a corresponding implementation file. If an implementation file exists then check the 'module' and 'namespace' declarations in the signature and implementation files match.")
    ]

[<Fact>]
let ``Signature And Implement File Test`` () =
    let encodeFs =
        FsSource """module Program

type A() =
    member val B: int = 0 with internal get, internal set
    member val C: int = 0 with internal get, internal set
    member val D: int = 0 with internal get, private set"""
    Fsi """module Program

type A =
    new: unit -> A
    member internal B: int
    member internal C: int with get, set    
    // will warning
    member D: int with internal get, private set
    abstract E: int with get, set""" 
    |> withAdditionalSourceFile encodeFs
    |> withLangVersionPreview
    |> compile
    |> shouldFail
    |> withDiagnostics [
        (Warning 3866, Line 8, Col 24, Line 8, Col 32, "The modifier will be ignored because accessible modifiers before getters and setters are not allowed in signature file.")
        (Warning 3866, Line 8, Col 38, Line 8, Col 45, "The modifier will be ignored because accessible modifiers before getters and setters are not allowed in signature file.")
        (Error 0034, Line 6, Col 16, Line 6, Col 17, "Module 'Program' contains
    member private A.D: int with set
    but its signature specifies
    member A.D: int with set
    The accessibility specified in the signature is more than that specified in the implementation")
        (Error 0034, Line 6, Col 16, Line 6, Col 17, "Module 'Program' contains
    member internal A.D: int
    but its signature specifies
    member A.D: int
    The accessibility specified in the signature is more than that specified in the implementation")
    ]
