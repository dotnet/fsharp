// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.Diagnostics
open NUnit.Framework
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Test.Compiler

[<TestFixture>]
module SignatureGenerationTests =

    let sigText (checkResults: FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults) =
        match checkResults.GenerateSignature() with
        | None -> failwith "Unable to generate signature text."
        | Some text -> text

    let sigShouldBe (expected: string) src =
        let text =
            FSharp src
            |> withLangVersion50
            |> typecheckResults
            |> sigText

        let actual =
            text.ToString()
            |> fun s -> s.Split('\n')
            |> Array.map (fun s -> s.TrimEnd(' '))

        printfn $"actual is\n-------\n{text.ToString()}\n---------"

        let expected2 =
            expected.Replace("\r\n", "\n")
            |> fun s -> s.Split('\n')
            |> Array.map (fun s -> s.TrimEnd(' '))

        Assert.shouldBeEquivalentTo expected2 actual
    
    [<Test>]
    let ``can generate sigs with comments`` () = 
        """
/// namespace comments
namespace Sample

/// exception comments
exception MyEx of reason: string

/// module-level docs
module Inner =
    /// type-level docs
    type Facts
        /// primary ctor docs
        (name: string) =
        /// constructor-level docs
        new() = Facts("default name")
        /// member-level docs
        member x.blah() = [1;2;3]
        /// auto-property-level docs
        member val Name = name with get, set

    /// module-level binding docs
    let module_member = ()

    /// record docs
    type TestRecord =
        {
            /// record field docs
            RecordField: int
        }
        /// record member docs
        member x.Data = 1
        /// static record member docs
        static member Foo = true

    /// union docs
    type TestUnion =
    /// docs for first case
    | FirstCase of thing: int
        /// union member
        member x.Thing = match x with | FirstCase thing -> thing
        """
        |> sigShouldBe """namespace Sample
  
  /// exception comments
  exception MyEx of reason: string
  
  /// module-level docs
  module Inner =
    
    /// type-level docs
    type Facts =
      
      /// constructor-level docs
      new: unit -> Facts
      
      /// primary ctor docs
      new: name: string -> Facts
      
      /// member-level docs
      member blah: unit -> int list
            
      /// auto-property-level docs
      member Name: string
    
    /// module-level binding docs
    val module_member: unit

    /// record docs
    type TestRecord =
      {
        /// record field docs
        RecordField: int
      }

      /// record member docs
      member Data: int

      /// static record member docs
      static member Foo: bool

    /// union docs
    type TestUnion =

      /// docs for first case
      | FirstCase of thing: int

      /// union member
      member Thing: int
  """