namespace Language.FixedBindings

open Xunit
open FSharp.Test.Compiler

module Legacy =
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked string`` langVersion =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (str: string) =
    use ptr = fixed str
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked array`` langVersion =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (arr: char[]) =
    use ptr = fixed arr
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin address of array element`` langVersion =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (arr: char[]) =
    use ptr = fixed &arr[1]
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin address of record field`` langVersion =
        Fsx """
open Microsoft.FSharp.NativeInterop

type Point = { mutable X: int; mutable Y: int }

let pinIt (thing: Point) =
    use ptr = fixed &thing.X
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 7, Col 9, Line 7, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 8, Col 5, Line 8, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin address of explicit field on this`` langVersion =
        Fsx """
open Microsoft.FSharp.NativeInterop

type Point =
    val mutable X: int
    val mutable Y: int
    
    new(x: int, y: int) = { X = x; Y = y }
    
    member this.PinIt() =
        use ptr = fixed &this.X
        NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 11, Col 13, Line 11, Col 16, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 12, Col 9, Line 12, Col 22, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]

    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked object - illegal`` langVersion =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: obj) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]
        

    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin naked int - illegal`` langVersion =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: int) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin generic - illegal`` langVersion =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: 'a) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]
        
    [<Theory>]
    [<InlineData("7.0")>]
    [<InlineData("preview")>]
    let ``Pin generic with unmanaged - illegal`` langVersion =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt<'a when 'a : unmanaged> (thing: 'a) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
        ]

// FS-1081 - Extend fixed bindings
module ExtendedFixedBindings =
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin int byref parmeter`` (langVersion, featureShouldActivate) =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: byref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3350, Line 5, Col 9, Line 5, Col 12, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")
                    ])
        
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin int inref parmeter`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: inref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3350, Line 5, Col 9, Line 5, Col 12, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")
                    ])
        
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin int outref parmeter`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: outref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3350, Line 5, Col 9, Line 5, Col 12, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")
                    ])
        
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin address of explicit field on this with default constructor class syntax`` (langVersion, featureShouldActivate) =
        // I think F# 7 and lower should have allowed this and that this was really just a bug, but we should preserve the existing behavior
        // when turning the feature off
        Fsx """
open Microsoft.FSharp.NativeInterop

type Point() =
    let mutable value = 42
    
    member this.PinIt() =
        let ptr = fixed &value
        NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 8, Col 13, Line 8, Col 16, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 9, Col 9, Line 9, Col 22, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 8, Col 13, Line 8, Col 16, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3350, Line 8, Col 13, Line 8, Col 16, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")
                    ])
        
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin int byref local variable`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt () =
    let mutable thing = 42
    use ptr = fixed &thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 7, Col 5, Line 7, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3350, Line 6, Col 9, Line 6, Col 12, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")
                    ])

#if NETCOREAPP
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin Span`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open System
open Microsoft.FSharp.NativeInterop

let pinIt (thing: Span<char>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 7, Col 5, Line 7, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""");
                        (Error 3350, Line 6, Col 9, Line 6, Col 12, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")
                    ])
#endif
    
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin custom struct byref type without GetPinnableReference method - illegal`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open System
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop

[<Struct; IsByRefLike>]
type BoringRefField<'T> = { Value: 'T }

let pinIt (thing: BoringRefField<char>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 10, Col 9, Line 10, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 10, Col 9, Line 10, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 10, Col 9, Line 10, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 10, Col 9, Line 10, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])

    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin type with method GetPinnableReference : unit -> byref<T>`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

type RefField<'T>(_value) =
    let mutable _value = _value 
    member this.GetPinnableReference () : byref<'T> = &_value

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 10, Col 5, Line 10, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3350, Line 9, Col 9, Line 9, Col 12, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")])
        
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin type with method GetPinnableReference : unit -> inref<T>`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

type RefField<'T>(_value) =
    let mutable _value = _value 
    member this.GetPinnableReference () : inref<'T> = &_value

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 10, Col 5, Line 10, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3350, Line 9, Col 9, Line 9, Col 12, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")
                    ])
        
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin type with extension method GetPinnableReference : unit -> byref<T>`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop

type RefField<'T> = { mutable _value: 'T }

[<Extension>]
type RefFieldExtensions =
    [<Extension>]
    static member GetPinnableReference(refField: RefField<'T>) : byref<'T> = &refField._value 

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 13, Col 9, Line 13, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 14, Col 5, Line 14, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 13, Col 9, Line 13, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3350, Line 13, Col 9, Line 13, Col 12, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")
                    ])
        
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin type with method GetPinnableReference with parameters - illegal`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

type StrangeType<'T>(_value) =
    let mutable _value = _value
    member this.GetPinnableReference(someValue: string) : byref<'T> = &_value

let pinIt (thing: StrangeType<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    """
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])

    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin type with method GetPinnableReference with non-byref return type - illegal`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

type StrangeType<'T>(_value) =
    let mutable _value = _value
    member this.GetPinnableReference() : 'T = _value

let pinIt (thing: StrangeType<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])

    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin type with a valid GetPinnableReference method and several invalid overloads`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

type RefField<'T>(_value) =
    let mutable _value = _value 
    member this.GetPinnableReference (x: int) : string = string x
    member this.GetPinnableReference (x: int, y: string) = string x + y
    member this.GetPinnableReference () : byref<'T> = &_value

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldSucceed
                    |> withDiagnostics [
                        (Warning 9, Line 11, Col 9, Line 11, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Warning 9, Line 12, Col 5, Line 12, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 11, Col 9, Line 11, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3350, Line 11, Col 9, Line 11, Col 12, """Feature 'extended fixed bindings for byrefs, inrefs, and GetPinnableReference' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.""")]
                )
        
    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin type with private method GetPinnableReference - illegal`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

type StrangeType<'T>(_value) =
    let mutable _value = _value
    member private this.GetPinnableReference() : byref<'T> = _value

let pinIt (thing: StrangeType<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])

    [<Theory>]
    [<InlineData("7.0", false)>]
    [<InlineData("preview", true)>]
    let ``Pin type with static method GetPinnableReference - illegal`` (langVersion, featureShouldActivate) =
        featureShouldActivate |> ignore
        Fsx """
open Microsoft.FSharp.NativeInterop

type StrangeType<'T>(_value) =
    let mutable _value = _value
    static member GetPinnableReference() : byref<'T> = Unchecked.defaultof<byref<'T>>

let pinIt (thing: StrangeType<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> withLangVersion langVersion
        |> ignoreWarnings
        |> typecheck
        |>  if featureShouldActivate then
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])
            else
                (fun comp ->
                    comp
                    |> shouldFail
                    |> withDiagnostics [
                        (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
                        (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is one of the following: an array, a string, a byref, an inref, or a type implementing GetPinnableReference()""")
                    ])
