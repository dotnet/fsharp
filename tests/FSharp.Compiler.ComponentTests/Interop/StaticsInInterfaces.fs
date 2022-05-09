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