// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Debugger

open Xunit
open FSharp.Test.Compiler
open System.Reflection.Metadata

module PortablePdbs =

    [<Fact>]
    let ``Valid Portable PDBs are produced by compiler`` () =
        FSharp """
namespace UserNamespace

open System

module Foo =
    let getcwd () = Environment.CurrentDirectory


namespace UserNamespace2

open System.IO

module Bar =
    let baz _ = ()
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
            ]
        ]
