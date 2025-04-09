// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ConstraintSolver

open Xunit
open FSharp.Test.Compiler

module MemberConstraints =

    [<Fact>]
    let ``Invalid member constraint with ErrorRanges``() = // Regression test for FSharp1.0:2262
        FSharp """
 let inline length (x: ^a) : int = (^a : (member Length : int with get, set) (x, ()))
        """
        |> withErrorRanges
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 697, Line 2, Col 43, Line 2, Col 76, "Invalid constraint")

    [<Fact>]
    let ``We can overload operators on a type and not add all the extra jazz such as inlining and the ^ operator.``() =

        FSharp """
type Foo(x : int) =
    member this.Val = x

    static member (-->) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (-->) ((src : Foo), (target : int)) = new Foo(src.Val + target)

    static member (+) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (+) ((src : Foo), (target : int)) = new Foo(src.Val + target)

let x = Foo(3) --> 4
let y = Foo(3) --> Foo(4)
let x2 = Foo(3) + 4
let y2 = Foo(3) + Foo(4)

if x.Val <> 7 then failwith "x.Val <> 7"
elif y.Val <> 7 then  failwith "y.Val <> 7"
elif x2.Val <> 7 then  failwith "x2.Val <> 7"
elif y2.Val <> 7 then  failwith "x.Val <> 7"
else ()
"""
        |> asExe
        |> compile
        |> run
        |> shouldSucceed

    [<Fact>]
    let ``Respect nowarn 957 for extension method`` () =
        FSharp """        
module Foo

type DataItem<'data> =
    { Identifier: string
      Label: string
      Data: 'data }

    static member Create<'data>(identifier: string, label: string, data: 'data) =
        { DataItem.Identifier = identifier
          DataItem.Label = label
          DataItem.Data = data }

#nowarn "957"

type DataItem< ^input> with

    static member inline Create(item: ^input) =
        let stringValue: string = (^input: (member get_StringValue: unit -> string) (item))
        let friendlyStringValue: string = (^input: (member get_FriendlyStringValue: unit -> string) (item))

        DataItem.Create< ^input>(stringValue, friendlyStringValue, item)
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Indirect constraint by operator`` () =
        FSharp """
List.average [42] |> ignore
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 15, Line 2, Col 17, "'List.average' does not support the type 'int', because the latter lacks the required (real or built-in) member 'DivideByInt'")

    [<Fact>]
    let ``Direct constraint by named (pseudo) operator`` () =
        FSharp """
abs -1u |> ignore
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 6, Line 2, Col 8, "The type 'uint32' does not support the operator 'abs'")

    [<Fact>]
    let ``Direct constraint by simple operator`` () =
        FSharp """
"" >>> 1 |> ignore
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 1, Line 2, Col 3, "The type 'string' does not support the operator '>>>'")

    [<Fact>]
    let ``Direct constraint by pseudo operator`` () =
        FSharp """
ignore ["1" .. "42"]
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 9, Line 2, Col 12, "The type 'string' does not support the operator 'op_Range'")

    [<Fact>]
    let ``FsharpPlus style SRTP backwards compat broken`` () =
        FSharp """module TestLib
type Default3 = class end
type Default2 = class inherit Default3 end
type Default1 = class inherit Default2 end

