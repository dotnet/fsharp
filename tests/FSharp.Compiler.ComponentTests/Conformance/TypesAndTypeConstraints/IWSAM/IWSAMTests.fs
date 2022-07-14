// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module FSharp.Compiler.ComponentTests.Conformance.TypeAndTypeConstraints.IWSAM

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

let typesModule =
    FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__,  "Types.fs")))
    |> withName "Types"
    |> withLangVersionPreview
    |> withOptions ["--nowarn:3535"]

let setupCompilation compilation =
    compilation
    |> asExe
    |> withLangVersionPreview
    |> withReferences [typesModule]

[<Theory; Directory(__SOURCE_DIRECTORY__ + "/testFiles")>]
let ``IWSAM test files`` compilation =
    compilation
    |> setupCompilation
    |> compileAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData("let inline f0 (x: ^T) = x",
             "val inline f0: x: ^T -> ^T")>]
// TODO: fix this:
//[<InlineData("""
//             let inline f0 (x: ^T) = x
//             let g0 (x: 'T) = f0 x""",
//             "val g0: x: 'T -> 'T")>]

[<InlineData("let inline f1 (x: ^T) = (^T : (static member A: int) ())",
             "val inline f1: x: ^T -> int when ^T: (static member A: int)")>]

[<InlineData("let inline f2 (x: 'T) = ((^T or int) : (static member A: int) ())",
             "val inline f2: x: ^T -> int when (^T or int) : (static member A: int)")>]

[<InlineData("let inline f3 (x: 'T) = ((^U or 'T) : (static member A: int) ())",
             "val inline f3: x: ^T -> int when (^U or ^T) : (static member A: int)")>]

[<InlineData("let inline f4 (x: 'T when 'T : (static member A: int) ) = 'T.A",
             "val inline f4: x: ^T -> int when ^T: (static member A: int)")>]
[<InlineData("""
             let inline f5 (x: ^T) = printfn "%d" x
             let g5 (x: 'T) = f5 x""",
             "val g5: x: int -> unit")>]
[<InlineData("""
             let inline f5 (x: ^T) = printfn "%d" x
             let inline h5 (x: 'T) = f5 x""",
             "val inline h5: x: ^T -> unit when ^T: (byte|int16|int32|int64|sbyte|uint16|uint32|uint64|nativeint|unativeint)")>]
[<InlineData("""
             let inline uint32 (value: ^T) = (^T : (static member op_Explicit: ^T -> uint32) (value))
             let inline uint value = uint32 value""",
             "val inline uint: value: ^a -> uint32 when ^a: (static member op_Explicit: ^a -> uint32)")>]

[<InlineData("let checkReflexive f x y = (f x y = - f y x)",
             "val checkReflexive: f: ('a -> 'a -> int) -> x: 'a -> y: 'a -> bool")>]
let ``Check static type parameter inference`` code expectedSignature =
    FSharp code
    |> ignoreWarnings
    |> signaturesShouldContain expectedSignature


module ``Equivalence of properties and getters`` =

    [<Theory>]
    [<InlineData("let inline f_StaticProperty<'T when 'T : (static member StaticProperty: int) >() = (^T : (static member StaticProperty: int) ())")>]
    [<InlineData("let inline f_StaticProperty<'T when 'T : (static member get_StaticProperty: unit -> int) >() = (^T : (static member get_StaticProperty: unit -> int) ())")>]
    [<InlineData("let inline f_StaticProperty<'T when 'T : (static member get_StaticProperty: unit -> int) >() = (^T : (static member StaticProperty: int) ())")>]
    [<InlineData("let inline f_StaticProperty<'T when 'T : (static member StaticProperty: int) >() = (^T : (static member get_StaticProperty: unit -> int) ())")>]
    let ``Static property getter`` code =
        Fsx code
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
          .method public static int32  f_StaticProperty<T>() cil managed
          {

            .maxstack  8
            IL_0000:  ldstr      "Dynamic invocation of get_StaticProperty is not su"
            + "pported"
            IL_0005:  newobj     instance void [runtime]System.NotSupportedException::.ctor(string)
            IL_000a:  throw
          }

          .method public static int32  f_StaticProperty$W<T>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> get_StaticProperty) cil managed
          {

            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  ldnull
            IL_0002:  tail.
            IL_0004:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
            IL_0009:  ret
          }"""]

    [<Theory>]
    [<InlineData("let inline f_set_StaticProperty<'T when 'T : (static member StaticProperty: int with set) >() = (^T : (static member StaticProperty: int with set) (3))")>]
    [<InlineData("let inline f_set_StaticProperty<'T when 'T : (static member set_StaticProperty: int -> unit) >() = (^T : (static member set_StaticProperty: int -> unit) (3))")>]
    [<InlineData("let inline f_set_StaticProperty<'T when 'T : (static member set_StaticProperty: int -> unit) >() = (^T : (static member StaticProperty: int with set) (3))")>]
    [<InlineData("let inline f_set_StaticProperty<'T when 'T : (static member StaticProperty: int with set) >() = (^T : (static member set_StaticProperty: int -> unit) (3))")>]
    let ``Static property setter`` code =
        Fsx code
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
          .method public static void  f_set_StaticProperty<T>() cil managed
          {

            .maxstack  8
            IL_0000:  ldstr      "Dynamic invocation of set_StaticProperty is not su"
            + "pported"
            IL_0005:  newobj     instance void [runtime]System.NotSupportedException::.ctor(string)
            IL_000a:  throw
          }

          .method public static void  f_set_StaticProperty$W<T>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> set_StaticProperty) cil managed
          {

            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  ldc.i4.3
            IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
            IL_0007:  pop
            IL_0008:  ret
          }"""]

    [<Theory>]
    [<InlineData("let inline f_Length<'T when 'T : (member Length: int) >(x: 'T) = (^T : (member Length: int) (x))")>]
    [<InlineData("let inline f_Length<'T when 'T : (member get_Length: unit -> int) >(x: 'T) = (^T : (member get_Length: unit -> int) (x))")>]
    [<InlineData("let inline f_Length<'T when 'T : (member get_Length: unit -> int) >(x: 'T) = (^T : (member Length: int) (x))")>]
    [<InlineData("let inline f_Length<'T when 'T : (member Length: int) >(x: 'T) = (^T : (member get_Length: unit -> int) (x))")>]
    let ``Instance property getter`` code =
        Fsx code
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
          .method public static int32  f_Length<T>(!!T x) cil managed
          {

            .maxstack  8
            IL_0000:  ldstr      "Dynamic invocation of get_Length is not supported"
            IL_0005:  newobj     instance void [runtime]System.NotSupportedException::.ctor(string)
            IL_000a:  throw
          }

          .method public static int32  f_Length$W<T>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!T,int32> get_Length,
                                                     !!T x) cil managed
          {

            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  ldarg.1
            IL_0002:  tail.
            IL_0004:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!T,int32>::Invoke(!0)
            IL_0009:  ret
          }"""]

    [<Theory>]
    [<InlineData("let inline f_set_Length<'T when 'T : (member Length: int with set) >(x: 'T) = (^T : (member Length: int with set) (x, 3))")>]
    [<InlineData("let inline f_set_Length<'T when 'T : (member set_Length: int -> unit) >(x: 'T) = (^T : (member set_Length: int -> unit) (x, 3))")>]
    [<InlineData("let inline f_set_Length<'T when 'T : (member set_Length: int -> unit) >(x: 'T) = (^T : (member Length: int with set) (x, 3))")>]
    [<InlineData("let inline f_set_Length<'T when 'T : (member Length: int with set) >(x: 'T) = (^T : (member set_Length: int -> unit) (x, 3))")>]
    let ``Instance property setter`` code =
        Fsx code
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
         .method public static void  f_set_Length<T>(!!T x) cil managed
          {

            .maxstack  8
            IL_0000:  ldstr      "Dynamic invocation of set_Length is not supported"
            IL_0005:  newobj     instance void [runtime]System.NotSupportedException::.ctor(string)
            IL_000a:  throw
          }

          .method public static void  f_set_Length$W<T>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!T,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> set_Length,
                                                        !!T x) cil managed
          {

            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  ldarg.1
            IL_0002:  ldc.i4.3
            IL_0003:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,int32>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                 !0,
                                                                                                                                                                 !1)
            IL_0008:  pop
            IL_0009:  ret
          }"""]

    [<Theory>]
    [<InlineData("let inline f_Item<'T when 'T : (member Item: int -> string with get) >(x: 'T) = (^T : (member Item: int -> string with get) (x, 3))")>]
    [<InlineData("let inline f_Item<'T when 'T : (member get_Item: int -> string) >(x: 'T) = (^T : (member get_Item: int -> string) (x, 3))")>]
    [<InlineData("let inline f_Item<'T when 'T : (member get_Item: int -> string) >(x: 'T) = (^T : (member Item: int -> string with get) (x, 3))")>]
    [<InlineData("let inline f_Item<'T when 'T : (member Item: int -> string with get) >(x: 'T) = (^T : (member get_Item: int -> string) (x, 3))")>]
    let ``Get item`` code =
        Fsx code
        |> withOptions ["--nowarn:77"]
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
         .method public static string  f_Item<T>(!!T x) cil managed
          {

            .maxstack  8
            IL_0000:  ldstr      "Dynamic invocation of get_Item is not supported"
            IL_0005:  newobj     instance void [runtime]System.NotSupportedException::.ctor(string)
            IL_000a:  throw
          }

          .method public static string  f_Item$W<T>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!T,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>> get_Item,
                                                    !!T x) cil managed
          {

            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  ldarg.1
            IL_0002:  ldc.i4.3
            IL_0003:  tail.
            IL_0005:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,int32>::InvokeFast<string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                          !0,
                                                                                                                          !1)
            IL_000a:  ret
          }"""]

    [<Theory>]
    [<InlineData("let inline f_set_Item<'T when 'T : (member Item: int -> string with set) >(x: 'T) = (^T : (member Item: int -> string with set) (x, 3, \"a\"))")>]
    [<InlineData("let inline f_set_Item<'T when 'T : (member set_Item: int * string -> unit) >(x: 'T) = (^T : (member set_Item: int * string -> unit) (x, 3, \"a\"))")>]
    [<InlineData("let inline f_set_Item<'T when 'T : (member set_Item: int * string -> unit) >(x: 'T) = (^T : (member Item: int -> string with set) (x, 3, \"a\"))")>]
    [<InlineData("let inline f_set_Item<'T when 'T : (member Item: int -> string with set) >(x: 'T) = (^T : (member set_Item: int * string -> unit) (x, 3, \"a\"))")>]
    let ``Set item`` code =
        Fsx code
        |> withOptions ["--nowarn:77"]
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
          .method public static void  f_set_Item<T>(!!T x) cil managed
          {

            .maxstack  8
            IL_0000:  ldstr      "Dynamic invocation of set_Item is not supported"
            IL_0005:  newobj     instance void [runtime]System.NotSupportedException::.ctor(string)
            IL_000a:  throw
          }

          .method public static void  f_set_Item$W<T>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!T,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>> set_Item,
                                                      !!T x) cil managed
          {

            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  ldarg.1
            IL_0002:  ldc.i4.3
            IL_0003:  ldstr      "a"
            IL_0008:  call       !!1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,int32>::InvokeFast<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>>>,
                                                                                                                                                                        !0,
                                                                                                                                                                        !1,
                                                                                                                                                                        !!0)
            IL_000d:  pop
            IL_000e:  ret
          }"""]


