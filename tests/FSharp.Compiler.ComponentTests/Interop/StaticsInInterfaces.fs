// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.ComponentTests.Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module ``Static Methods In Interfaces`` =

    let withCSharpLanguageVersion (ver: CSharpLanguageVersion) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | CS cs -> CS { cs with LangVersion = ver }
        | _ -> failwith "Only supported in C#"

    let csharpBaseClass = 
        CSharp """
    namespace StaticsInInterfaces
    {
        public interface IGetNext<T> where T : IGetNext<T>
        {
            static abstract T Next(T other);
        }

        public interface IGetNext2<T> where T : IGetNext2<T>
        {
            abstract T Next(T other);
        }

        public record RepeatSequence : IGetNext<RepeatSequence>
        {
            private const char Ch = 'A';
            public string Text = new string(Ch, 1);

            public static RepeatSequence Next(RepeatSequence other) => other with { Text = other.Text + Ch };

            public override string ToString() => Text;
        }

    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.Preview |> withName "csLib"

    
    let csharpOperators =
        CSharp """
        namespace StaticsInInterfaces
        {
            public interface IAddable<T> where T : IAddable<T>
            {
                static abstract T operator +(T left, T right);
            }


            public record MyInteger : IAddable<MyInteger>
            {
                public int Value { get; init; } = default;
                public MyInteger(int value)
                {
                    Value = value;
                }

                public static MyInteger operator +(MyInteger left, MyInteger right) => new MyInteger(left.Value + right.Value);
            }

        }
        """  |> withCSharpLanguageVersion CSharpLanguageVersion.Preview |> withName "csOpLib"

    #if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# can use operators declared in C#`` () =

        let fsharpSource =
            """
open System
open StaticsInInterfaces

type MyInteger2 =
    val Value : int
    new(value: int) = { Value = value }
    static member op_Addition(left: MyInteger2, right: MyInteger2) : MyInteger2 = MyInteger2(left.Value + right.Value)
    interface IAddable<MyInteger2> with
        static member op_Addition(left: MyInteger2, right: MyInteger2) : MyInteger2 = MyInteger2.op_Addition(left, right)

[<EntryPoint>]
let main _ =
    let mint1 = new MyInteger(1)
    let mint2 = new MyInteger(2)

    let sum = mint1 + mint2

    let mint21 = new MyInteger2(2)
    let mint22 = new MyInteger2(4)

    let sum2 = mint21 + mint22

    if sum.Value <> 3 then
        failwith $"Unexpected result: %d{sum.Value}"

    if sum2.Value <> 6 then
        failwith $"Unexpected result: %d{sum2.Value}"

    // TODO: Figure out if we allow something like:
    // let add (a: IAddable<_>) (b: IAddable<_>) = a + b
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpOperators]
        |> compileAndRun
        |> shouldSucceed

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# can call static methods declared in interfaces from C#`` () =

        let csharpLib = csharpBaseClass 

        let fsharpSource =
            """
open System
open StaticsInInterfaces

[<EntryPoint>]
let main _ =

    let mutable str = RepeatSequence ()
    let res = [ for i in 0..10 do
                    yield string(str)
                    str <- RepeatSequence.Next(str) ]

    if res <> ["A"; "AA"; "AAA"; "AAAA"; "AAAAA"; "AAAAAA"; "AAAAAAA"; "AAAAAAAA"; "AAAAAAAAA"; "AAAAAAAAAA"; "AAAAAAAAAAA"] then
        failwith $"Unexpected result: %A{res}"

    if string(str) <> "AAAAAAAAAAAA" then
        failwith $"Unexpected result %s{string(str)}"
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compileAndRun
        |> shouldSucceed


    (*  For reference:
        Roslyn generates the following interface:
        .class interface public auto ansi abstract IGetNext`1<(class IGetNext`1<!T>) T>
        {
            // Methods
            .method public hidebysig abstract virtual static 
            !T Next (
                !T other
            ) cil managed 
            {
            } // end of method IGetNext`1::Next

        } // end of class IGetNext`1

        And the following implementation:
        .method public hidebysig static 
        class RepeatSequence Next (class RepeatSequence other) cil managed 
        {
            .override method !0 class IGetNext`1<class RepeatSequence>::Next(!0)
            ...
        }
    *)
    #if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
    #else
    [<Fact>]
    #endif
    let ``F# generates valid IL for abstract static interface methods`` () =

        let csharpLib = csharpBaseClass 

        let fsharpSource =
            """
module StaticsTesting 
open StaticsInInterfaces

type MyRepeatSequence() =
    interface IGetNext<MyRepeatSequence> with
        static member Next(other: MyRepeatSequence) : MyRepeatSequence = other