type IsAltLeftZero =
    inherit Default1

    static member inline IsAltLeftZero (_: ref<'T>   when 'T : struct    , _mthd: Default3) = false
    static member inline IsAltLeftZero (_: ref<'T>   when 'T : not struct, _mthd: Default2) = false

    static member inline IsAltLeftZero (t: ref<'At>                               , _mthd: Default1) = (^At : (static member IsAltLeftZero : _ -> _) t.Value)
    static member inline IsAltLeftZero (_: ref< ^t> when ^t: null and ^t: struct , _mthd: Default1) = ()

    static member        IsAltLeftZero (t: ref<option<_>  > , _mthd: IsAltLeftZero) = Option.isSome t.Value
    static member        IsAltLeftZero (t: ref<voption<_>  >, _mthd: IsAltLeftZero) = ValueOption.isSome t.Value

    static member        IsAltLeftZero (t: ref<Result<_,_>> , _mthd: IsAltLeftZero) = match t.Value with (Ok _        ) -> true | _ -> false
    static member        IsAltLeftZero (t: ref<Choice<_,_>> , _mthd: IsAltLeftZero) = match t.Value with (Choice1Of2 _) -> true | _ -> false

    static member inline Invoke (x: 'At) : bool =
        let inline call (mthd : ^M, input: ^I) =
            ((^M or ^I) : (static member IsAltLeftZero : _*_ -> _) (ref input), mthd)
        call(Unchecked.defaultof<IsAltLeftZero>, x)"""
        |> withLangVersion80
        |> asLibrary
        |> compile
        |> verifyIL ["""
   .class nested public auto ansi serializable IsAltLeftZero
        extends _/Default1
    {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = (
            01 00 03 00 00 00 00 00
        )
        // Methods
        .method public static 
            bool IsAltLeftZero<valuetype T> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!T> _arg1,
                class _/Default3 _mthd
            ) cil managed 
        {
            // Method begins at RVA 0x2050
            // Code size 2 (0x2)
            .maxstack 8

            IL_0000: ldc.i4.0
            IL_0001: ret
        } // end of method IsAltLeftZero::IsAltLeftZero

        .method public static 
            bool IsAltLeftZero<class T> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!T> _arg2,
                class _/Default2 _mthd
            ) cil managed 
        {
            // Method begins at RVA 0x2054
            // Code size 2 (0x2)
            .maxstack 8

            IL_0000: ldc.i4.0
            IL_0001: ret
        } // end of method IsAltLeftZero::IsAltLeftZero

        .method public static 
            !!a IsAltLeftZero<At, a> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!At> t,
                class _/Default1 _mthd
            ) cil managed 
        {
            // Method begins at RVA 0x2058
            // Code size 11 (0xb)
            .maxstack 8

            IL_0000: ldstr "Dynamic invocation of IsAltLeftZero is not supported"
            IL_0005: newobj instance void [System.Runtime]System.NotSupportedException::.ctor(string)
            IL_000a: throw
        } // end of method IsAltLeftZero::IsAltLeftZero

        .method public static 
            !!a IsAltLeftZero$W<At, a> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!At, !!a> isAltLeftZero,
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!At> t,
                class _/Default1 _mthd
            ) cil managed 
        {
            // Method begins at RVA 0x2064
            // Code size 15 (0xf)
            .maxstack 8

            IL_0000: ldarg.0
            IL_0001: ldarg.1
            IL_0002: callvirt instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!At>::get_Value()
            IL_0007: tail.
            IL_0009: callvirt instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!At, !!a>::Invoke(!0)
            IL_000e: ret
        } // end of method IsAltLeftZero::IsAltLeftZero$W

        .method public static 
            void IsAltLeftZero<class t> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!t> _arg3,
                class _/Default1 _mthd
            ) cil managed 
        {
            // Method begins at RVA 0x2074
            // Code size 1 (0x1)
            .maxstack 8

            IL_0000: ret
        } // end of method IsAltLeftZero::IsAltLeftZero

        .method public static 
            bool IsAltLeftZero<a> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!!a>> t,
                class _/IsAltLeftZero _mthd
            ) cil managed 
        {
            // Method begins at RVA 0x2078
            // Code size 10 (0xa)
            .maxstack 8

            IL_0000: ldarg.0
            IL_0001: call instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!!a>>::get_contents()
            IL_0006: ldnull
            IL_0007: cgt.un
            IL_0009: ret
        } // end of method IsAltLeftZero::IsAltLeftZero

        .method public static 
            bool IsAltLeftZero<a> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a>> t,
                class _/IsAltLeftZero _mthd
            ) cil managed 
        {
            // Method begins at RVA 0x2084
            // Code size 18 (0x12)
            .maxstack 3
            .locals init (
                [0] valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a>
            )

            IL_0000: ldarg.0
            IL_0001: call instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a>>::get_contents()
            IL_0006: stloc.0
            IL_0007: ldloca.s 0
            IL_0009: call instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!!a>::get_Tag()
            IL_000e: ldc.i4.1
            IL_000f: ceq
            IL_0011: ret
        } // end of method IsAltLeftZero::IsAltLeftZero

        .method public static 
            bool IsAltLeftZero<a, b> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a, !!b>> t,
                class _/IsAltLeftZero _mthd
            ) cil managed 
        {
            // Method begins at RVA 0x20a4
            // Code size 21 (0x15)
            .maxstack 3
            .locals init (
                [0] valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a, !!b>
            )

            IL_0000: ldarg.0
            IL_0001: call instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a, !!b>>::get_contents()
            IL_0006: stloc.0
            IL_0007: ldloca.s 0
            IL_0009: call instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpResult`2<!!a, !!b>::get_Tag()
            IL_000e: ldc.i4.0
            IL_000f: bne.un.s IL_0013

            IL_0011: ldc.i4.1
            IL_0012: ret

            IL_0013: ldc.i4.0
            IL_0014: ret
        } // end of method IsAltLeftZero::IsAltLeftZero

        .method public static 
            bool IsAltLeftZero<a, b> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!!a, !!b>> t,
                class _/IsAltLeftZero _mthd
            ) cil managed 
        {
            // Method begins at RVA 0x20c8
            // Code size 17 (0x11)
            .maxstack 8

            IL_0000: ldarg.0
            IL_0001: call instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!!a, !!b>>::get_contents()
            IL_0006: isinst class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<!!a, !!b>
            IL_000b: brfalse.s IL_000f

            IL_000d: ldc.i4.1
            IL_000e: ret

            IL_000f: ldc.i4.0
            IL_0010: ret
        } // end of method IsAltLeftZero::IsAltLeftZero

        .method public static 
            bool Invoke<At> (
                !!At x
            ) cil managed 
        {
            // Method begins at RVA 0x20dc
            // Code size 13 (0xd)
            .maxstack 3
            .locals init (
                [0] class _/IsAltLeftZero
            )

            IL_0000: ldnull
            IL_0001: stloc.0
            IL_0002: ldstr "Dynamic invocation of IsAltLeftZero is not supported"
            IL_0007: newobj instance void [System.Runtime]System.NotSupportedException::.ctor(string)
            IL_000c: throw
        } // end of method IsAltLeftZero::Invoke

        .method public static 
            bool Invoke$W<At> (
                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!At>, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class _/IsAltLeftZero, bool>> isAltLeftZero,
                !!At x
            ) cil managed 
        {
            // Method begins at RVA 0x20f8
            // Code size 18 (0x12)
            .maxstack 5
            .locals init (
                [0] class _/IsAltLeftZero
            )

            IL_0000: ldnull
            IL_0001: stloc.0
            IL_0002: ldarg.0
            IL_0003: ldarg.1
            IL_0004: call class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<!!At>(!!0)
            IL_0009: ldloc.0
            IL_000a: tail.
            IL_000c: call !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!At>, class _/IsAltLeftZero>::InvokeFast<bool>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1, !!0>>, !0, !1)
            IL_0011: ret
        } // end of method IsAltLeftZero::Invoke$W

    } // end of class IsAltLeftZero
"""]
        
