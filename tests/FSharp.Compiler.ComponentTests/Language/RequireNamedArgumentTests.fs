// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// FS-1095: RequireNamedArgumentAttribute (recognised by full type name) forces named-argument call syntax.
module RequireNamedArgumentTests =

    let private fsPolyfill =
        """
namespace System.Runtime.CompilerServices

open System

[<AttributeUsage(AttributeTargets.Method)>]
type RequireNamedArgumentAttribute() =
    inherit Attribute()
"""

    let private withPolyfill (extra: string) = FSharp(fsPolyfill + extra)

    // Permissive polyfill that also targets constructors (the real Method-only BCL attribute warns there).
    let private withPolyfillCtor (extra: string) =
        FSharp("""
namespace System.Runtime.CompilerServices

open System

[<AttributeUsage(AttributeTargets.Method ||| AttributeTargets.Constructor)>]
type RequireNamedArgumentAttribute() =
    inherit Attribute()
""" + extra)

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

        [RequireNamedArgument]
        public static int Scale(int x, int factor = 2) => x * factor;
    }
}
"""
        |> asLibrary
        |> withName "CsAnnotatedLib"

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

    let private csExtensionLib =
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
    public static class Ext
    {
        [RequireNamedArgument]
        public static int AddTo(this int self, int y) => self + y;

        [RequireNamedArgument]
        public static int SumTo(this int self, params int[] rest)
        {
            int s = self;
            foreach (var r in rest) s += r;
            return s;
        }
    }
}
"""
        |> asLibrary
        |> withName "CsExtensionLib"

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

    [<Fact>]
    let ``Overload resolution - positional call binds the unannotated overload and succeeds`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C =
    static member Add(x: int, y: int) = x + y
    [<RequireNamedArgument>]
    static member Add(x: string, y: string) = x + y

module Use =
    let r = C.Add(1, 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Overload resolution - positional call binds the annotated overload and is an error`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C =
    static member Add(x: int, y: int) = x + y
    [<RequireNamedArgument>]
    static member Add(x: string, y: string) = x + y

module Use =
    let r = C.Add("a", "b")
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``F# extension member - positional call is an error`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C() =
    member _.Value = 0

[<AutoOpen>]
module Extensions =
    type C with
        [<RequireNamedArgument>]
        member _.Add(x: int, y: int) = x + y

