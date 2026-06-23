module Language.NotNullIfNotNull

open FSharp.Test
open FSharp.Test.Compiler

let withStrictNullness cu =
    cu
    |> withLangVersionPreview
    |> withCheckNulls
    |> withWarnOn 3261
    |> withOptions ["--warnaserror+"]

let typeCheckWithStrictNullness cu =
    cu
    |> withStrictNullness
    |> typecheck

let csNotNullLib =
    CSharp """
#nullable enable
using System.Diagnostics.CodeAnalysis;
namespace NotNullLib {
    public class C {
        [return: NotNullIfNotNull("input")]
        public static string? Echo(string? input) => input;

        // The result is non-null when the SECOND parameter is non-null.
        [return: NotNullIfNotNull("second")]
        public static string? DependsOnSecond(string? first, string? second) => second;
    }

    public static class Extensions {
        // Degenerate case: the return depends on the 'this' parameter of a C#-style extension method.
        // When called instance-style the receiver is an object argument, not an unnamed caller argument.
        [return: NotNullIfNotNull("self")]
        public static string? PreferSelf(this string? self, string? other) => self ?? other;
    }

    public static class Variadic {
        // The result depends on an optional parameter ('b') that is not in the first position.
        [return: NotNullIfNotNull("b")]
        public static string? PickB(string? a = null, string? b = null) => b ?? a;

        // The result depends on the first parameter, which precedes a params array.
        [return: NotNullIfNotNull("first")]
        public static string? JoinRest(string? first, params string?[] rest) => first;
    }
}""" |> withName "csNotNullLib"

let private nullableExpected = "was expected but this expression is nullable"

[<FactForNETCOREAPP>]
let ``BCL Path.GetExtension - non-null input yields non-null result`` () =
    FSharp """module MyLibrary
open System.IO

let nonNull : string = "file.txt"
let ext : string = Path.GetExtension(nonNull)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``BCL Path.GetExtension - nullable input yields nullable result`` () =
    FSharp """module MyLibrary
open System.IO

let maybeNull : string | null = "file.txt"
let ext : string = Path.GetExtension(maybeNull)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``Multiple NotNullIfNotNull attributes are not supported - Delegate.Combine stays nullable`` () =
    // Delegate.Combine carries two [return: NotNullIfNotNull] attributes. We cannot currently represent nullness linking
    // to multiple types (logical OR), so the declared nullable return type is kept even though an argument is non-null.
    FSharp """module MyLibrary
open System

let d1 : Delegate = Action(fun () -> ()) :> Delegate
let dMaybe : Delegate | null = null

let combined : Delegate = Delegate.Combine(d1, dMaybe)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - non-null propagation works positionally`` () =
    FSharp """module MyLibrary
open NotNullLib

let notNull : string = "x"
let maybeNull : string | null = "y"

// Single referenced parameter, passed positionally
let r1 : string = C.Echo(notNull)

