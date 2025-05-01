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
    // should fail
    member val B2: int = 0 with set
    member val B3: int = 0 with get, set
    member val B4: int = 0 with internal get
    // should fail
    member val B5: int = 0 with internal set
    member val B6: int = 0 with internal get, internal set
    member val B7: int = 0 with internal get, set
    member val B8: int = 0 with get, internal set
    // should fail
    member val internal B11: int = 0 with internal get, set
    // should fail
    member val internal B12: int = 0 with internal get
    // should fail
    member val internal B13: int = 0 with internal set
    member val internal B14: int = 0 with get, set
    member val internal B15: int = 0 with get
    // should fail
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
    // should fail
    static member val B2: int = 0 with set
    static member val B3: int = 0 with get, set
    static member val B4: int = 0 with internal get
    // should fail
    static member val B5: int = 0 with internal set
    static member val B6: int = 0 with internal get, internal set
    static member val B7: int = 0 with internal get, set
    static member val B8: int = 0 with get, internal set
    // should fail
    static member val internal B11: int = 0 with internal get, set
    // should fail
    static member val internal B12: int = 0 with internal get
    // should fail
    static member val internal B13: int = 0 with internal set
    static member val internal B14: int = 0 with get, set
    static member val internal B15: int = 0 with get
    // should fail
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
    // should fail
    abstract member B4: int with internal get, set
    // should fail
    abstract member B5: int with get, internal set
    // should fail
    abstract member B6: int with internal get, internal set
    // should fail
    abstract member B7: int with internal get
    // should fail
    abstract member B8: int with internal set""" 
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 0561, Line 6, Col 34, Line 6, Col 42, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        (Error 0561, Line 8, Col 39, Line 8, Col 47, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        (Error 0561, Line 10, Col 34, Line 10, Col 42, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        (Error 0561, Line 10, Col 48, Line 10, Col 56, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        (Error 0561, Line 12, Col 34, Line 12, Col 42, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        (Error 0561, Line 14, Col 34, Line 14, Col 42, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
    ]

[<Fact>]
let ``Signature File Test: no access modifiers before getter and setter`` () =
    Fsi """module Program

type A =
    new: unit -> A
    member internal B: int
    member internal C: int with get, set    
    member D: int with internal get, private set
    abstract E: int with get, set
    abstract F: int with get, private set""" 
    |> withLangVersionPreview
    |> verifyCompile
    |> shouldFail
    |> withDiagnostics [
        (Error 0561, Line 9, Col 31, Line 9, Col 38, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
    ]

[<Fact>]
let ``Signature And Implement File Test`` () =
    let encodeFs =
        FsSource """module Program

type A() =
    member val B: int = 0 with internal get, internal set
    member val C: int = 0 with internal get, internal set
    member val D: int = 0 with internal get, private set
    member val E: int = 0 with internal get, private set"""
    Fsi """module Program

type A =
    new: unit -> A
    member internal B: int
    member internal C: int with get, set
    member D: int with internal get, private set
    member E: int with get, set""" 
    |> withAdditionalSourceFile encodeFs
    |> withLangVersionPreview
    |> compile
    |> shouldFail
    |> withDiagnostics [
        (Error 0034, Line 7, Col 16, Line 7, Col 17, "Module 'Program' contains
    member A.E: int with private set    
but its signature specifies
    member A.E: int with set    
The accessibility specified in the signature is more than that specified in the implementation")
        (Error 0034, Line 7, Col 16, Line 7, Col 17, "Module 'Program' contains
    member A.E: int with internal get    
but its signature specifies
    member A.E: int with get    
The accessibility specified in the signature is more than that specified in the implementation")
    ]

[<Fact>]
let ``Cannot use in F# 8.0`` () =
    let encodeFs =
        FsSource """module Program

type A() =
    member val B: int = 0 with internal get
    member _.C with internal set (v: int) = ()
    member val D: int = 0 with internal get, private set"""
    Fsi """module Program

type A =
    new: unit -> A
    member B: int with internal get
    member C: int with internal set
    member D: int with internal get, private set""" 
    |> withAdditionalSourceFile encodeFs
    |> withLangVersion80
    |> compile
    |> shouldFail
    |> withDiagnostics [
        (Error 3350, Line 5, Col 24, Line 5, Col 32, "Feature 'Allow access modifiers to auto properties getters and setters' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
        (Error 3350, Line 6, Col 24, Line 6, Col 32, "Feature 'Allow access modifiers to auto properties getters and setters' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
        (Error 3350, Line 7, Col 24, Line 7, Col 32, "Feature 'Allow access modifiers to auto properties getters and setters' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
        (Error 3350, Line 4, Col 32, Line 4, Col 40, "Feature 'Allow access modifiers to auto properties getters and setters' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
        (Error 3350, Line 6, Col 32, Line 6, Col 40, "Feature 'Allow access modifiers to auto properties getters and setters' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
    ]
