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
        public record RepeatSequence : IGetNext<RepeatSequence>
        {
            private const char Ch = 'A';
            public string Text = new string(Ch, 1);

            public static RepeatSequence Next(RepeatSequence other) => other with { Text = other.Text + Ch };

            public override string ToString() => Text;
        }

    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.Preview |> withName "csLib"

    
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
"""
        Fsx fsharpSource
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed
        |> verifyIL [
        """
.class public abstract auto ansi sealed StaticsTesting
extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
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
    interface IGetNext<MyRepeatSequence> with
        static member Next(other: MyRepeatSequence) : MyRepeatSequence = other 

[<EntryPoint>]
let main _ =

    let mutable str = MyRepeatSequence ()
    let res = [ for i in 0..10 do
                    yield string(str)
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