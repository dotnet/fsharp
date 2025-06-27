// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.Types

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module TypesAndTypeConstraints_IWSAMsAndSRTPs =

    let typesModule realsig =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__,  "Types.fs")))
        |> withName "Types"
        |> withLangVersion70
        |> withRealInternalSignature realsig
        |> withOptions ["--nowarn:3535"]

    let setupCompilation realsig compilation =
        compilation
        |> asExe
        |> withLangVersion70
        |> withRealInternalSignature realsig
        |> withReferences [typesModule realsig]

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    [<Fact>]
    let ``Srtp call Zero property returns valid result`` () =
        Fsx """
let inline zero<'T when 'T: (static member Zero: 'T)> = 'T.Zero
let result = zero<int>
if result <> 0 then failwith $"Something's wrong: {result}"
        """
        |> runFsi
        |> shouldSucceed

    [<Fact>]
    let ``Srtp call to custom property returns valid result`` () =
        FSharp """
module Foo
type Foo = 
    static member Bar = 1

type HasBar<'T when 'T: (static member Bar: int)> = 'T

let inline bar<'T when HasBar<'T>> =
    'T.Bar

[<EntryPoint>]
let main _ =
    let result = bar<Foo>
    if result <> 0 then
        failwith $"Unexpected result: {result}"
    0
        """
        |> asExe
        |> compileAndRun

#if !NETCOREAPP
    [<Theory(Skip = "IWSAMs are not supported by NET472.")>]
