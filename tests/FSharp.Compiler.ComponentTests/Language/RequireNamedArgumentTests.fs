// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// FS-1095: RequireNamedArgumentAttribute (recognised by full type name) forces named-argument call syntax.
module RequireNamedArgumentTests =

    let private fsPolyfillTargeting (targets: string) =
        $"""
namespace System.Runtime.CompilerServices

open System

[<AttributeUsage({targets})>]
type RequireNamedArgumentAttribute() =
    inherit Attribute()
"""

    let private fsPolyfill = fsPolyfillTargeting "AttributeTargets.Method"

    let private withPolyfill (extra: string) = FSharp(fsPolyfill + extra)

    // Variant that also targets constructors, so the constructor fixtures can apply it without an AttributeUsage error at the declaration.
    let private withPolyfillCtor (extra: string) =
        FSharp(fsPolyfillTargeting "AttributeTargets.Method ||| AttributeTargets.Constructor" + extra)

    let private acceptsNamed cu =
        cu |> withLangVersionPreview |> typecheck |> shouldSucceed |> ignore

    let private rejectsCompiled cu =
        cu |> withLangVersionPreview |> compile |> shouldFail |> withErrorCode 3916 |> ignore

    let private acceptsCompiled cu =
        cu |> withLangVersionPreview |> compile |> shouldSucceed |> ignore

    let private requiresNamed (name: string) =
        $"The method '{name}' requires named arguments. Use named-argument syntax, e.g. 'MethodName(argumentName = value)'."

    // Merge several call sites into one run; count-exact (one FS3916 per listed method, identified by name).
    let private rejectsAll (methods: string list) cu =
        cu |> withLangVersionPreview |> typecheck |> shouldFail |> withErrorMessages (List.map requiresNamed methods) |> ignore

    let private rejectsAllCompiled (methods: string list) cu =
        cu |> withLangVersionPreview |> compile |> shouldFail |> withErrorMessages (List.map requiresNamed methods) |> ignore

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

    let private csPolyfillTargeting (targets: string) =
        """
using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(""" + targets + """, AllowMultiple = false, Inherited = false)]
    public sealed class RequireNamedArgumentAttribute : Attribute { }
}
"""

    let private csharpWithPolyfillTargeting (targets: string) (extra: string) = CSharp(csPolyfillTargeting targets + extra)

    let private csharpWithPolyfill (extra: string) = csharpWithPolyfillTargeting "AttributeTargets.Method" extra

    let private csAnnotatedLib =
        csharpWithPolyfill """
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
        csharpWithPolyfill """
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

    // Shared same-compilation-unit surface: every annotated shape declared once, each member
    // distinctly named so a merged FS3916 assertion pins the exact violating call site.
    let private annotatedApi = """
namespace Test

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type Api =
    [<RequireNamedArgument>] static member Basic(x: int, y: int) = x + y
    [<RequireNamedArgument>] static member Mixed(x: int, y: int) = x + y
    [<RequireNamedArgument>] static member FirstClass(x: int, y: int) = x + y
    [<RequireNamedArgument>] static member Zero() = 42
    [<RequireNamedArgument>] static member Optional(x: int, [<Optional; DefaultParameterValue(0)>] y: int) = x + y
    [<RequireNamedArgument>] static member Params([<ParamArray>] rest: int[]) = Array.sum rest
    [<RequireNamedArgument>] static member Generic<'T>(value: 'T) = value
    static member Overloaded(x: int, y: int) = x + y
    [<RequireNamedArgument>] static member Overloaded(x: string, y: string) = x + y

type IFace =
    [<RequireNamedArgument>] abstract member ViaSlot: x: int * y: int -> int

type Delegated =
    [<RequireNamedArgument>] static member Ping(x: int) = x

type Holder() =
    member _.Value = 0

[<AutoOpen>]
module Extensions =
    type Holder with
        [<RequireNamedArgument>] member _.Ext(x: int, y: int) = x + y

type IndexerGet() =
    member _.Item with [<RequireNamedArgument>] get (i: int) = i * 2

type IndexerSet() =
    let mutable store = 0
    member _.Item with [<RequireNamedArgument>] set (i: int) (v: int) = store <- i + v

type PropertySet() =
    let mutable store = 0
    member _.P with [<RequireNamedArgument>] set (v: int) = store <- v

type Curried =
    [<RequireNamedArgument>] static member Add (x: int) (y: int) = x + y
"""

    let private withZoo (extra: string) = withPolyfill (annotatedApi + extra)

    [<Fact>]
    let ``Same compilation unit - positional and positional-like calls are all rejected`` () =
        withZoo """
module Use =
    let basic = Api.Basic(1, 2)
    let mixed = Api.Mixed(1, y = 2)
    let firstClass = Api.FirstClass
    let optional = Api.Optional(1, 2)
    let paramArray = Api.Params(1, 2, 3)
    let generic = Api.Generic(5)
    let overloaded = Api.Overloaded("a", "b")
    let viaInterface (i: IFace) = i.ViaSlot(1, 2)
    let ext = Holder().Ext(1, 2)
    let asDelegate = System.Func<int, int>(Delegated.Ping)
"""
        |> rejectsAll [ "Basic"; "Mixed"; "FirstClass"; "Optional"; "Params"; "Generic"; "Overloaded"; "ViaSlot"; "Ext"; "Ping" ]

    [<Fact>]
    let ``Same compilation unit - named and non-applicable calls are all accepted`` () =
        withZoo """
module Use =
    let basic = Api.Basic(x = 1, y = 2)
    let zero = Api.Zero()
    let optionalOmitted = Api.Optional(x = 1)
    let paramArray = Api.Params(rest = [| 1; 2; 3 |])
    let generic = Api.Generic(value = 5)
    let overloadUnannotated = Api.Overloaded(1, 2)
    let ext = Holder().Ext(x = 1, y = 2)
    let lambdaForward x y = Api.Basic(x = x, y = y)
    // attribute present but no named-argument form applies:
    let indexerGet = IndexerGet().[1]
    let indexerSet = let c = IndexerSet() in c.[1] <- 2
    let propertySet = let c = PropertySet() in c.P <- 5
    let curried = Curried.Add 1 2
"""
        |> acceptsNamed

    [<Fact>]
    let ``Different F# assembly - positional call is an error`` () =
        FSharp """
module Test
open AnnotatedLib
let r = Api.Add(1, 2)
"""
        |> withReferences [ fsAnnotatedLib ]
        |> rejectsCompiled

    [<Fact>]
    let ``Different F# assembly - named call succeeds`` () =
        FSharp """
module Test
open AnnotatedLib
let r = Api.Add(x = 1, y = 2)
"""
        |> withReferences [ fsAnnotatedLib ]
        |> acceptsCompiled

    [<FactForNETCOREAPP>]
    let ``C# assembly (IL methods) - positional calls are errors`` () =
        FSharp """
module Test
open AnnotatedLib
let plain = Api.Add(1, 2)
let optionalOmitted = Api.Scale(5)
"""
        |> withReferences [ csAnnotatedLib ]
        |> rejectsAllCompiled [ "Add"; "Scale" ]

    [<FactForNETCOREAPP>]
    let ``C# assembly (IL methods) - named calls succeed`` () =
        FSharp """
module Test
open AnnotatedLib
let plain = Api.Add(x = 1, y = 2)
let optionalOmitted = Api.Scale(x = 5)
"""
        |> withReferences [ csAnnotatedLib ]
        |> acceptsCompiled

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
        |> acceptsNamed

    [<FactForNETCOREAPP>]
    let ``Same-named attribute from a different C# namespace is not recognised`` () =
        FSharp """
module Test
open AnnotatedLib
let r = WrongApi.Add(1, 2)
"""
        |> withReferences [ csWrongNamespaceLib ]
        |> acceptsCompiled

    [<FactForNETCOREAPP>]
    let ``C# extension methods - positional calls are errors (receiver is not a positional argument)`` () =
        FSharp """
module Test
open AnnotatedLib
let plain = (1).AddTo(2)
let paramArray = (1).SumTo(2, 3)
"""
        |> withReferences [ csExtensionLib ]
        |> rejectsAllCompiled [ "AddTo"; "SumTo" ]

    [<FactForNETCOREAPP>]
    let ``C# extension methods - named calls succeed`` () =
        FSharp """
module Test
open AnnotatedLib
let plain = (1).AddTo(y = 2)
let paramArray = (1).SumTo(rest = [| 2; 3 |])
"""
        |> withReferences [ csExtensionLib ]
        |> acceptsCompiled

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
        |> withErrorCode 3916
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
        |> acceptsNamed

    let private csInterfaceLib =
        csharpWithPolyfill """
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
        csharpWithPolyfillTargeting "AttributeTargets.Constructor" """
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
    let ``C# interface - positional calls via the slot and the concrete type are errors`` () =
        FSharp """
module Test
let viaInterface (i: AnnotatedLib.IFoo) = i.ViaSlot(1, 2)
let viaConcrete (c: AnnotatedLib.FooImpl) = c.ViaSlot(1, 2)
"""
        |> withReferences [ csInterfaceLib ]
        |> rejectsAllCompiled [ "ViaSlot"; "ViaSlot" ]

    [<FactForNETCOREAPP>]
    let ``C# interface - named call via the slot succeeds`` () =
        FSharp """
module Test
let call (i: AnnotatedLib.IFoo) = i.ViaSlot(x = 1, y = 2)
"""
        |> withReferences [ csInterfaceLib ]
        |> acceptsCompiled

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
        |> withErrorCode 3916
        |> withDiagnosticMessageMatches "The method 'S' requires named arguments"
        |> ignore

    [<FactForNETCOREAPP>]
    let ``C# struct constructor - named call succeeds`` () =
        FSharp """
module Test
let s = AnnotatedLib.S(x = 1, y = 2)
"""
        |> withReferences [ csStructCtorLib ]
        |> acceptsCompiled

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
        |> rejectsCompiled
