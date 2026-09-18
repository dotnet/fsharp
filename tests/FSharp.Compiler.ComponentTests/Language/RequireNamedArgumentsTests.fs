// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// FS-1095: RequireNamedArgumentsAttribute (recognised by full type name) forces named-argument call syntax.
module RequireNamedArgumentsTests =

    let private fsPolyfill =
        """
namespace System.Diagnostics.CodeAnalysis

open System

[<Sealed; AttributeUsage(AttributeTargets.Method ||| AttributeTargets.Constructor ||| AttributeTargets.Property ||| AttributeTargets.Delegate ||| AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)>]
type RequireNamedArgumentsAttribute() =
    inherit Attribute()
"""

    let private withPolyfill (extra: string) = FSharp(fsPolyfill + extra)

    let private acceptsNamed cu =
        cu |> withLangVersionPreview |> typecheck |> shouldSucceed |> ignore

    let private rejectsCompiled cu =
        cu |> withLangVersionPreview |> compile |> shouldFail |> withErrorCode 3923 |> ignore

    let private acceptsCompiled cu =
        cu |> withLangVersionPreview |> compile |> shouldSucceed |> ignore

    let private requiresNamed (name: string) =
        $"The method '{name}' requires named arguments. Use named-argument syntax, e.g. 'MethodName(argumentName = value)'."

    // Merge several call sites into one run; count-exact (one FS3923 per listed method, identified by name).
    let private rejectsAll (methods: string list) cu =
        cu |> withLangVersionPreview |> typecheck |> shouldFail |> withErrorMessages (List.map requiresNamed methods) |> ignore

    let private rejectsAllCompiled (methods: string list) cu =
        cu |> withLangVersionPreview |> compile |> shouldFail |> withErrorMessages (List.map requiresNamed methods) |> ignore

    let private fsAnnotatedLib =
        withPolyfill """
namespace AnnotatedLib

open System.Diagnostics.CodeAnalysis

type Api =
    [<RequireNamedArguments>]
    static member Add(x: int, y: int) = x + y
"""
        |> asLibrary
        |> withName "FsAnnotatedLib"

    let private csPolyfill =
        """
using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Delegate | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class RequireNamedArgumentsAttribute : Attribute { }
}
"""

    let private csharpWithPolyfill (extra: string) = CSharp(csPolyfill + extra)

    let private csAnnotatedLib =
        csharpWithPolyfill """
namespace AnnotatedLib
{
    public static class Api
    {
        [RequireNamedArguments]
        public static int Add(int x, int y) => x + y;

        [RequireNamedArguments]
        public static int Scale(int x, int factor = 2) => x * factor;
    }
}
"""
        |> asLibrary
        |> withName "CsAnnotatedLib"

    let private csExtensionLib =
        csharpWithPolyfill """
namespace AnnotatedLib
{
    public static class Ext
    {
        [RequireNamedArguments]
        public static int AddTo(this int self, int y) => self + y;

        [RequireNamedArguments]
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
    // distinctly named so a merged FS3923 assertion pins the exact violating call site.
    let private annotatedApi = """
namespace Test

open System
open System.Diagnostics.CodeAnalysis
open System.Runtime.InteropServices

type Api =
    [<RequireNamedArguments>] static member Basic(x: int, y: int) = x + y
    [<RequireNamedArguments>] static member Mixed(x: int, y: int) = x + y
    [<RequireNamedArguments>] static member FirstClass(x: int, y: int) = x + y
    [<RequireNamedArguments>] static member Zero() = 42
    [<RequireNamedArguments>] static member Optional(x: int, [<Optional; DefaultParameterValue(0)>] y: int) = x + y
    [<RequireNamedArguments>] static member Params([<ParamArray>] rest: int[]) = Array.sum rest
    [<RequireNamedArguments>] static member Generic<'T>(value: 'T) = value
    static member Parameter([<RequireNamedArguments>] x: int) = x
    static member Overloaded(x: int, y: int) = x + y
    [<RequireNamedArguments>] static member Overloaded(x: string, y: string) = x + y

type IFace =
    [<RequireNamedArguments>] abstract member ViaSlot: x: int * y: int -> int

type Delegated =
    [<RequireNamedArguments>] static member Ping(x: int) = x

[<RequireNamedArguments>]
type Callback = delegate of x: int -> int

type Holder() =
    member _.Value = 0

[<AutoOpen>]
module Extensions =
    type Holder with
        [<RequireNamedArguments>] member _.Ext(x: int, y: int) = x + y

type IndexerGet() =
    member _.Item with [<RequireNamedArguments>] get (i: int) = i * 2

type IndexerSet() =
    let mutable store = 0
    member _.Item with [<RequireNamedArguments>] set (i: int) (v: int) = store <- i + v

type PropertySet() =
    let mutable store = 0
    [<RequireNamedArguments>]
    member _.P with [<RequireNamedArguments>] set (v: int) = store <- v

type Curried =
    [<RequireNamedArguments>] static member Add (x: int) (y: int) = x + y