#else
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/testFiles")>]
#endif
    let ``IWSAM test files`` compilation =
        compilation
        |> setupCompilation false
        |> withLangVersionPreview
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
        |> withLangVersion70
        |> signaturesShouldContain expectedSignature

    [<Fact>]
    let ``Static type parameter inference in version 6`` () =
        FSharp """
        let inline f0 (x: ^T) = x
        let g0 (x: 'T) = f0 x"""
        |> withLangVersion60
        |> signaturesShouldContain "val g0: x: obj -> obj"

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

    #if !NETCOREAPP
    [<Theory(Skip = "IWSAMs are not supported by NET472.")>]
    #else   
    [<InlineData("6.0")>]
    [<InlineData("7.0")>]
    [<Theory>]
    #endif
    let ``Extension method on interface without SAM does not produce a warning`` version =
        Fsx """
        type INormalInterface =
            abstract member IntMember: int

        module INormalInterfaceExtensions =
            type INormalInterface with
                static member ExtMethod (a: INormalInterface) =
                    ()
        """
        |> withLangVersion version
        |> compile
        |> shouldSucceed

    [<Theory>]
    [<InlineData("let inline f_TraitWithOptional<'T when 'T : (static member StaticMethod: ?x: int -> int) >() = ()")>]
    [<InlineData("let inline f_TraitWithIn<'T when 'T : (static member StaticMethod: x: inref<int> -> int) >() = ()")>]
    [<InlineData("let inline f_TraitWithOut<'T when 'T : (static member StaticMethod: x: outref<int> -> int) >() = ()")>]
    [<InlineData("let inline f_TraitWithParamArray<'T when 'T : (static member StaticMethod: [<System.ParamArray>] x: int[] -> int) >() = ()")>]
    [<InlineData("let inline f_TraitWithCallerName<'T when 'T : (static member StaticMethod: [<System.Runtime.CompilerServices.CallerMemberNameAttribute>] x: int[] -> int) >() = ()")>]
    [<InlineData("""
        open System.Runtime.InteropServices
        let inline callX2<'T when 'T : (static member X: [<Out>] Name: 'T byref -> bool)> () = ()""")>]
    let ``Trait warning or error`` code =
        let errorMessage = "A trait may not specify optional, in, out, ParamArray, CallerInfo or Quote arguments"

        Fsx code
        |> withLangVersion60
        |> compile
        |> shouldFail
        |> withWarningCode 3532
        |> withDiagnosticMessage errorMessage
        |> ignore

        Fsx code
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withErrorCode 3532
        |> withDiagnosticMessage errorMessage
        |> ignore

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``IWSAM warning`` (realsig) =
        Fsx "let fExpectAWarning(x: Types.ISinOperator<'T>) = (realsig)"
        |> withReferences [typesModule realsig]
        |> withRealInternalSignature realsig
        |> compile
        |> shouldFail
        |> withWarningCode 3536
        |> withDiagnosticMessage """'ISinOperator<_>' is normally used as a type constraint in generic code, e.g. "'T when ISomeInterface<'T>" or "let f (x: #ISomeInterface<_>)". See https://aka.ms/fsharp-iwsams for guidance. You can disable this warning by using '#nowarn "3536"' or '--nowarn:3536'."""
        |> ignore

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Multiple support types trait error`` (realsig) =
        Fsx "let inline f5 (x: 'T when ('T or int) : (static member A: int) ) = 'T.A"
        |> withRealInternalSignature realsig
        |> compile
        |> shouldFail
        |> withErrorCode 3537
        |> withDiagnosticMessage "The trait 'A' invoked by this call has multiple support types. This invocation syntax is not permitted for such traits. See https://aka.ms/fsharp-srtp for guidance."
        |> ignore

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``SRTP Delegate conversion not supported`` (realsig) =
        Fsx "let inline f_TraitWithDelegate<'T when 'T : (static member StaticMethod: x: System.Func<int,int> -> int) >() =
            'T.StaticMethod(fun x -> x + 1)"
        |> withRealInternalSignature realsig
        |> compile
        |> shouldFail
        |> withErrorMessage "This function takes too many arguments, or is used in a context where a function is not expected"

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``SRTP Expression conversion not supported`` (realsig) =
        Fsx "let inline f_TraitWithExpression<'T when 'T : (static member StaticMethod: x: System.Linq.Expressions.Expression<System.Func<int,int>> -> int) >() =
            'T.StaticMethod(fun x -> x + 1)"
        |> withRealInternalSignature realsig
        |> compile
        |> shouldFail
        |> withErrorMessage "This function takes too many arguments, or is used in a context where a function is not expected"

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``IWSAM Delegate conversion works`` (realsig) =
        Fsx
            """
            open Types

            let inline f_IwsamWithFunc<'T when IDelegateConversion<'T>>() =
                'T.FuncConversion(fun x -> x + 1)

            if not (f_IwsamWithFunc<C>().Value = 4) then
                failwith "Unexpected result"

            """
        |> setupCompilation realsig
        |> compileAndRun
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``IWSAM Expression conversion works`` (realsig) =
        Fsx
            """
            open Types

            let inline f_IwsamWithExpression<'T when IDelegateConversion<'T>>() =
                'T.ExpressionConversion(fun x -> x + 1)

            if not (f_IwsamWithExpression<C>().Value = 4) then
                failwith "Unexpected result"

            """
        |> setupCompilation realsig
        |> compileAndRun
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``SRTP Byref can be passed with old syntax`` (realsig) =
        Fsx "let inline f_TraitWithByref<'T when 'T : ( static member TryParse: string * byref<int> -> bool) >() =
                let mutable result = 0
                (^T : ( static member TryParse: x: string * byref<int> -> bool) (\"42\", &result))"
        |> withRealInternalSignature realsig
        |> compile
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``SRTP Byref can be passed with new syntax`` (realsig) =
        Fsx "let inline f_TraitWithByref<'T when 'T : ( static member TryParse: string * byref<int> -> bool) >() =
                let mutable result = 0
                'T.TryParse(\"42\", &result)"
        |> withRealInternalSignature realsig
        |> compile
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Call with old syntax`` (realsig) =
        Fsx """
        type C1() =
            static member X(p: C1 byref) = p

        let inline callX<'T when 'T : (static member X: 'T byref -> 'T)> (x: 'T byref) = (^T: (static member X : 'T byref -> 'T) (&x))

        let mutable c1 = C1()
        let g1 = callX<C1> &c1

        if g1 <> c1 then
            failwith "Unexpected result"
        """
        |> withRealInternalSignature realsig
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Call with new syntax`` (realsig) =
        Fsx """
        type C2() =
            static member X(p: C2 byref) = p

        let inline callX2<'T when 'T : (static member X: 'T byref -> 'T)> (x: 'T byref) = 'T.X &x
        let mutable c2 = C2()
        let g2 = callX2<C2> &c2

        if g2 <> c2 then
            failwith "Unexpected result"
        """
        |> withRealInternalSignature realsig
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Call with tuple`` (realsig) =
        Fsx """

        type C3() =
            static member X(p: C3 byref, n: int) = p

        let inline callX3<'T when 'T : (static member X: 'T byref * int -> 'T)> (x: 'T byref) = 'T.X (&x, 3)
        let mutable c3 = C3()
        let g3 = callX3<C3> &c3

        if g3 <> c3 then
            failwith "Unexpected result"
        """
        |> withRealInternalSignature realsig
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let test4 (realsig) =
        Fsx """
        type C4() =
            static member X() = C4()

        let inline callX4<'T when 'T : (static member X: unit -> 'T)> ()  = 'T.X ()
        let g4 = callX4<C4> ()

        if g4.GetType() <> typeof<C4> then
            failwith "Unexpected result"
        """
        |> withRealInternalSignature realsig
        |> compileExeAndRun
        |> shouldSucceed

    // NOTE: Trait constraints that involve byref returns currently can never be satisfied by any method. No other warning is given.
    // This is a bug that may be fixed in the future.
    // These tests are pinning down current behavior.
    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Byref returns not allowed`` (realsig) =
        Fsx """
        type C5() =
            static member X(p: C5 byref) = &p

        let inline callX5<'T when 'T : (static member X: 'T byref -> 'T byref)> (x: 'T byref)  = 'T.X &x
        let mutable c5 = C5()
        let g5 () = callX5<C5> &c5
        """
        |> withRealInternalSignature realsig
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "This expression was expected to have type\\s+'byref<C5>'\\s+but here has type\\s+'C5'"

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Byref returns not allowed pt 2`` (realsig) =
        Fsx """
        type C6() =
            static member X(p: C6 byref) = &p

        // NOTE: you can declare trait call which returns the address of the thing provided, you just can't satisfy the constraint
        let inline callX6<'T when 'T : (static member X: 'T byref -> 'T byref)> (x: 'T byref)  = &'T.X &x
        let mutable c6 = C6()
        let g6 () = callX6<C6> &c6
        """
        |> withRealInternalSignature realsig
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "This expression was expected to have type\\s+'byref<C6>'\\s+but here has type\\s+'C6'"

    let library realsig=
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
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> withOptions ["--nowarn:3535"]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Function implicit conversion not supported on constrained type`` (realsig) =
        Fsx
            """
            open Lib
            let f_function_implicit_conversion<'T when ICanBeInt<'T>>(a: 'T) : int =
                add1(a)
            """
        |> withReferences [library realsig]
        |> withLangVersion70
        |> withRealInternalSignature realsig
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "This expression was expected to have type\\s+'int'\\s+but here has type\\s+''T'"

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Method implicit conversion not supported on constrained type`` (realsig) =
        Fsx
            """
            open Lib
            let f_method_implicit_conversion<'T when ICanBeInt<'T>>(a: 'T) : int =
                C.TakeInt(a)
            """
        |> withRealInternalSignature realsig
        |> withReferences [library realsig]
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "This expression was expected to have type\\s+'int'\\s+but here has type\\s+''T'"

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Function explicit conversion works on constrained type`` (realsig) =
        Fsx
            """
            open Lib
            let f_function_explicit_conversion<'T when ICanBeInt<'T>>(a: 'T) : int =
                add1(int(a))
            """
        |> withReferences [library realsig]
        |> withLangVersion70
        |> compile
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Method explicit conversion works on constrained type`` (realsig) =
        Fsx
            """
            open Lib
            let f_method_explicit_conversion<'T when ICanBeInt<'T>>(a: 'T) : int =
                C.TakeInt(int(a))
            """
        |> withRealInternalSignature realsig
        |> withReferences [library realsig]
        |> withLangVersion70
        |> compile
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Nominal type can be used after or`` (realsig) =
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
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Nominal type can't be used before or`` (realsig) =
        Fsx
            """
            type C() =
                static member X(n, c) = $"{n} OK"

            let inline callX (x: 'T) (y: C) = ((C or ^T): (static member X : 'T * C -> string) (x, y));;
            """
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "Unexpected keyword 'static' in binding"

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Nominal type is preferred`` (realsig) =
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
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    let library2 realsig =
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
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> withName "Potato"
        |> withOptions ["--nowarn:3535"]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Active patterns- Using IWSAM in active pattern`` (realsig) =
        FSharp """
            module Potato.Test

            open Lib

            let (|GoodPotato|_|) (x : 'T when 'T :> IPotato<'T>) = if 'T.IsGood x then Some () else None

            match Potato() with GoodPotato -> () | _ -> failwith "Unexpected result"
            match Rock() with GoodPotato -> failwith "Unexpected result" | _ -> ()
            """
        |> withReferences [library2 realsig]
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [
            """
             .method public specialname static class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> '|GoodPotato|_|'<(class [Potato]Potato.Lib/IPotato`1<!!T>) T>(!!T x) cil managed
              {

                .maxstack  8
                IL_0000:  ldarg.0
                IL_0001:  constrained. !!T
                IL_0007:  call       bool class [Potato]Potato.Lib/IPotato`1<!!T>::IsGood(!0)
                IL_000c:  brfalse.s  IL_0015
            """
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Active patterns - Using IWSAM equality in active pattern uses generic equality intrinsic`` (realsig) =
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
        |> withReferences [library2 realsig]
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> verifyIL [
            """
            .method public specialname static class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> '|IsEqual|IsNonEqual|'<(class [Potato]Potato.Lib/IPotato`1<!!T>) T>(!!T x, !!T y) cil managed
            {

            .maxstack  8
            IL_0000:  ldarg.0
            IL_0001:  ldarg.1
            IL_0002:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<!!0>(!!0,

            """
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Suppression of System Numerics interfaces on unitized types`` (realsig) =
        Fsx """
            open System.Numerics
            let f (x: 'T when 'T :> IMultiplyOperators<'T,'T,'T>) = x;;
            f 3.0 |> ignore"""
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> compile
        |> shouldSucceed

#if !NETCOREAPP
    [<Theory(Skip = "IWSAMs are not supported by NET472.")>]
#else
    [<Theory>]
    [<InlineData("IAdditionOperators", 3)>]
    [<InlineData("IAdditiveIdentity", 2)>]
    [<InlineData("IBinaryFloatingPointIeee754", 1)>]
    [<InlineData("IBinaryNumber", 1)>]
    [<InlineData("IBitwiseOperators", 3)>]
    [<InlineData("IComparisonOperators", 3)>]
    [<InlineData("IDecrementOperators", 1)>]
    [<InlineData("IDivisionOperators", 3)>]
    [<InlineData("IEqualityOperators", 3)>]
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
#endif
    let ``Unitized type shouldn't be compatible with System_Numerics_I*`` name paramCount =
        let typeParams = Seq.replicate paramCount "'T" |> String.concat ","
        let genericType = $"{name}<{typeParams}>"
        let potatoParams = Seq.replicate paramCount "float<potato>" |> String.concat ","
        let potatoType = $"{name}<{potatoParams}>"
        Fsx $"""
            open System.Numerics

            [<Measure>] type potato

            let f (x: 'T when {genericType}) = x;;
            f 3.0<potato> |> ignore"""
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withErrorMessage $"The type 'float<potato>' is not compatible with the type '{potatoType}'"

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Interface A with static abstracts can be inherited in interface B and then implemented in type C which inherits B in lang version70`` (realsig) =
        Fsx """
            type IParsable<'T when 'T :> IParsable<'T>> =
                static abstract member Parse : string -> 'T

            type IAction<'T when 'T :> IAction<'T>> =
                inherit IParsable<'T>

            type SomeAction = A | B with
                interface IAction<SomeAction> with
                    static member Parse (s: string) : SomeAction =
                        match s with
                        | "A" -> A
                        | "B" -> B
                        | _ -> failwith "can't parse"

            let parse<'T when 'T :> IParsable<'T>> (x: string) : 'T = 'T.Parse x

            if parse<SomeAction> "A" <> A then
                failwith "failed"
        """
        |> withNoWarn 3535
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> compile
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Static abstracts can be inherited through multiple levels in lang version70`` (realsig) =
        Fsx """
            type IParsable<'T when 'T :> IParsable<'T>> =
                static abstract member Parse : string -> 'T

            type IAction1<'T when 'T :> IAction1<'T>> =
                inherit IParsable<'T>

            type IAction2<'T when 'T :> IAction2<'T>> =
                inherit IAction1<'T>
                static abstract member AltParse : string -> 'T

            type IAction3<'T when 'T :> IAction3<'T>> =
                inherit IAction2<'T>

            type SomeAction = A | B with
                interface IAction3<SomeAction> with
                    static member AltParse (s: string) : SomeAction = A
                    static member Parse (s: string) : SomeAction =
                        match s with
                        | "A" -> A
                        | "B" -> B
                        | _ -> failwith "can't parse"

            let parse<'T when 'T :> IParsable<'T>> (x: string) : 'T = 'T.Parse x
            let altParse<'T when 'T :> IAction3<'T>> (x: string) : 'T = 'T.AltParse x

            let x: SomeAction = parse "A"
            let y: SomeAction = altParse "A"

            if x <> A || y <> A then
                failwith "failed"
        """
        |> withNoWarn 3535
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> compile
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Static abstracts from BCL can be inherited through multiple levels in lang version70`` (realsig) =
        Fsx """
            open System
            open System.Globalization

            type Person = Person with
                interface ISpanParsable<Person> with
                    static member Parse(_x: string, _provider: IFormatProvider) = Person
                    static member TryParse(_x: string, _provider: IFormatProvider, _result: byref<Person>) = true

                    static member Parse(_x: ReadOnlySpan<char>, _provider: IFormatProvider) = Person
                    static member TryParse(_x: ReadOnlySpan<char>, _provider: IFormatProvider, _result: byref<Person>) = true

            let parse<'T when 'T :> IParsable<'T>> (x: string) : 'T = 'T.Parse (x, CultureInfo.InvariantCulture)

            let x: Person = parse "Something"
            if x <> Person then
                failwith "failed"
        """
        |> withNoWarn 3535
        |> withRealInternalSignature realsig
        |> withLangVersion70
        |> compile
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Produce an error when one leaves out keyword "static" in an implementation of IWSAM`` (realsig) =
        Fsx """
module StaticAbstractBug =
    type IOperation =
        static abstract member Execute: unit -> unit
        abstract member Execute2: unit -> unit
        static abstract member Property: int
        abstract member Property2: int
        static abstract Property3 : int with get, set

    type FaultyOperation() =
        interface IOperation with
            member _.Execute() = ()
            member _.Execute2() = ()
            member this.Property = 0
            member this.Property2 = 0
            member this.Property3 = 0
            member this.Property3 with set value = ()
        """
        |> withOptions [ "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withDiagnostics [
             (Error 855, Line 12, Col 22, Line 12, Col 29, "No abstract or interface member was found that corresponds to this override")
             (Error 859, Line 14, Col 25, Line 14, Col 33, "No abstract property was found that corresponds to this override")
             (Error 859, Line 16, Col 25, Line 16, Col 34, "No abstract property was found that corresponds to this override")
             (Error 859, Line 17, Col 25, Line 17, Col 34, "No abstract property was found that corresponds to this override")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Produce an error when one leaves out keyword "static" in an implementation of IWSAM with multiple overloads`` (realsig) =
        Fsx """
module StaticAbstractBug =
    type IOperation =
        static abstract member Execute: unit -> unit
        abstract member Execute: unit -> bool
        static abstract member Property: int
        abstract member Property: int

    type FaultyOperation() =
        interface IOperation with
            member _.Execute() = ()
            member _.Execute() = false
            member this.Property = 0
            member this.Property = false
        """
        |> withOptions [ "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 11, Col 34, Line 11, Col 36, "This expression was expected to have type
    'bool'    
but here has type
    'unit'    ")
            (Error 1, Line 14, Col 36, Line 14, Col 41, "This expression was expected to have type
    'int'    
but here has type
    'bool'    ")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Produce an error for interface with static abstract member that is implemented as instance member`` (realsig) =
        Fsx """
module StaticAbstractBug =
    type IFoo<'T> =
       abstract DoIt: unit -> string
       static abstract Other : int -> int
       static abstract member Property: int
       abstract member Property2: int
       static abstract Property3 : int with get, set
    type MyFoo = {
       Value : int
    } with
      interface IFoo<MyFoo> with
        member me.DoIt() = string me.Value
        member _.Other(value) = value + 1
        member this.Property = 0
        member this.Property2 = 0
        member this.Property3 = 0
        member this.Property3 with set value = ()
        """
        |> withOptions [ "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 14, Col 18, Line 14, Col 23, "No abstract or interface member was found that corresponds to this override");   
            (Error 859, Line 15, Col 21, Line 15, Col 29, "No abstract property was found that corresponds to this override");  
            (Error 859, Line 17, Col 21, Line 17, Col 30, "No abstract property was found that corresponds to this override");  
            (Error 859, Line 18, Col 21, Line 18, Col 30, "No abstract property was found that corresponds to this override")
        ]
         
    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Produce an error for interface with static abstract member that is implemented as instance member with multiple overloads`` (realsig) =
        Fsx """
module StaticAbstractBug =
    type IFoo<'T> =
       abstract DoIt: unit -> string
       static abstract Other : int -> int
       abstract Other : int -> bool
       static abstract member Property: int
       abstract member Property: bool
    type MyFoo = {
       Value : int
    } with
      interface IFoo<MyFoo> with
        member me.DoIt() = string me.Value
        member _.Other(value) = value + 1
        member _.Other(value) = value = 1
        member this.Property = 0
        member this.Property = false
        """
        |> withOptions [ "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 41, Line 14, Col 42, "The type 'bool' does not match the type 'int'")
            (Error 1, Line 16, Col 32, Line 16, Col 33, "This expression was expected to have type
    'bool'    
but here has type
    'int'    ")
         ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Produce an error when one leaves out keyword "static" in multiple IWSAM implementations`` (realsig) =
        Fsx """
module StaticAbstractBug =
    type IOperation =
        static abstract member Execute: unit -> int
        abstract member Execute2: unit -> unit
        
    type IOperation2 =
        static abstract member Execute: unit -> int
        abstract member Execute2: unit -> unit

    type FaultyOperation() =
        interface IOperation with
            member this.Execute() = 0
            member _.Execute2() = ()
            
        interface IOperation2 with
            member this.Execute() = 0
            member this.Execute2() = ()
        """
        |> withOptions [ "--nowarn:3535" ]
        |> withLangVersion80
        |> withRealInternalSignature realsig
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 13, Col 25, Line 13, Col 32, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 17, Col 25, Line 17, Col 32, "No abstract or interface member was found that corresponds to this override")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Produces errors when includes keyword "static" when implementing a generic interface in a type`` (realsig) =
        Fsx """
module StaticAbstractBug =
    type IFoo<'T> =
       abstract DoIt: unit -> string
       abstract Other : int -> int
       abstract member Property: int
       abstract member Property2: int
       abstract Property3 : int with get, set
    type MyFoo = {
       Value : int
    } with
      interface IFoo<MyFoo> with
        static member DoIt() = ""
        static member Other(value) = value + 1
        static member Property = 0
        static member Property2 = 0
        static member Property3 = 0
        static member Property3 with set value = ()
        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3855, Line 13, Col 23, Line 13, Col 27, "No static abstract member was found that corresponds to this override")
            (Error 3855, Line 14, Col 23, Line 14, Col 28, "No static abstract member was found that corresponds to this override")
            (Error 3859, Line 15, Col 23, Line 15, Col 31, "No static abstract property was found that corresponds to this override")
            (Error 3859, Line 16, Col 23, Line 16, Col 32, "No static abstract property was found that corresponds to this override")
            (Error 3859, Line 17, Col 23, Line 17, Col 32, "No static abstract property was found that corresponds to this override")
            (Error 3859, Line 18, Col 23, Line 18, Col 32, "No static abstract property was found that corresponds to this override")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Produces errors when includes keyword "static" when implementing an interface in a type`` (realsig) =
        Fsx """
module StaticAbstractBug =
    type IOperation =
        abstract member Execute: unit -> unit
        abstract member Execute: unit -> bool
        abstract member Property: int
        abstract member Property: int

    type FaultyOperation() =
        interface IOperation with
            static member Execute() = ()
            static member Execute() = false
            static member Property = 0
            static member Property = false
        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3855, Line 11, Col 27, Line 11, Col 34, "No static abstract member was found that corresponds to this override")
            (Error 3855, Line 12, Col 27, Line 12, Col 34, "No static abstract member was found that corresponds to this override")
            (Error 3859, Line 13, Col 27, Line 13, Col 35, "No static abstract property was found that corresponds to this override")
            (Error 3859, Line 14, Col 27, Line 14, Col 35, "No static abstract property was found that corresponds to this override")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``No error when implementing interfaces with static members by using class types`` (realsig) =
        Fsx """
type IPrintable =
    abstract member Print: unit -> unit
    static abstract member Log: string -> string

type SomeClass1(x: int, y: float) =
    member this.GetPrint() = (this :> IPrintable).Print()
    
    interface IPrintable with
        member this.Print() = printfn $"%d{x} %f{y}"
        static member Log(s: string) = s
        
let someClass = SomeClass1(1, 2.0) :> IPrintable
let someClass1 = SomeClass1(1, 2.0)

someClass.Print()
someClass1.GetPrint()
        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> typecheck
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``No error when implementing interfaces with static members and IWSAM by using class types`` (realsig) =
        Fsx """
[<Interface>]
type IPrintable =
    static abstract member Log: string -> string
    static member Say(s: string) = s

type SomeClass1() =    
    interface IPrintable with
        static member Log(s: string) = s
        
let someClass1 = SomeClass1()
let execute = IPrintable.Say("hello")
        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> typecheck
        |> shouldSucceed
        
    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Accessing to IWSAM(System.Numerics non virtual) produces a compilation error`` (realsig) =
         Fsx """
open System.Numerics

IAdditionOperators.op_Addition (3, 6)
        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 3866, Line 4, Col 1, Line 4, Col 38, "A static abstract non-virtual interface member should only be called via type parameter (for example: 'T.op_Addition).")

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Accessing to IWSAM(System.Numerics virtual member) compiles and runs`` (realsig) =
         Fsx """
open System.Numerics

let res = IAdditionOperators.op_CheckedAddition (3, 6)

printf "%A" res"""
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> withLangVersion80
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> verifyOutput "9"

#if !NETCOREAPP
    [<Theory(Skip = "IWSAMs are not supported by NET472.")>]
#else
    // SOURCE=ConstrainedAndInterfaceCalls.fs							# ConstrainedAndInterfaceCalls.fs
    [<Theory; FileInlineData("ConstrainedAndInterfaceCalls.fs")>]
#endif
    let ``ConstrainedAndInterfaceCalls.fs`` compilation =
        compilation
        |> getCompilation
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3866, Line 12, Col 82, Line 12, Col 126, "A static abstract non-virtual interface member should only be called via type parameter (for example: 'T.op_Addition).")
            (Error 3866, Line 13, Col 82, Line 13, Col 126, "A static abstract non-virtual interface member should only be called via type parameter (for example: 'T.op_Addition).")
            (Error 3866, Line 15, Col 82, Line 15, Col 129, "A static abstract non-virtual interface member should only be called via type parameter (for example: 'T.Parse).")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Error message that explicitly disallows static abstract methods in abstract classes.`` (realsig) =
        Fsx """
[<AbstractClass>]
type A () =
    static abstract M : unit -> unit
        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3867, Line 4, Col 21, Line 4, Col 22, "Classes cannot contain static abstract members.")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Error message that explicitly disallows static abstract methods in classes.`` (realsig) =
        Fsx """
type A () =
    static abstract M : unit -> unit
        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3867, Line 3, Col 21, Line 3, Col 22, "Classes cannot contain static abstract members.")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Access modifiers cannot be applied to an SRTP constraint in preview`` (realsig) =
        FSharp """
let inline length (x: ^a when ^a: (member public Length: int)) = x.Length
let inline length2 (x: ^a when ^a: (member Length: int with public get)) = x.Length
let inline length3 (x: ^a when ^a: (member Length: int with public set)) = x.set_Length(1)
let inline length4 (x: ^a when ^a: (member public get_Length: unit -> int)) = x.get_Length()
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3871, Line 2, Col 43, Line 2, Col 49, "Access modifiers cannot be applied to an SRTP constraint.")
            (Error 3871, Line 3, Col 61, Line 3, Col 67, "Access modifiers cannot be applied to an SRTP constraint.")
            (Error 3871, Line 4, Col 61, Line 4, Col 67, "Access modifiers cannot be applied to an SRTP constraint.")
            (Error 3871, Line 5, Col 44, Line 5, Col 50, "Access modifiers cannot be applied to an SRTP constraint.")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Access modifiers in an SRTP constraint generate warning in F# 8.0`` (realsig) =
        FSharp """
let inline length (x: ^a when ^a: (member public Length: int)) = x.Length
let inline length2 (x: ^a when ^a: (member Length: int with public get)) = x.Length
let inline length3 (x: ^a when ^a: (member Length: int with public set)) = x.set_Length(1)
let inline length4 (x: ^a when ^a: (member public get_Length: unit -> int)) = x.get_Length()
        """
        |> withLangVersion80
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3871, Line 2, Col 43, Line 2, Col 49, "Access modifiers cannot be applied to an SRTP constraint.")
            (Warning 3871, Line 3, Col 61, Line 3, Col 67, "Access modifiers cannot be applied to an SRTP constraint.")
            (Warning 3871, Line 4, Col 61, Line 4, Col 67, "Access modifiers cannot be applied to an SRTP constraint.")
            (Warning 3871, Line 5, Col 44, Line 5, Col 50, "Access modifiers cannot be applied to an SRTP constraint.")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Error for partial implementation of interface with static abstract members`` (realsig) =
        Fsx """
type IFace =
    static abstract P1 : int
    static abstract P2 : int

type T =
    interface IFace with
        static member P1 = 1

        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 366, Line 7, Col 15, Line 7, Col 20, "No implementation was given for 'static abstract IFace.P2: int'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Error for no implementation of interface with static abstract members`` (realsig) =
        Fsx """
type IFace =
    static abstract P1 : int
    static abstract P2 : int

type T =
    interface IFace with
        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 366, Line 7, Col 15, Line 7, Col 20, "No implementation was given for those members: 
	'static abstract IFace.P1: int'
	'static abstract IFace.P2: int'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Error for partial implementation of interface with static and non static abstract members`` (realsig) =
        Fsx """
type IFace =
    static abstract P1 : int
    static abstract P2 : int
    abstract member P3 : int
    abstract member P4 : int

type T =
    interface IFace with
        static member P1 = 1
        member this.P3 = 3
    
        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 366, Line 9, Col 15, Line 9, Col 20, "No implementation was given for those members: 
	'static abstract IFace.P2: int'
	'abstract IFace.P4: int'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Error for no implementation of interface with static and non static abstract members`` (realsig) =
        Fsx """
type IFace =
    static abstract P1 : int
    static abstract P2 : int
    abstract member P3 : int
    abstract member P4 : int

type T =
    interface IFace with

        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 366, Line 9, Col 15, Line 9, Col 20, "No implementation was given for those members: 
	'static abstract IFace.P1: int'
	'static abstract IFace.P2: int'
	'abstract IFace.P3: int'
	'abstract IFace.P4: int'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Error for partial implementation of interface with non static abstract members`` (realsig) =
        Fsx """
type IFace =
    abstract member P3 : int
    abstract member P4 : int

type T =
    interface IFace with
        member this.P3 = 3

        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 366, Line 7, Col 15, Line 7, Col 20, "No implementation was given for 'abstract IFace.P4: int'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<TheoryForNETCOREAPP>]
    let ``Error for no implementation of interface with non static abstract members`` (realsig) =
        Fsx """
type IFace =
    abstract member P3 : int
    abstract member P4 : int

type T =
    interface IFace with

        """
        |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
        |> withRealInternalSignature realsig
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 366, Line 7, Col 15, Line 7, Col 20, "No implementation was given for those members: 
	'abstract IFace.P3: int'
	'abstract IFace.P4: int'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        ]

