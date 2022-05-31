// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Debugger

open Xunit
open FSharp.Test.Compiler
open System
open System.IO
open System.Reflection.Metadata
open FSharp.Test

module PortablePdbs =

    [<Fact>]
    let ``Valid Portable PDBs are produced by compiler`` () =
        FSharp """namespace UserNamespace
// Below, comments show where do we expect sequence points ranges to be.
open System

module Foo =
    let getcwd () = Environment.CurrentDirectory
// (6,21)-(6,49) -  ^                           ^
    let _getcwd2 () =    ignore
// (8,26)-(8,32) -       ^     ^

namespace UserNamespace2

open System.IO

module Bar =
    let baz _ =    ()
// (16,20)-(16,22) ^ ^
open System.Collections.Generic

module Baz = 
    let aiou _ =   ()
// (21,20)-(21,22) ^ ^
        """
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [
            VerifyImportScopes [
                [
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp.Core" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp.Collections" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp.Control" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "System" }
                ]
                [
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp.Core" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp.Collections" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp.Control" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "System.IO" }
                ]
                [
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp.Core" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp.Collections" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "Microsoft.FSharp.Control" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "System.IO" }
                    { Kind = ImportDefinitionKind.ImportNamespace; Name = "System.Collections.Generic" }
                ]
            ]
            VerifySequencePoints [ 
                Line 6, Col 21, Line 6, Col 49
                Line 8, Col 26, Line 8, Col 32
                Line 16, Col 20, Line 16, Col 22
                Line 21, Col 20, Line 21, Col 22
            ]
            VerifyDocuments [
                Path.Combine(Environment.CurrentDirectory, "test.fs")
            ]
        ]

    [<Fact>]
    let ``Portable PDBs contain signature files`` () =

        let compilation =
            Fsi """
namespace Foo

module M =
   val f: unit -> int
        """
        compilation
        |> withAdditionalSourceFile (FsSource """
namespace Foo

module M =
   let f () = 1
        """)
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [
            VerifyDocuments [
                Path.Combine(Environment.CurrentDirectory, "test.fsi")
                Path.Combine(Environment.CurrentDirectory, "test.fs")
            ]
        ]
