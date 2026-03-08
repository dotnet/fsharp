    [<Fact>]
    let ``Struct method without UnscopedRef has scoped receiver cross-assembly`` () =
        let fsharpLib =
            FSharp """
module MyLib
open System
open System.Runtime.CompilerServices
open System.Diagnostics.CodeAnalysis

[<Struct; IsByRefLike>]
type S =
    val mutable X: int
    // No [UnscopedRef] - 'this' should be scoped
    member this.AsSpan() : Span<int> = Span<int>(&this.X)
"""
            |> asLibrary
            |> withName "FSharpScopedRefCrossAsmLib"
            |> withLangVersionPreview

        FSharp """
module Test
open System
open MyLib

let test (s: byref<S>) : Span<int> =
    // s is a byref param (can escape), but AsSpan() treats 'this' as scoped.
    // So 'this' (s) cannot be captured in the return value.
    s.AsSpan()
"""
            |> asLibrary
            |> withLangVersionPreview
            |> withReferences [fsharpLib]
            |> compile
            |> shouldFail
            |> withErrorCodes [3235]