module Use =
    let r = C().Add(1, 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``F# extension member - named call succeeds`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C() =
    member _.Value = 0

[<AutoOpen>]
module Extensions =
    type C with
        [<RequireNamedArgument>]
        member _.Add(x: int, y: int) = x + y

module Use =
    let r = C().Add(x = 1, y = 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# extension method - positional call is an error, receiver is not a positional argument`` () =
        FSharp """
module Test
open AnnotatedLib
let r = (1).AddTo(2)
"""
        |> withReferences [ csExtensionLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# extension method - named call succeeds`` () =
        FSharp """
module Test
open AnnotatedLib
let r = (1).AddTo(y = 2)
"""
        |> withReferences [ csExtensionLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``First-class use of an annotated method is an error`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C =
    [<RequireNamedArgument>]
    static member Add(x: int, y: int) = x + y

module Use =
    let f = C.Add
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``Explicit lambda forwarding with named arguments succeeds`` () =
        withPolyfill """
namespace Test

open System.Runtime.CompilerServices

type C =
    [<RequireNamedArgument>]
    static member Add(x: int, y: int) = x + y

module Use =
    let f x y = C.Add(x = x, y = y)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Indexer getter with the attribute on its accessor is not enforced`` () =
        withPolyfill """
namespace Test
open System.Runtime.CompilerServices
type C() =
    member _.Item with [<RequireNamedArgument>] get (i: int) = i * 2
module Use =
    let c = C()
    let r = c.[1]
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Indexer setter with the attribute on its accessor is not enforced`` () =
        withPolyfill """
namespace Test
open System.Runtime.CompilerServices
type C() =
    let mutable store = 0
    member _.Item with [<RequireNamedArgument>] set (i: int) (v: int) = store <- i + v
module Use =
    let c = C()
    c.[1] <- 2
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Property setter with the attribute on its accessor is not enforced`` () =
        withPolyfill """
namespace Test
open System.Runtime.CompilerServices
type C() =
    let mutable store = 0
    member _.P with [<RequireNamedArgument>] set (v: int) = store <- v
module Use =
    let c = C()
    c.P <- 5
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Curried member is not enforced because it has no named-argument form`` () =
        withPolyfill """
namespace Test
open System.Runtime.CompilerServices
type C =
    [<RequireNamedArgument>]
    static member Add (x: int) (y: int) = x + y
module Use =
    let r = C.Add 1 2
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Constructor positional call is rejected and the diagnostic names the type`` () =
        withPolyfillCtor """
namespace Test
open System.Runtime.CompilerServices
type C [<RequireNamedArgument>] (x: int, y: int) =
    member _.V = x + y
module Use =
    let c = C(1, 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> withDiagnosticMessageMatches "The method 'C' requires named arguments"
        |> ignore

    [<Fact>]
    let ``Constructor named call succeeds`` () =
        withPolyfillCtor """
namespace Test
open System.Runtime.CompilerServices
type C [<RequireNamedArgument>] (x: int, y: int) =
    member _.V = x + y
module Use =
    let c = C(x = 1, y = 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# extension method with ParamArray - positional call is an error`` () =
        FSharp """
module Test
open AnnotatedLib
let r = (1).SumTo(2, 3)
"""
        |> withReferences [ csExtensionLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# extension method with ParamArray - named array call succeeds`` () =
        FSharp """
module Test
open AnnotatedLib
let r = (1).SumTo(rest = [| 2; 3 |])
"""
        |> withReferences [ csExtensionLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# method with an optional parameter - positional call is an error`` () =
        FSharp """
module Test
open AnnotatedLib
let r = Api.Scale(5)
"""
        |> withReferences [ csAnnotatedLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# method with an optional parameter - named call omitting the optional succeeds`` () =
        FSharp """
module Test
open AnnotatedLib
let r = Api.Scale(x = 5)
"""
        |> withReferences [ csAnnotatedLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Generic annotated method - the attribute survives instantiation`` () =
        withPolyfill """
namespace Test
open System.Runtime.CompilerServices
type C =
    [<RequireNamedArgument>]
    static member Id<'T>(value: 'T) = value
module Use =
    let r = C.Id(5)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<Fact>]
    let ``Generic annotated method - named call succeeds`` () =
        withPolyfill """
namespace Test
open System.Runtime.CompilerServices
type C =
    [<RequireNamedArgument>]
    static member Id<'T>(value: 'T) = value
module Use =
    let r = C.Id(value = 5)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    let private csInterfaceLib =
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
    public interface IFoo
    {
        [RequireNamedArgument]
        int ViaSlot(int x, int y);
    }

    public class FooImpl : IFoo
    {
        [RequireNamedArgument]
        public int ViaSlot(int x, int y) => x + y;
    }
}
"""
        |> asLibrary
        |> withName "CsInterfaceLib"

    let private csStructCtorLib =
        CSharp """
using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class RequireNamedArgumentAttribute : Attribute { }
}

namespace AnnotatedLib
{
    public struct S
    {
        public int X;
        public int Y;

        [RequireNamedArgument]
        public S(int x, int y) { X = x; Y = y; }
    }
}
"""
        |> asLibrary
        |> withName "CsStructCtorLib"

    [<FactForNETCOREAPP>]
    let ``C# interface slot - positional call via the interface is an error`` () =
        FSharp """
module Test
let call (i: AnnotatedLib.IFoo) = i.ViaSlot(1, 2)
"""
        |> withReferences [ csInterfaceLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# interface slot - named call via the interface succeeds`` () =
        FSharp """
module Test
let call (i: AnnotatedLib.IFoo) = i.ViaSlot(x = 1, y = 2)
"""
        |> withReferences [ csInterfaceLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# class override - positional call on the concrete type is an error`` () =
        FSharp """
module Test
let call (c: AnnotatedLib.FooImpl) = c.ViaSlot(1, 2)
"""
        |> withReferences [ csInterfaceLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# struct constructor - positional call is an error`` () =
        FSharp """
module Test
let s = AnnotatedLib.S(1, 2)
"""
        |> withReferences [ csStructCtorLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3910
        |> withDiagnosticMessageMatches "The method 'S' requires named arguments"
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# struct constructor - named call succeeds`` () =
        FSharp """
module Test
let s = AnnotatedLib.S(x = 1, y = 2)
"""
        |> withReferences [ csStructCtorLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Method group coerced to a delegate cannot smuggle a positional call`` () =
        withPolyfill """
namespace Test
open System.Runtime.CompilerServices
type C =
    [<RequireNamedArgument>]
    static member Ping(x: int) = x
module Use =
    let f = System.Func<int, int>(C.Ping)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3910
        |> ignore

    [<FactForNETCOREAPP>]
    let ``Local F# method annotated with an attribute imported from a referenced assembly is enforced`` () =
        FSharp """
module Test
open System.Runtime.CompilerServices
type C =
    [<RequireNamedArgument>]
    static member Add(x: int, y: int) = x + y
let r = C.Add(1, 2)
"""
        |> withReferences [ csAnnotatedLib ]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3910
        |> ignore