// Referenced parameter is the second one; nullable first, non-null second -> non-null.
// Arguments are positional (no named arguments), so this proves the parameter is identified by name.
let r2 : string = C.DependsOnSecond(maybeNull, notNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - non-null propagation works with named arguments`` () =
    FSharp """module MyLibrary
open NotNullLib

let notNull : string = "x"
let maybeNull : string | null = "y"

let r : string = C.DependsOnSecond(second = notNull, first = maybeNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - Echo stays nullable for nullable input`` () =
    FSharp """module MyLibrary
open NotNullLib

let maybeNull : string | null = "y"
let r : string = C.Echo(maybeNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - depends on second parameter, not the first`` () =
    FSharp """module MyLibrary
open NotNullLib

let notNull : string = "x"
let maybeNull : string | null = "y"

// Non-null first but nullable referenced (second) parameter -> result stays nullable
let r : string = C.DependsOnSecond(notNull, maybeNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - extension this-parameter must be identified, not the explicit argument`` () =
    FSharp """module MyLibrary
open NotNullLib

let notNull : string = "x"
let maybeNull : string | null = "y"

// Result depends on 'self' (the receiver), which is nullable -> result must stay nullable and warn.
let r : string = maybeNull.PreferSelf(notNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - optional parameter referenced positionally`` () =
    FSharp """module MyLibrary
open NotNullLib

let notNull : string = "x"
let maybeNull : string | null = "y"

// 'b' is the referenced (second, optional) parameter, passed positionally and non-null -> result non-null.
let r : string = Variadic.PickB(maybeNull, notNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - optional parameter referenced by name`` () =
    FSharp """module MyLibrary
open NotNullLib

let notNull : string = "x"

// Only the referenced optional parameter is supplied, by name and non-null -> result non-null.
let r : string = Variadic.PickB(b = notNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - optional parameter omitted stays nullable`` () =
    FSharp """module MyLibrary
open NotNullLib

let notNull : string = "x"

// The referenced optional parameter 'b' is omitted (defaults to null) -> result stays nullable.
let r : string = Variadic.PickB(notNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - parameter before params array, non-null propagation`` () =
    FSharp """module MyLibrary
open NotNullLib

let notNull : string = "x"
let maybeNull : string | null = "y"

// Referenced parameter 'first' precedes the params array; non-null first -> result non-null.
let r : string = Variadic.JoinRest(notNull, maybeNull, maybeNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - parameter before params array, stays nullable`` () =
    FSharp """module MyLibrary
open NotNullLib

let notNull : string = "x"
let maybeNull : string | null = "y"

// Referenced parameter 'first' is nullable -> result stays nullable regardless of params args.
let r : string = Variadic.JoinRest(maybeNull, notNull, notNull)
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``Csharp NotNullIfNotNull - unannotated parameter with non-null return annotation fails`` () =
    FSharp """module MyLibrary
open NotNullLib

let f x : string = C.Echo(x)
let _ : string = f null
"""
    |> asLibrary
    |> withReferences [csNotNullLib]
    |> withStrictNullness
    |> compile
    |> shouldFail
    |> withDiagnosticMessageMatches "Nullness warning: The type 'string' does not support 'null'."

[<FactForNETCOREAPP>]
let ``Local F# method with NotNullIfNotNull - non-null propagation`` () =
    FSharp """module MyLibrary
open System.Diagnostics.CodeAnalysis

type C =
    [<return: NotNullIfNotNull("x")>]
    static member Echo(x: string | null) : string | null = x

let notNull : string = "a"
let ok : string = C.Echo(notNull)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Local F# method with NotNullIfNotNull - stays nullable for nullable input`` () =
    FSharp """module MyLibrary
open System.Diagnostics.CodeAnalysis

type C =
    [<return: NotNullIfNotNull("x")>]
    static member Echo(x: string | null) : string | null = x

let maybeNull : string | null = "a"
let bad : string = C.Echo(maybeNull)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``Referenced F# method with NotNullIfNotNull - non-null propagation`` () =
    let fsharpLib =
        FSharp """module NotNullFSharpLib
open System.Diagnostics.CodeAnalysis

type C =
    [<return: NotNullIfNotNull("x")>]
    static member Echo(x: string | null) : string | null = x
"""
        |> withCheckNulls
        |> withName "NotNullFSharpLib"

    FSharp """module MyLibrary
open NotNullFSharpLib

let notNull : string = "a"
let ok : string = C.Echo(notNull)
"""
    |> asLibrary
    |> withReferences [fsharpLib]
    |> withStrictNullness
    |> compile
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Referenced F# method with NotNullIfNotNull - stays nullable for nullable input`` () =
    let fsharpLib =
        FSharp """module NotNullFSharpLib
open System.Diagnostics.CodeAnalysis

type C =
    [<return: NotNullIfNotNull("x")>]
    static member Echo(x: string | null) : string | null = x
"""
        |> withCheckNulls
        |> withName "NotNullFSharpLib"

    FSharp """module MyLibrary
open NotNullFSharpLib

let maybeNull : string | null = "a"
let bad : string = C.Echo(maybeNull)
"""
    |> asLibrary
    |> withReferences [fsharpLib]
    |> withStrictNullness
    |> compile
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``BCL Path.GetExtension - null literal input yields nullable result`` () =
    FSharp """module MyLibrary
open System.IO

let ext : string = Path.GetExtension(null)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``BCL Path.GetExtension - null-bound variable input yields nullable result`` () =
    FSharp """module MyLibrary
open System.IO

let maybeNull = null
let ext : string = Path.GetExtension(maybeNull)
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldFail
    |> withDiagnosticMessageMatches nullableExpected

[<FactForNETCOREAPP>]
let ``BCL Path.GetExtension - explicit non-null parameter annotation yields non-null result`` () =
    FSharp """module MyLibrary
open System.IO

let f (x: string) : string = Path.GetExtension x
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed