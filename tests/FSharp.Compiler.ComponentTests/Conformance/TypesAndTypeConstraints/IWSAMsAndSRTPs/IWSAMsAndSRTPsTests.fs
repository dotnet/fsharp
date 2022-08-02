// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module FSharp.Compiler.ComponentTests.Conformance.TypeAndTypeConstraints.IWSAMsAndSRTPs

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


#if !NETCOREAPP
[<Theory(Skip = "IWSAMs are not supported by NET472.")>]
#else
[<Theory; Directory(__SOURCE_DIRECTORY__ + "/testFiles")>]
#endif
let ``IWSAM test files`` compilation =
    compilation
    |> setupCompilation
    |> compileAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData("let inline f0 (x: ^T) = x",
             "val inline f0: x: ^T -> ^T")>]
[<InlineData("""
             let inline f0 (x: ^T) = x
             let g0 (x: 'T) = f0 x""",
             "val g0: x: 'T -> 'T")>]

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
    |> withLangVersionPreview
    |> signaturesShouldContain expectedSignature


[<Fact>]
let ``Static type parameter inference in version 6`` () =
    FSharp """
        let inline f0 (x: ^T) = x
        let g0 (x: 'T) = f0 x"""
    |> withLangVersion60
    |> signaturesShouldContain "val g0: x: obj -> obj"


module ``Equivalence of properties and getters`` =

    [<Theory>]
    [<InlineData("let inline f_StaticProperty<'T when 'T : (static member StaticProperty: int) >() = (^T : (static member StaticProperty: int) ())")>]
    [<InlineData("let inline f_StaticProperty<'T when 'T : (static member get_StaticProperty: unit -> int) >() = (^T : (static member get_StaticProperty: unit -> int) ())")>]
    [<InlineData("let inline f_StaticProperty<'T when 'T : (static member get_StaticProperty: unit -> int) >() = (^T : (static member StaticProperty: int) ())")>]
    [<InlineData("let inline f_StaticProperty<'T when 'T : (static member StaticProperty: int) >() = (^T : (static member get_StaticProperty: unit -> int) ())")>]
    [<InlineData("let inline f_StaticProperty<'T when 'T : (static member StaticProperty: int) >() = 'T.StaticProperty")>]
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
    [<InlineData("let inline f_set_StaticProperty<'T when 'T : (static member StaticProperty: int with set) >() = 'T.set_StaticProperty(3)")>]
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
    [<InlineData("let inline f_Length<'T when 'T : (member Length: int) >(x: 'T) = x.Length")>]
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
    [<InlineData("let inline f_set_Length<'T when 'T : (member Length: int with set) >(x: 'T) = x.set_Length(3)")>]
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
    [<InlineData("let inline f_Item<'T when 'T : (member Item: int -> string with get) >(x: 'T) = x.get_Item(3)")>]
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
    [<InlineData("let inline f_set_Item<'T when 'T : (member Item: int -> string with set) >(x: 'T) = x.set_Item(3, \"a\")")>]
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
    let ``Trait warning`` code =
        Fsx code
        |> compile
        |> shouldFail
        |> withWarningCode 3532
        |> withDiagnosticMessage "A trait may not specify optional, in, out, ParamArray, CallerInfo or Quote arguments"
        |> ignore

    #if !NETCOREAPP
    [<Fact(Skip = "IWSAMs are not supported by NET472.")>]
    #else
    [<Fact>]
    #endif
    let ``IWSAM warning`` () =
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


module InvocationBehavior =

    [<Fact>]
    let ``SRTP Delegate conversion not supported`` () =
        Fsx "let inline f_TraitWithDelegate<'T when 'T : (static member StaticMethod: x: System.Func<int,int> -> int) >() =
            'T.StaticMethod(fun x -> x + 1)"
        |> compile
        |> shouldFail
        |> withErrorMessage "This function takes too many arguments, or is used in a context where a function is not expected"

    [<Fact>]
    let ``SRTP Expression conversion not supported`` () =
        Fsx "let inline f_TraitWithExpression<'T when 'T : (static member StaticMethod: x: System.Linq.Expressions.Expression<System.Func<int,int>> -> int) >() =
            'T.StaticMethod(fun x -> x + 1)"
        |> compile
        |> shouldFail
        |> withErrorMessage "This function takes too many arguments, or is used in a context where a function is not expected"

    #if !NETCOREAPP
    [<Fact(Skip = "IWSAMs are not supported by NET472.")>]
    #else
    [<Fact>]
    #endif
    let ``IWSAM Delegate conversion works`` () =
        Fsx
            """
            open Types

            let inline f_IwsamWithFunc<'T when IDelegateConversion<'T>>() =
                'T.FuncConversion(fun x -> x + 1)

            if not (f_IwsamWithFunc<C>().Value = 4) then
                failwith "Unexpected result"

            """
        |> setupCompilation
        |> compileAndRun
        |> shouldSucceed

    #if !NETCOREAPP
    [<Fact(Skip = "IWSAMs are not supported by NET472.")>]
    #else
    [<Fact>]
    #endif
    let ``IWSAM Expression conversion works`` () =
        Fsx
            """
            open Types

            let inline f_IwsamWithExpression<'T when IDelegateConversion<'T>>() =
                'T.ExpressionConversion(fun x -> x + 1)

            if not (f_IwsamWithExpression<C>().Value = 4) then
                failwith "Unexpected result"

            """
        |> setupCompilation
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``SRTP Byref can be passed with old syntax`` () =
        Fsx "let inline f_TraitWithByref<'T when 'T : ( static member TryParse: string * byref<int> -> bool) >() =
                let mutable result = 0
                (^T : ( static member TryParse: x: string * byref<int> -> bool) (\"42\", &result))"
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``SRTP Byref can be passed with new syntax`` () =
        Fsx "let inline f_TraitWithByref<'T when 'T : ( static member TryParse: string * byref<int> -> bool) >() =
                let mutable result = 0
                'T.TryParse(\"42\", &result)"
        |> compile
        |> shouldSucceed


module ``SRTP byref tests`` =

    [<Fact>]
    let ``Call with old syntax`` () =
        Fsx """
        type C1() =
            static member X(p: C1 byref) = p

        let inline callX<'T when 'T : (static member X: 'T byref -> 'T)> (x: 'T byref) = (^T: (static member X : 'T byref -> 'T) (&x))

        let mutable c1 = C1()
        let g1 = callX<C1> &c1

        if g1 <> c1 then
            failwith "Unexpected result"
        """
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Call with new syntax`` () =
        Fsx """
        type C2() =
            static member X(p: C2 byref) = p

        let inline callX2<'T when 'T : (static member X: 'T byref -> 'T)> (x: 'T byref) = 'T.X &x
        let mutable c2 = C2()
        let g2 = callX2<C2> &c2

        if g2 <> c2 then
            failwith "Unexpected result"
        """
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Call with tuple`` () =
        Fsx """

        type C3() =
            static member X(p: C3 byref, n: int) = p

        let inline callX3<'T when 'T : (static member X: 'T byref * int -> 'T)> (x: 'T byref) = 'T.X (&x, 3)
        let mutable c3 = C3()
        let g3 = callX3<C3> &c3

        if g3 <> c3 then
            failwith "Unexpected result"
        """
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let test4 () =
        Fsx """
        type C4() =
            static member X() = C4()

        let inline callX4<'T when 'T : (static member X: unit -> 'T)> ()  = 'T.X ()
        let g4 = callX4<C4> ()

        if g4.GetType() <> typeof<C4> then
            failwith "Unexpected result"
        """
        |> compileExeAndRun
        |> shouldSucceed

    // Trait constraints that involve byref returns currently can never be satisfied by any method. No other warning is given.
    [<Fact>]
    let ``Byref returns not allowed`` () =
        Fsx """
        type C5() =
            static member X(p: C5 byref) = &p

        let inline callX5<'T when 'T : (static member X: 'T byref -> 'T byref)> (x: 'T byref)  = 'T.X &x
        let mutable c5 = C5()
        let g5 () = callX5<C5> &c5
        """
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "This expression was expected to have type\\s+'byref<C5>'\\s+but here has type\\s+'C5'"

    [<Fact>]
    let ``Byref returns not allowed pt. 2`` () =
        Fsx """
        type C6() =
            static member X(p: C6 byref) = &p

        // NOTE: you can declare trait call which returns the address of the thing provided, you just can't satisfy the constraint
        let inline callX6<'T when 'T : (static member X: 'T byref -> 'T byref)> (x: 'T byref)  = &'T.X &x
        let mutable c6 = C6()
        let g6 () = callX6<C6> &c6
        """
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "This expression was expected to have type\\s+'byref<C6>'\\s+but here has type\\s+'C6'"

    [<Fact>]
    let ``No out args allowed`` () =
        Fsx """
        open System.Runtime.InteropServices

        let inline callX2<'T when 'T : (static member X: [<Out>] Name: 'T byref -> bool)> () = ()
        """
        |> compile
        |> shouldFail
        |> withDiagnosticMessage "A trait may not specify optional, in, out, ParamArray, CallerInfo or Quote arguments"


module ``Implicit conversion`` =

    let library =
        FSharp
            """
            module Lib

                type ICanBeInt<'T when 'T :> ICanBeInt<'T>> =
                    static abstract op_Implicit: 'T -> int

                type C(c: int) =
                    member _.Value = c

                    interface ICanBeInt<C> with
                        static member op_Implicit(x) = x.Value

                    static member TakeInt(x: int) = x

                let add1 (x: int) = x + 1
            """
        |> withLangVersionPreview
        |> withOptions ["--nowarn:3535"]

    #if !NETCOREAPP
    [<Fact(Skip = "IWSAMs are not supported by NET472.")>]
    #else
    [<Fact>]
    #endif
    let ``Function implicit conversion not supported on constrained type`` () =
        Fsx
            """
            open Lib
            let f_function_implicit_conversion<'T when ICanBeInt<'T>>(a: 'T) : int =
                add1(a)
            """
        |> withReferences [library]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "This expression was expected to have type\\s+'int'\\s+but here has type\\s+''T'"

    #if !NETCOREAPP
    [<Fact(Skip = "IWSAMs are not supported by NET472.")>]
    #else
    [<Fact>]
    #endif
    let ``Method implicit conversion not supported on constrained type`` () =
        Fsx
            """
            open Lib
            let f_method_implicit_conversion<'T when ICanBeInt<'T>>(a: 'T) : int =
                C.TakeInt(a)
            """
        |> withReferences [library]
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "This expression was expected to have type\\s+'int'\\s+but here has type\\s+''T'"

    #if !NETCOREAPP
    [<Fact(Skip = "IWSAMs are not supported by NET472.")>]
    #else
    [<Fact>]
    #endif
    let ``Function explicit conversion works on constrained type`` () =
        Fsx
            """
            open Lib
            let f_function_explicit_conversion<'T when ICanBeInt<'T>>(a: 'T) : int =
                add1(int(a))
            """
        |> withReferences [library]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    #if !NETCOREAPP
    [<Fact(Skip = "IWSAMs are not supported by NET472.")>]
    #else
    [<Fact>]
    #endif
    let ``Method explicit conversion works on constrained type`` () =
        Fsx
            """
            open Lib
            let f_method_explicit_conversion<'T when ICanBeInt<'T>>(a: 'T) : int =
                C.TakeInt(int(a))
            """
        |> withReferences [library]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed


module ``Nominal type after or`` =

    [<Fact>]
    let ``Nominal type can be used after or`` () =
        Fsx
            """
            type C() =
                static member X(n, c) = $"{n} OK"

            let inline callX (x: 'T) (y: C) = ((^T or C): (static member X : 'T * C -> string) (x, y));;

            if not (callX 1 (C()) = "1 OK") then
                failwith "Unexpected result"

            if not (callX "A" (C()) = "A OK") then
                failwith "Unexpected result"
            """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Nominal type can't be used before or`` () =
        Fsx
            """
            type C() =
                static member X(n, c) = $"{n} OK"

            let inline callX (x: 'T) (y: C) = ((C or ^T): (static member X : 'T * C -> string) (x, y));;
            """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "Unexpected keyword 'static' in binding"

    [<Fact>]
    let ``Nominal type is preferred`` () =
        Fsx
            """
            type C() =
                static member X(a, b) = "C"

            type D() =
                static member X(d: D, a) = "D"

            let inline callX (x: 'T) (y: C) = ((^T or C): (static member X : 'T * C -> string) (x, y));;

            if not (callX (D()) (C()) = "C") then
                failwith "Unexpected result"

            let inline callX2 (x: C) (y: 'T) = ((^T or C): (static member X : 'T * C -> string) (y, x));;

            if not (callX2 (C()) (D()) = "C") then
                failwith "Unexpected result"
            """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed

module ``Active patterns`` =

    let library =
        FSharp """
        module Potato.Lib
            type IPotato<'T when 'T :> IPotato<'T>> =
                static abstract member IsGood: 'T -> bool
                static abstract member op_Equality: 'T * 'T -> bool

            type Potato() =
                interface IPotato<Potato> with
                    static member IsGood c = true
                    static member op_Equality (a, b) = false

            type Rock() =
                interface IPotato<Rock> with
                    static member IsGood c = false
                    static member op_Equality (a, b) = false
            """
        |> withLangVersionPreview
        |> withName "Potato"
        |> withOptions ["--nowarn:3535"]

    #if !NETCOREAPP
    [<Fact(Skip = "IWSAMs are not supported by NET472.")>]
    #else
    [<Fact>]
    #endif
    let ``Using IWSAM in active pattern`` () =
        FSharp """
            module Potato.Test

            open Lib

            let (|GoodPotato|_|) (x : 'T when 'T :> IPotato<'T>) = if 'T.IsGood x then Some () else None

            match Potato() with GoodPotato -> () | _ -> failwith "Unexpected result"
            match Rock() with GoodPotato -> failwith "Unexpected result" | _ -> ()
            """
        |> withReferences [library]
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [
            """
             .method public specialname static class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>
                      '|GoodPotato|_|'<(class [Potato]Potato.Lib/IPotato`1<!!T>) T>(!!T x) cil managed
              {

                .maxstack  8
                IL_0000:  ldarg.0
                IL_0001:  constrained. !!T
                IL_0007:  call       bool class [Potato]Potato.Lib/IPotato`1<!!T>::IsGood(!0)
                IL_000c:  brfalse.s  IL_0015
            """
        ]

    #if !NETCOREAPP
    [<Fact(Skip = "IWSAMs are not supported by NET472.")>]
    #else
    [<Fact>]
    #endif
    let ``Using IWSAM equality in active pattern uses generic equality intrinsic`` () =
        FSharp """
            module Potato.Test

            open Lib

            let (|IsEqual|IsNonEqual|) (x: 'T when IPotato<'T>, y: 'T when IPotato<'T>) =
                match x with
                | x when x = y -> IsEqual
                | _ -> IsNonEqual

            match Potato(), Potato() with
            | IsEqual -> failwith "Unexpected result"
            | IsNonEqual -> ()
            """
        |> withReferences [library]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> verifyIL [
            """
            .method public specialname static class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
                  '|IsEqual|IsNonEqual|'<(class [Potato]Potato.Lib/IPotato`1<!!T>) T>(!!T x,
                                                                                      !!T y) cil managed
            {

            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  ldarg.1
            IL_0002:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<!!0>(!!0,

            """
        ]

module ``Suppression of System Numerics interfaces on unitized types`` =

    [<Fact(Skip = "Solution needs to be updated to .NET 7")>]
    let Baseline () =
        Fsx """
            open System.Numerics
            let f (x: 'T when 'T :> IMultiplyOperators<'T,'T,'T>) = x;;
            f 3.0 |> ignore"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Theory(Skip = "Solution needs to be updated to .NET 7")>]
    [<InlineData("IAdditionOperators", 3)>]
    [<InlineData("IAdditiveIdentity", 2)>]
    [<InlineData("IBinaryFloatingPointIeee754", 1)>]
    [<InlineData("IBinaryNumber", 1)>]
    [<InlineData("IBitwiseOperators", 3)>]
    [<InlineData("IComparisonOperators", 2)>]
    [<InlineData("IDecrementOperators", 1)>]
    [<InlineData("IDivisionOperators", 3)>]
    [<InlineData("IEqualityOperators", 2)>]
    [<InlineData("IExponentialFunctions", 1)>]
    [<InlineData("IFloatingPoint", 1)>]
    [<InlineData("IFloatingPointIeee754", 1)>]
    [<InlineData("IHyperbolicFunctions", 1)>]
    [<InlineData("IIncrementOperators", 1)>]
    [<InlineData("ILogarithmicFunctions", 1)>]
    [<InlineData("IMinMaxValue", 1)>]
    [<InlineData("IModulusOperators", 3)>]
    [<InlineData("IMultiplicativeIdentity", 2)>]
    [<InlineData("IMultiplyOperators", 3)>]
    [<InlineData("INumber", 1)>]
    [<InlineData("INumberBase", 1)>]
    [<InlineData("IPowerFunctions", 1)>]
    [<InlineData("IRootFunctions", 1)>]
    [<InlineData("ISignedNumber", 1)>]
    [<InlineData("ISubtractionOperators", 3)>]
    [<InlineData("ITrigonometricFunctions", 1)>]
    [<InlineData("IUnaryNegationOperators", 2)>]
    [<InlineData("IUnaryPlusOperators", 2)>]
    let ``Unitized type shouldn't be compatible with System.Numerics.I*`` name paramCount =
        let typeParams = Seq.replicate paramCount "'T" |> String.concat ","
        let genericType = $"{name}<{typeParams}>"
        let potatoParams = Seq.replicate paramCount "float<potato>" |> String.concat ","
        let potatoType = $"{name}<{potatoParams}>"
        Fsx $"""
            open System.Numerics

            [<Measure>] type potato

            let f (x: 'T when {genericType}) = x;;
            f 3.0<potato> |> ignore"""
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorMessage $"The type 'float<potato>' is not compatible with the type '{potatoType}'"
