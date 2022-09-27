module FSharp.Compiler.ComponentTests.Signatures.RecordTests

open Xunit
open FsUnit
open FSharp.Test.Compiler
open FSharp.Compiler.ComponentTests.Signatures.TestHelpers

[<Fact>]
let ``Internal record with xml comment`` () =
    FSharp
        """
module SignatureFileGeneration.MyModule

type PullActions =
    internal
        {
            /// Any repo which doesn't have a master branch will have one created for it.
            Log : int
        }
"""
    |> printSignaturesWith 80
    |> should
        equal
        """
module SignatureFileGeneration.MyModule

type PullActions =
  internal
    {

      /// Any repo which doesn't have a master branch will have one created for it.
      Log: int
    }"""

[<Fact>]
let ``Attribute on record field with argument`` () =
    FSharp
        """
namespace MyApp.Types

open System

type SomeEnum =
    | ValueOne = 1
    | ValueTwo = 2
    | ValueThree = 3

[<AttributeUsage(AttributeTargets.All)>]
type MyAttribute() =
    inherit System.Attribute()
    member val SomeValue: SomeEnum = SomeEnum.ValueOne with get, set
    
type SomeTypeName =
    {
        /// Some Xml doc
        FieldOne : string
        [<MyAttribute(SomeValue = SomeEnum.ValueTwo)>]
        FieldTwo : string list
        /// Some other Xml doc
        [<MyAttribute(SomeValue = SomeEnum.ValueThree)>]
        FieldThree : string
    }
"""
    |> printSignatures
    |> prependNewline
    |> should equal
        """
namespace MyApp.Types

  [<Struct>]
  type SomeEnum =
    | ValueOne = 1
    | ValueTwo = 2
    | ValueThree = 3

  [<System.AttributeUsage (enum<System.AttributeTargets> (32767))>]
  type MyAttribute =
    inherit System.Attribute

    new: unit -> MyAttribute

    member SomeValue: SomeEnum

  type SomeTypeName =
    {

      /// Some Xml doc
      FieldOne: string
      [<My (SomeValue = enum<SomeEnum> (2))>]
      FieldTwo: string list

      /// Some other Xml doc
      [<My (SomeValue = enum<SomeEnum> (3))>]
      FieldThree: string
    }"""