type MyRepeatSequence2() =
    static member Next(other: MyRepeatSequence2) = other
    interface IGetNext<MyRepeatSequence2> with
        static member Next(other: MyRepeatSequence2) : MyRepeatSequence2 = MyRepeatSequence2.Next(other)
"""
        Fsx fsharpSource
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed
        |> verifyIL [
        """
.class auto ansi serializable nested public MyRepeatSequence
extends [runtime]System.Object
implements class [csLib]StaticsInInterfaces.IGetNext`1<class StaticsTesting/MyRepeatSequence>
    {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
instance void  .ctor() cil managed
    {
          
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ret
    } 
    
    .method public hidebysig static class StaticsTesting/MyRepeatSequence 
'StaticsInInterfaces.IGetNext<StaticsTesting.MyRepeatSequence>.Next'(class StaticsTesting/MyRepeatSequence other) cil managed
    {
        .override  method !0 class [csLib]StaticsInInterfaces.IGetNext`1<class StaticsTesting/MyRepeatSequence>::Next(!0)
          
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ret
    } 
    
    } 
    
    .class auto ansi serializable nested public MyRepeatSequence2
extends [runtime]System.Object
implements class [csLib]StaticsInInterfaces.IGetNext`1<class StaticsTesting/MyRepeatSequence2>
    {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
instance void  .ctor() cil managed
    {
          
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ret
    } 
    
    .method public static class StaticsTesting/MyRepeatSequence2 
Next(class StaticsTesting/MyRepeatSequence2 other) cil managed
    {
          
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ret
    } 
    
    .method public hidebysig static class StaticsTesting/MyRepeatSequence2 
'StaticsInInterfaces.IGetNext<StaticsTesting.MyRepeatSequence2>.Next'(class StaticsTesting/MyRepeatSequence2 other) cil managed
    {
        .override  method !0 class [csLib]StaticsInInterfaces.IGetNext`1<class StaticsTesting/MyRepeatSequence2>::Next(!0)
          
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ret
    } 
    
    }
        """]
    
#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# can implement static methods declared in interfaces from C#`` () =

        let csharpLib = csharpBaseClass 

        let fsharpSource =
            """
open System
open StaticsInInterfaces

type MyRepeatSequence() =
    [<DefaultValue>] val mutable Text : string

    override this.ToString() = this.Text

    static member Next(other: MyRepeatSequence) =
        other.Text <- other.Text + "A"
        other

    interface IGetNext<MyRepeatSequence> with
        static member Next(other: MyRepeatSequence) : MyRepeatSequence = MyRepeatSequence.Next(other)

[<EntryPoint>]
let main _ =

    let mutable str = MyRepeatSequence ()
    str.Text <- "A"
    let res = [ for i in 0..10 do
                    yield str.ToString()
                    str <- MyRepeatSequence.Next(str) ]

    if res <> ["A"; "AA"; "AAA"; "AAAA"; "AAAAA"; "AAAAAA"; "AAAAAAA"; "AAAAAAAA"; "AAAAAAAAA"; "AAAAAAAAAA"; "AAAAAAAAAAA"] then
        failwith $"Unexpected result: %A{res}"

    if string(str) <> "AAAAAAAAAAAA" then
        failwith $"Unexpected result %s{string(str)}"
    0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compileAndRun
        |> shouldSucceed

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# can implement interfaces with static abstract methods`` () =

        let fsharpSource =
            """
#nowarn "3535"
type IAdditionOperator<'T> =
    static abstract op_Addition: 'T * 'T -> 'T

type C() =
    interface IAdditionOperator<C> with
        static member op_Addition(x: C, y: C) = C()

[<EntryPoint>]
let main _ = 0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# supports inference for types of arguments when implementing interfaces`` () =

        let fsharpSource =
            """
#nowarn "3535"
type IAdditionOperator<'T> =
    static abstract op_Addition: 'T * 'T -> 'T

type C() =
    interface IAdditionOperator<C> with
        static member op_Addition(x, y) = C() // no type annotation needed on 'x' and 'y'

[<EntryPoint>]
let main _ = 0
"""
        FSharp fsharpSource
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``F# can call interface with static abstract method`` () =
        FSharp
            """
#nowarn "3535"
namespace Tests

[<assembly: System.Runtime.Versioning.RequiresPreviewFeaturesAttribute()>]
do()

