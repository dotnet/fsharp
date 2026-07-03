// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module ``UncheckedDefaultofOptimization`` =

    // https://github.com/dotnet/fsharp/issues/18128
    // Unused `Unchecked.defaultof<concreteType>` bindings should be eliminated under optimization.
    // Pins both the absence of `initobj` and that no decimal local slot is allocated for any of the
    // three discarded bindings.
    [<Fact>]
    let ``Unused Unchecked.defaultof bindings of concrete types are eliminated`` () =
        FSharp """
module Test
open System
let f (n: float32) =
    Console.WriteLine n
    let _ = Unchecked.defaultof<decimal>
    let _ = Unchecked.defaultof<decimal>
    let _ = Unchecked.defaultof<decimal>
    let n' = n * 2.f
    Console.WriteLine n'
        """
        |> withOptimize
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [ "initobj"; "valuetype [runtime]System.Decimal" ]

    // https://github.com/dotnet/fsharp/issues/18128
    // The FSharpPlus-style SRTP witness pattern from the issue. Elimination happens at the (fully
    // instantiated) use site: `doWork` reduces to a direct multiplication with the `nil<PreOps>` and
    // `nil< ^b >` witness bindings gone. The assertion is scoped to `doWork` rather than the whole
    // module because the inline functions also emit a compiler-generated dynamic-invocation stub which
    // legitimately retains an `ldnull` (see the guard below and issue #19758 - the witness bindings of
    // an *inline* body must be preserved so cross-assembly consumers can re-inline and specialize them).
    [<Fact>]
    let ``Unused Unchecked.defaultof SRTP witness bindings are eliminated at the use site`` () =
        FSharp """
module Test

open System.ComponentModel

[<AbstractClass; Sealed; EditorBrowsable(EditorBrowsableState.Never)>]
type PreOps =
    static member inline Double (n: float<'u>) : float<'u> = n * 2.
    static member inline Double (n: float32<'u>) : float32<'u> = n * 2.f

#nowarn "64"
module PreludeOperators =
    let inline private nil<'T> = Unchecked.defaultof<'T>
    let inline double (x: ^a) =
        let inline _call (_: ^M, input: ^I, _: ^R) = ((^M or ^I) : (static member Double : ^I -> ^R) input)
        _call (nil<PreOps>, x, nil< ^b >)

open PreludeOperators
let doWork (n: float) = double n
        """
        |> withOptimize
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.method public static float64  doWork(float64 n) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.r8     2.
    IL_000a:  mul
    IL_000b:  ret
  }"""
        ]

    // https://github.com/dotnet/fsharp/issues/18128
    // Soundness pin: eliminating an unused `Unchecked.defaultof<T>` must not introduce a new reference
    // to T in the enclosing method. `defaultof` of a reference type lowers to `ldnull` (not `newobj`),
    // so removing the binding cannot suppress an observable cctor call - `f`'s body must contain no
    // reference to `WithCctor` at all.
    [<Fact>]
    let ``Eliminated Unchecked.defaultof leaves no reference to T in the caller`` () =
        FSharp """
module Test

type WithCctor() =
    static do failwith "cctor must not run"

let f () =
    let _ = Unchecked.defaultof<WithCctor>
    42
        """
        |> withOptimize
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.method public static int32  f() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   42
    IL_0002:  ret
  }"""
        ]

    // https://github.com/dotnet/fsharp/issues/18128
    // Regression for SRTP witness/dummy-argument patterns (e.g. FSharpPlus) where eliminating an
    // `Unchecked.defaultof` binding whose type still references unsolved typars would trip FS0073 in
    // IlxGen. Such bindings must be kept; only fully-ground ilzero bindings are removed.
    [<Fact>]
    let ``Unused Unchecked.defaultof bindings of unsolved generic types do not cause FS0073`` () =
        FSharp """
module Test
let inline witness () = Unchecked.defaultof<'T>
let inline run< ^T when ^T: (static member Zero: ^T)> () =
    let _ = (witness () : ^T)
    LanguagePrimitives.GenericZero< ^T>
let v : int = run< int> ()
        """
        |> withOptimize
        |> asLibrary
        |> compile
        |> shouldSucceed
