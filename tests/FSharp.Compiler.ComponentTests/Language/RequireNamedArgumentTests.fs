// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// FS-1095: a method annotated with System.Runtime.CompilerServices.RequireNamedArgumentAttribute
/// must be called using named-argument syntax. The attribute is recognised by full type name only
/// (polyfill), so it is honoured whether it originates from the same compilation unit, a different
/// assembly, or (mechanically identically to a referenced assembly) the runtime assembly.
module RequireNamedArgumentTests =

    // F# polyfill of the attribute, prepended to the same-compilation-unit sources below.
    let private fsPolyfill =
        """
namespace System.Runtime.CompilerServices

open System

[<AttributeUsage(AttributeTargets.Method)>]
type RequireNamedArgumentAttribute() =
    inherit Attribute()
"""

    /// Build a single-compilation source that polyfills the attribute and then uses it.
    let private withPolyfill (extra: string) = FSharp(fsPolyfill + extra)

    // A separately compiled F# assembly that polyfills the attribute and exposes an annotated method.
    // Consuming it exercises the FSMeth (cached Val flags) classification path for a *different assembly*.
    let private fsAnnotatedLib =
        withPolyfill """
namespace AnnotatedLib

open System.Runtime.CompilerServices

type Api =
    [<RequireNamedArgument>]
    static member Add(x: int, y: int) = x + y
"""
        |> asLibrary
        |> withName "FsAnnotatedLib"

    // A separately compiled C# assembly that polyfills the attribute and exposes an annotated method.
    // Consuming it exercises the ILMeth (cached IL flags) classification path - the same path the
    // runtime-defined attribute would take once the BCL ships it.
    let private csAnnotatedLib =
        CSharp """
using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RequireNamedArgumentAttribute : Attribute { }
}

namespace AnnotatedLib
{
    public static class Api
    {
        [RequireNamedArgument]
        public static int Add(int x, int y) => x + y;
    }
}
"""
        |> asLibrary
        |> withName "CsAnnotatedLib"

    // A separately compiled C# assembly whose annotated method carries a *different* attribute that merely
    // shares the simple name RequireNamedArgumentAttribute but lives in another namespace. Recognition is by
    // full type name, so this must NOT be treated as the well-known attribute.
    let private csWrongNamespaceLib =
        CSharp """
using System;

namespace MyApp
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RequireNamedArgumentAttribute : Attribute { }
}

namespace AnnotatedLib
{
    public static class WrongApi
    {
        [MyApp.RequireNamedArgument]
        public static int Add(int x, int y) => x + y;
    }
}
"""
        |> asLibrary
        |> withName "CsWrongNamespaceLib"

    [<Fact>]
    let ``Same compilation unit - positional call is an error`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C =
    [<RequireNamedArgument>]
    static member Add(x: int, y: int) = x + y

module Use =
    let r = C.Add(1, 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``Same compilation unit - named call succeeds`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C =
    [<RequireNamedArgument>]
    static member Add(x: int, y: int) = x + y

module Use =
    let r = C.Add(x = 1, y = 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Zero-argument annotated method - unnamed call is allowed`` () =
        // Regression guard: a call with no positional arguments must not be flagged, even when the
        // method carries the attribute (the classic 'M()' false positive to avoid).
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C =
    [<RequireNamedArgument>]
    static member Ping() = 42

module Use =
    let r = C.Ping()
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Different F# assembly - positional call is an error`` () =
        FSharp """
module Test
open AnnotatedLib
let r = Api.Add(1, 2)
"""
        |> withReferences [ fsAnnotatedLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``Different F# assembly - named call succeeds`` () =
        FSharp """
module Test
open AnnotatedLib
let r = Api.Add(x = 1, y = 2)
"""
        |> withReferences [ fsAnnotatedLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``Different C# assembly (IL method) - positional call is an error`` () =
        FSharp """
module Test
open AnnotatedLib
let r = Api.Add(1, 2)
"""
        |> withReferences [ csAnnotatedLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<FactForNETCOREAPP>]
    let ``Different C# assembly (IL method) - named call succeeds`` () =
        FSharp """
module Test
open AnnotatedLib
let r = Api.Add(x = 1, y = 2)
"""
        |> withReferences [ csAnnotatedLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Feature is off under non-preview langversion`` () =
        // The enforcement is language-version gated: under 9.0 a positional call must still succeed.
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C =
    [<RequireNamedArgument>]
    static member Add(x: int, y: int) = x + y

module Use =
    let r = C.Add(1, 2)
"""
        |> withLangVersion "9.0"
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Optional argument passed positionally is an error`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type C =
    [<RequireNamedArgument>]
    static member Add(x: int, [<Optional; DefaultParameterValue(0)>] y: int) = x + y

module Use =
    let positional = C.Add(1, 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``Optional argument may be omitted when the required argument is named`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type C =
    [<RequireNamedArgument>]
    static member Add(x: int, [<Optional; DefaultParameterValue(0)>] y: int) = x + y

module Use =
    let omitted = C.Add(x = 1)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Interface method - positional call through the interface is an error`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type I =
    [<RequireNamedArgument>]
    abstract member Add: x: int * y: int -> int

module Use =
    let f (i: I) = i.Add(1, 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``Mixed named and positional call is an error`` () =
        // A single positional argument alongside a named one must still be rejected.
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C =
    [<RequireNamedArgument>]
    static member Add(x: int, y: int) = x + y

module Use =
    let r = C.Add(1, y = 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``Same-named attribute in a different F# namespace is not recognised`` () =
        // Recognition is by full type name: an attribute that only shares the simple name must be ignored.
        FSharp """
namespace MyApp

open System

[<AttributeUsage(AttributeTargets.Method)>]
type RequireNamedArgumentAttribute() =
    inherit Attribute()

namespace Test

open MyApp

type C =
    [<RequireNamedArgument>]
    static member Add(x: int, y: int) = x + y

module Use =
    let r = C.Add(1, 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``Same-named attribute from a different C# namespace is not recognised`` () =
        FSharp """
module Test
open AnnotatedLib
let r = WrongApi.Add(1, 2)
"""
        |> withReferences [ csWrongNamespaceLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``ParamArray positional arguments are an error`` () =
        // Regression guard for the ParamArray hole: when every positional caller argument is captured by a
        // ParamArray the unnamed-arg count is zero, so enforcement must also inspect ParamArrayCallerArgs.
        withPolyfill """
namespace Test

open System
open System.Runtime.CompilerServices

type C =
    [<RequireNamedArgument>]
    static member Sum([<ParamArray>] rest: int[]) = Array.sum rest

module Use =
    let r = C.Sum(1, 2, 3)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``ParamArray passed by name as an array succeeds`` () =
        withPolyfill """
namespace Test

open System
open System.Runtime.CompilerServices

type C =
    [<RequireNamedArgument>]
    static member Sum([<ParamArray>] rest: int[]) = Array.sum rest

module Use =
    let r = C.Sum(rest = [| 1; 2; 3 |])
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore
