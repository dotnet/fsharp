namespace Language.FixedExpressionTests

open Xunit
open FSharp.Test.Compiler

module Legacy =
    [<Fact>]
    let ``Pin naked string`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (str: string) =
    use ptr = fixed str
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin naked array`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (arr: char[]) =
    use ptr = fixed arr
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin address of array element`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (arr: char[]) =
    use ptr = fixed &arr[1]
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin address of record field`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

type Point = { mutable X: int; mutable Y: int }

let pinIt (thing: Point) =
    use ptr = fixed &thing.X
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 7, Col 9, Line 7, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 8, Col 5, Line 8, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin address of explicit field on this`` () =
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
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 11, Col 13, Line 11, Col 16, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 12, Col 9, Line 12, Col 22, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]

    [<Fact>]
    let ``Pin naked object - illegal`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: obj) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is an array, the address of a field, the address of an array element or a string'""")
        ]
        

    [<Fact>]
    let ``Pin naked int - illegal`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: int) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 5, Col 9, Line 5, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is an array, the address of a field, the address of an array element or a string'""")
        ]

// FS-1081 - Extend fixed expressions
module ExtendedFixedExpressions =
    [<Fact>]
    let ``Pin int byref parmeter`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: byref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin int inref parmeter`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: inref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin int outref parmeter`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt (thing: outref<int>) =
    use ptr = fixed &thing
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 5, Col 9, Line 5, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 6, Col 5, Line 6, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin int byref local variable`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

let pinIt () =
    let mutable thing = 42
    use ptr = fixed &thing
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 7, Col 5, Line 7, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]

    [<Fact>]
    let ``Pin Span`` () =
        Fsx """
open System
open Microsoft.FSharp.NativeInterop

let pinIt (thing: Span<char>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 6, Col 9, Line 6, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 7, Col 5, Line 7, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    let ``Pin custom byref type without GetPinnableReference method - illegal`` () =
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
        |> ignoreWarnings
        |> typecheck
        |> shouldFail

    [<Fact>]
    let ``Pin type with method GetPinnableReference : unit -> byref<T>`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

type RefField<'T>(_value) =
    let mutable _value = _value 
    member this.GetPinnableReference () : byref<'T> = &_value

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 10, Col 5, Line 10, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin type with method GetPinnableReference : unit -> inref<T>`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

type RefField<'T>(_value) =
    let mutable _value = _value 
    member this.GetPinnableReference () : inref<'T> = &_value

let pinIt (thing: RefField<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 10, Col 5, Line 10, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin type with extension method GetPinnableReference : unit -> byref<T>`` () =
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
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 13, Col 9, Line 13, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 14, Col 5, Line 14, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
        
    [<Fact>]
    let ``Pin type with method GetPinnableReference with parameters - illegal`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

type StrangeType<'T>(_value) =
    let mutable _value = _value
    member this.GetPinnableReference(someValue: string) : byref<'T> = &_value

let pinIt (thing: StrangeType<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is an array, the address of a field, the address of an array element or a string'""")
        ]

    [<Fact>]
    let ``Pin type with method GetPinnableReference with non-byref return type - illegal`` () =
        Fsx """
open Microsoft.FSharp.NativeInterop

type StrangeType<'T>(_value) =
    let mutable _value = _value
    member this.GetPinnableReference() : 'T = _value

let pinIt (thing: StrangeType<'T>) =
    use ptr = fixed thing
    NativePtr.get ptr 0
"""
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 9, Line 9, Col 9, Line 9, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Error 3207, Line 9, Col 9, Line 9, Col 12, """Invalid use of 'fixed'. 'fixed' may only be used in a declaration of the form 'use x = fixed expr' where the expression is an array, the address of a field, the address of an array element or a string'""")
        ]

    [<Fact>]
    let ``Pin type with a valid GetPinnableReference method and several invalid overloads`` () =
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
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 9, Line 11, Col 9, Line 11, Col 12, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
            (Warning 9, Line 12, Col 5, Line 12, Col 18, """Uses of this construct may result in the generation of unverifiable .NET IL code. This warning can be disabled using '--nowarn:9' or '#nowarn "9"'.""")
        ]