module Negative =

    [<Theory>]
    [<InlineData("let inline f_TraitWithOptional<'T when 'T : (static member StaticMethod: ?x: int -> int) >() = ()")>]
    [<InlineData("let inline f_TraitWithIn<'T when 'T : (static member StaticMethod: x: inref<int> -> int) >() = ()")>]
    [<InlineData("let inline f_TraitWithOut<'T when 'T : (static member StaticMethod: x: outref<int> -> int) >() = ()")>]
    [<InlineData("let inline f_TraitWithParamArray<'T when 'T : (static member StaticMethod: [<System.ParamArray>] x: int[] -> int) >() = ()")>]
    [<InlineData("let inline f_TraitWithCallerName<'T when 'T : (static member StaticMethod: [<System.Runtime.CompilerServices.CallerMemberNameAttribute>] x: int[] -> int) >() = ()")>]
// TODO: fix this:    [<InlineData("let inline f_TraitWithExpression<'T when 'T : (static member StaticMethod: x: System.Linq.Expressions.Expression<System.Func<int,int>> -> int) >() = ()")>]
    let ``Trait warning`` code =
        Fsx code
        |> compile
        |> shouldFail
        |> withWarningCode 3532
        |> withDiagnosticMessage "A trait may not specify optional, in, out, ParamArray, CallerInfo or Quote arguments"
        |> ignore

    [<Fact>]
    let ``IWSAM waring`` () =
        Fsx "let fExpectAWarning(x: Types.ISinOperator<'T>) = ()"
        |> withReferences [typesModule]
        |> compile
        |> shouldFail
        |> withWarningCode 3536
        |> withDiagnosticMessage """'ISinOperator<_>' is normally used as a type constraint in generic code, e.g. "'T when ISomeInterface<'T>" or "let f (x: #ISomeInterface<_>)". See https://aka.ms/fsharp-iwsams for guidance. You can disable this warning by using '#nowarn "3536"' or '--nowarn:3536'."""
        |> ignore

    [<Fact>]
    let ``Multiple support types trait error`` () =
        Fsx "let inline f5 (x: 'T when ('T or int) : (static member A: int) ) = 'T.A"
        |> compile
        |> shouldFail
        |> withErrorCode 3537
        |> withDiagnosticMessage "The trait 'A' invoked by this call has multiple support types. This invocation syntax is not permitted for such traits. See https://aka.ms/fsharp-srtp for guidance."
        |> ignore
