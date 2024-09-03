// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.Diagnostics
open Xunit
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Test.Compiler

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
    
    [<Fact>]
    let ``can generate sigs with comments`` () = 
        """
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
      member Name: string with get, set
    
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
    
    [<Fact>]
    let ``can generate signatures for autoproperties`` () = 
        """
namespace Sample

module Inner =
    type Facts(name1: string, name2: string) =

        let mutable name3 = ""
        let mutable value = 1
        
        member val SomeAutoProp = name1 with get, set
        
        member val SomeAutoPropWithGetOnly = name1 with get
        
        member this.PropWithExplGetterOnly
            with get() = name2
            
        member this.PropWithExplSetterOnly
            with set (s: string) (i: float) = name3 <- s

        member this.PropWithExplGetterAndSetter
            with set (s: string) (i: float) = name3 <- s
            and get(j: int) = name3
            
        abstract Property1 : int with get, set
        default this.Property1 with get() = value and set(v : int) = value <- v
        """
        |> sigShouldBe """namespace Sample

  module Inner =

    type Facts =

      new: name1: string * name2: string -> Facts

      member PropWithExplGetterAndSetter: s: string -> float with set

      member PropWithExplGetterAndSetter: j: int -> string with get

      member PropWithExplGetterOnly: string

      member PropWithExplSetterOnly: s: string -> float with set

      override Property1: int with get, set

      abstract Property1: int with get, set

      member SomeAutoProp: string with get, set

      member SomeAutoPropWithGetOnly: string"""

    [<Fact>]
    let ``can generate attributes for implicit namespace`` () = 
        """
[<AutoOpen>]
module A.B

open System

    module Say =
        let hello name =
            printfn "Hello %s" name

        let f a = a
        """
        |> sigShouldBe """
[<AutoOpen>]
module A.B

module Say =

  val hello: name: string -> unit

  val f: a: 'a -> 'a"""

    [<Fact>]
    let ``can generate attributes for implicit namespace with multiple modules`` () = 
        """
[<AutoOpen>]
module A.B.C.D

open System

    module Say =
        let hello name =
            printfn "Hello %s" name

        let f a = a
        """
        |> sigShouldBe """
[<AutoOpen>]
module A.B.C.D

module Say =

  val hello: name: string -> unit

  val f: a: 'a -> 'a"""