module Test =

    type IAdditionOperator<'T> =
        static abstract op_Addition: 'T * 'T -> 'T

    type C(c: int) =
        member _.Value = c
        interface IAdditionOperator<C> with
            static member op_Addition(x, y) = C(x.Value + y.Value)

    let f<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
        'T.op_Addition(x, y)

    [<EntryPoint>]
    let main _ =
        if f<C>(C(3), C(4)).Value <> 7 then
            failwith "incorrect value"
        0
"""
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
        |> verifyIL [
            """
.class public abstract auto ansi sealed Tests.Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
  .class interface abstract auto ansi serializable nested public IAdditionOperator`1<T>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 )
    .method public hidebysig static abstract virtual
            !T  op_Addition(!T A_0,
                            !T A_1) cil managed
    {
    }

  }

  .class auto ansi serializable nested public C
         extends [runtime]System.Object
         implements class Tests.Test/IAdditionOperator`1<class Tests.Test/C>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 )
    .field assembly int32 c
    .method public specialname rtspecialname
            instance void  .ctor(int32 c) cil managed
    {

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      int32 Tests.Test/C::c
      IL_000f:  ret
    }

    .method public hidebysig specialname
            instance int32  get_Value() cil managed
    {

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Tests.Test/C::c
      IL_0006:  ret
    }

    .method public hidebysig static class Tests.Test/C
            'Tests.Test.IAdditionOperator<Tests.Test.C>.op_Addition'(class Tests.Test/C x,
                                                                     class Tests.Test/C y) cil managed
    {
      .override  method !0 class Tests.Test/IAdditionOperator`1<class Tests.Test/C>::op_Addition(!0,
                                                                                                 !0)

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Tests.Test/C::c
      IL_0006:  ldarg.1
      IL_0007:  ldfld      int32 Tests.Test/C::c
      IL_000c:  add
      IL_000d:  newobj     instance void Tests.Test/C::.ctor(int32)
      IL_0012:  ret
    }

    .property instance int32 Value()
    {
      .get instance int32 Tests.Test/C::get_Value()
    }
  }

  .method public static !!T  f<(class Tests.Test/IAdditionOperator`1<!!T>) T>(!!T x,
                                                                              !!T y) cil managed
  {

    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  constrained. !!T
    IL_0008:  call       !0 class Tests.Test/IAdditionOperator`1<!!T>::op_Addition(!0,
                                                                                   !0)
    IL_000d:  ret
  }

  .method public static int32  main(string[] _arg1) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 )

    .maxstack  4
    .locals init (class Tests.Test/C V_0,
             class Tests.Test/C V_1)
    IL_0000:  ldc.i4.3
    IL_0001:  newobj     instance void Tests.Test/C::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.4
    IL_0008:  newobj     instance void Tests.Test/C::.ctor(int32)
    IL_000d:  stloc.1
    IL_000e:  ldloc.0
    IL_000f:  ldloc.1
    IL_0010:  constrained. Tests.Test/C
    IL_0016:  call       !0 class Tests.Test/IAdditionOperator`1<class Tests.Test/C>::op_Addition(!0,
                                                                                                  !0)
    IL_001b:  ldfld      int32 Tests.Test/C::c
    IL_0020:  ldc.i4.7
    IL_0021:  beq.s      IL_002e

    IL_0023:  ldstr      "incorrect value"
    IL_0028:  newobj     instance void [netstandard]System.Exception::.ctor(string)
    IL_002d:  throw

    IL_002e:  ldc.i4.0
    IL_002f:  ret
  }

}
            """ ]

#if !NETCOREAPP
    [<Fact(Skip = "NET472 is unsupported runtime for this kind of test.")>]
#else
    [<Fact>]
#endif
    let ``C# can call constrained method defined in F#`` () =
        let FSharpLib =
            FSharp """
            namespace MyNamespace

                type IFoo<'T> =
                    static abstract Foo: 'T * 'T -> 'T

                module Stuff =
                    let F<'T when 'T :> IFoo<'T>>(x: 'T, y: 'T) =
                        'T.Foo(x, y)
                """
                |> withLangVersionPreview
                |> withName "FsLibAssembly"
                |> withOptions ["--nowarn:3535"]

        CSharp """
        namespace MyNamespace {

            class Potato : IFoo<Potato>
            {
                public Potato(int x) => this.x = x;

                public int x;

                public static Potato Foo(Potato x, Potato y) => new Potato(x.x + y.x);

                public static void Main(string[] args)
                {
                    var x = new Potato(4);
                    var y = new Potato(9);
                    var z = Stuff.F(x, y);
                    if (z.x != 13)
                    {
                        throw new System.Exception("incorrect value");
                    }
                }
            }
        }
        """
        |> withReferences [FSharpLib]
        |> withName "CsProgram"
        |> compileExeAndRun
        |> shouldSucceed