"""

    let private withZoo (extra: string) = withPolyfill (annotatedApi + extra)

    [<Fact>]
    let ``Same compilation unit - positional and positional-like calls are all rejected`` () =
        withZoo """
module Use =
    let name = nameof Api.Basic
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
    let parameter = Api.Parameter(1)
    let callback = Callback(fun x -> x).Invoke(1)
"""
        |> acceptsNamed

    [<Fact>]
    let ``Nameof accepts annotated methods in the same compilation unit`` () =
        withZoo """
module Use =
    let basic = nameof Api.Basic
    let generic = nameof Api.Generic<int>
    let viaInterface (i: IFace) = nameof i.ViaSlot
    let extension (h: Holder) = nameof h.Ext
    let pattern = function nameof Api.Basic -> true | _ -> false
"""
        |> acceptsNamed

    [<TheoryForNETCOREAPP>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``Nameof accepts annotated methods from another assembly`` (csharp: bool) =
        FSharp """
module Test
open AnnotatedLib
let name = nameof Api.Add
"""
        |> withReferences [ if csharp then csAnnotatedLib else fsAnnotatedLib ]
        |> acceptsCompiled

    [<Theory>]
    [<InlineData("9.0")>]
    [<InlineData("preview")>]
    let ``Local DllImport attribute does not replace a managed method body`` (langVersion: string) =
        FSharp """
namespace System.Runtime.InteropServices
open System
[<Sealed; AttributeUsage(AttributeTargets.Method)>]
type DllImportAttribute(libraryName: string) =
    inherit Attribute()

namespace Test
open System.Runtime.InteropServices
module Program =
    [<DllImport("missing-library")>]
    let add (x: int) (y: int) = x + y

    [<EntryPoint>]
    let main _ = if add 20 22 = 42 then 0 else 1
"""
        |> withLangVersion langVersion
        |> compileExeAndRun
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

open System.Diagnostics.CodeAnalysis

type C =
    [<RequireNamedArguments>]
    static member Add(x: int, y: int) = x + y

module Use =
    let r = C.Add(1, 2)
"""
        |> withLangVersion "9.0"
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Theory>]
    [<InlineData("MyApp", "RequireNamedArguments")>]
    [<InlineData("System.Runtime.CompilerServices", "RequireNamedArguments")>]
    [<InlineData("System.Runtime.CompilerServices", "RequireNamedArgument")>]
    [<InlineData("System.Diagnostics.CodeAnalysis", "RequireNamedArgument")>]
    let ``Local attribute with a different namespace or name is not recognised`` (ns: string) (name: string) =
        FSharp(
            (fsPolyfill + annotatedApi).Replace("System.Diagnostics.CodeAnalysis", ns).Replace("RequireNamedArguments", name)
            + """
module Use =
    let r = Api.Basic(1, 2)
"""
        )
        |> acceptsNamed

    [<TheoryForNETCOREAPP>]
    [<InlineData("MyApp", "RequireNamedArguments")>]
    [<InlineData("System.Runtime.CompilerServices", "RequireNamedArguments")>]
    [<InlineData("System.Runtime.CompilerServices", "RequireNamedArgument")>]
    [<InlineData("System.Diagnostics.CodeAnalysis", "RequireNamedArgument")>]
    let ``Imported attribute with a different namespace or name is not recognised`` (ns: string) (name: string) =
        let source =
            csPolyfill + """
public static class Api
{
    [RequireNamedArguments]
    public static int Add(int x, int y) => x + y;
}
"""

        FSharp """
module Test
let r = Api.Add(1, 2)
"""
        |> withReferences [
            CSharp(source.Replace("System.Diagnostics.CodeAnalysis", ns).Replace("RequireNamedArguments", name))
            |> withName "WrongAttributeLib"
        ]
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
        withPolyfill """
namespace Test
open System.Diagnostics.CodeAnalysis
type C [<RequireNamedArguments>] (x: int, y: int) =
    member _.V = x + y
module Use =
    let c = C(1, 2)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 3923
        |> withDiagnosticMessageMatches "The method 'C' requires named arguments"
        |> ignore

    [<Fact>]
    let ``Constructor named call succeeds`` () =
        withPolyfill """
namespace Test
open System.Diagnostics.CodeAnalysis
type C [<RequireNamedArguments>] (x: int, y: int) =
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
        [RequireNamedArguments]
        int ViaSlot(int x, int y);
    }

    public class FooImpl : IFoo
    {
        [RequireNamedArguments]
        public int ViaSlot(int x, int y) => x + y;
    }
}
"""
        |> asLibrary
        |> withName "CsInterfaceLib"

    let private csStructCtorLib =
        csharpWithPolyfill """
namespace AnnotatedLib
{
    public struct S
    {
        public int X;
        public int Y;

        [RequireNamedArguments]
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
        |> withErrorCode 3923
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
open System.Diagnostics.CodeAnalysis
type C =
    [<RequireNamedArguments>]
    static member Add(x: int, y: int) = x + y
let r = C.Add(1, 2)
"""
        |> withReferences [ csAnnotatedLib ]
        |> rejectsCompiled
