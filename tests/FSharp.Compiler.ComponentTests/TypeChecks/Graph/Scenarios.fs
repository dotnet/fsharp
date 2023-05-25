module FSharp.Compiler.ComponentTests.TypeChecks.Graph.Scenarios

open TestUtils

type Scenario =
    {
        Name: string
        Files: FileInScenario array
    }

    override x.ToString() = x.Name

and FileInScenario =
    {
        FileWithAST: TestFileWithAST
        ExpectedDependencies: Set<int>
        Content: string
    }

let private scenario name files =
    let files = files |> List.toArray |> Array.mapi (fun idx f -> f idx)
    { Name = name; Files = files }

let private sourceFile fileName content (dependencies: Set<int>) =
    fun idx ->
        let fileWithAST =
            {
                Idx = idx
                AST = parseSourceCode (fileName, content)
                File = fileName
            }

        {
            FileWithAST = fileWithAST
            ExpectedDependencies = dependencies
            Content = content
        }

let internal codebases =
    [
        scenario
            "Link via full open statement"
            [
                sourceFile
                    "A.fs"
                    """
module A
do ()
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
module B
open A
"""
                    (set [| 0 |])
            ]
        scenario
            "Partial open statement"
            [
                sourceFile
                    "A.fs"
                    """
module Company.A
let a = ()
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
module Other.B
open Company
open A
"""
                    (set [| 0 |])
            ]
        scenario
            "Link via fully qualified identifier"
            [
                sourceFile
                    "X.fs"
                    """
module X.Y.Z

let z = 9
"""
                    Set.empty
                sourceFile
                    "Y.fs"
                    """
module A.B

let a = 1 + X.Y.Z.z
"""
                    (set [| 0 |])
            ]
        scenario
            "Link via partial open and prefixed identifier"
            [
                sourceFile
                    "A.fs"
                    """
module A.B.C

let d = 1
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
module X.Y.Z

open A.B

let e = C.d + 1
"""
                    (set [| 0 |])
            ]
        scenario
            "Modules sharing a namespace do not link them automatically"
            [
                sourceFile
                    "A.fs"
                    """
module Project.A

let a = 0
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
module Project.B

let b = 0
"""
                    Set.empty
                sourceFile
                    "C.fs"
                    """
module Project.C

let c = 0
"""
                    Set.empty
                sourceFile
                    "D.fs"
                    """
module Project.D

let d = 0
"""
                    Set.empty
            ]
        scenario
            "Files which add types to a namespace are automatically linked to files that share said namespace"
            [
                sourceFile
                    "A.fs"
                    """
namespace Product

type X = { Y : string }
"""
                    Set.empty
                // There is no way to infer what `b` is in this example
                // It could be the type defined in A, so we cannot take any risks here.
                // We link A as dependency of B because A exposes a type in the shared namespace `Product`.
                sourceFile
                    "B.fs"
                    """
module Product.Feature

let a b = b.Y + "z"
"""
                    (set [| 0 |])
            ]
        scenario
            "Toplevel AutoOpen attribute will link to all the subsequent files"
            [
                sourceFile
                    "A.fs"
                    """
[<AutoOpen>]
module Utils

let a b c = b - c
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
namespace X

type Y = { Q: int }
"""
                    (set [| 0 |])
            ]
        // Notice how we link B.fs to A.fsi, this will always be the case for signature/implementation pairs.
        // When debugging, notice that the `Helpers` will be not a part of the trie.
        scenario
            "Signature files are being used to construct the Trie"
            [
                sourceFile // 0
                    "A.fsi"
                    """
module A

val a: int -> int
"""
                    Set.empty
                sourceFile // 1
                    "A.fs"
                    """
module A

module Helpers =
    let impl a = a + 1

let a b = Helpers.impl b
"""
                    (set [| 0 |])
                sourceFile // 2
                    "B.fs"
                    """
module B

let b = A.a 42
"""
                    (set [| 0 |])
            ]
        scenario
            "A partial open statement still links to a file as a last resort"
            [
                sourceFile
                    "A.fs"
                    """
module X.A

let a = 0
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
module X.B

let b = 0
"""
                    Set.empty
                sourceFile
                    "C.fs"
                    """
module Y.C

// This open statement does not do anything.
// It can safely be removed, but because of its presence we need to link it to something that exposes the namespace X.
// We try and pick the file with the lowest index 
open X

let c = 0
"""
                    (set [| 0 |])
            ]
        // `open X` does exist but there is no file that is actively contributing to the X namespace.
        // This is a trade-off scenario, if A.fs had a type or nested module we would consider it to contribute to the X namespace.
        // As it is empty, we don't include the file index in the trie.
        // To satisfy the open statement we link it to the lowest file idx of the found namespace node X in the trie.
        scenario
            "An open statement that leads to a namespace node without any types, should link to the lowest file idx of that namespace node."
            [
                sourceFile
                    "A.fs"
                    """
namespace X
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
namespace Y
"""
                    Set.empty
                sourceFile
                    "C.fs"
                    """
namespace Z

open X
"""
                    (set [| 0 |])
            ]
        // The nested module in this case adds content to the namespace
        // Similar if a namespace had a type.
        scenario
            "Nested module with auto open attribute"
            [
                sourceFile
                    "A.fs"
                    """
namespace Product

[<AutoOpen>]
module X =
    let x: int = 0
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
module Product.Feature

let a b = x + b
"""
                    (set [| 0 |])
            ]
        // Similar to a top level auto open attribute, the global namespace also introduces a link to all the files that come after it.
        scenario
            "Global namespace always introduces a link"
            [
                sourceFile
                    "A.fs"
                    """
namespace global

type A = { B : int }
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
module Product.Feature

let f a = a.B
"""
                    (set [| 0 |])
            ]
        scenario
            "Reference to property of static member from nested module is detected"
            [
                sourceFile
                    "A.fs"
                    """
module A

module B =
    type Person = {Name : string}
    type C =
        static member D: Person = failwith ""
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
module B
let person: string = A.B.C.D.Name
"""
                    (set [| 0 |])
            ]
        // Diamond scenario
        scenario
            "Dependent signature files"
            [
                sourceFile // 0
                    "A.fsi"
                    """
module A

type AType = class end
"""
                    Set.empty
                sourceFile // 1
                    "A.fs"
                    """
module A

type AType = class end
            """
                    (set [| 0 |])
                sourceFile // 2
                    "B.fsi"
                    """
module B

open A

val b: AType -> unit
"""
                    (set [| 0 |])
                sourceFile // 3
                    "B.fs"
                    """
module B

open A

let b (a:AType) = ()
"""
                    (set [| 0; 2 |])
                sourceFile // 4
                    "C.fsi"
                    """
                module C

                type CType = class end
                            """
                    Set.empty
                sourceFile // 5
                    "C.fs"
                    """
module C

type CType = class end
                            """
                    (set [| 4 |])
                sourceFile // 6
                    "D.fsi"
                    """
module D

open A
open C

val d: CType -> unit
            """
                    (set [| 0; 4 |])
                sourceFile // 7
                    "D.fs"
                    """
module D

open A
open B
open C

let d (c: CType) =
    let a : AType = failwith "todo"
    b a
            """
                    (set [| 0; 2; 4; 6 |])
            ]
        scenario
            "Module abbreviations with shared namespace"
            [
                sourceFile
                    "A.fsi"
                    """
module internal FSharp.Compiler.CheckExpressions

exception BakedInMemberConstraintName of string
"""
                    Set.empty
                sourceFile
                    "A.fs"
                    """
module internal FSharp.Compiler.CheckExpressions

exception BakedInMemberConstraintName of string
"""
                    (set [| 0 |])
                sourceFile
                    "B.fs"
                    """
namespace FSharp.Compiler.CodeAnalysis

open FSharp.Compiler

module Tc = CheckExpressions
"""
                    (set [| 0 |])
            ]
        scenario
            "Top level module with auto open and namespace prefix"
            [
                // This file is added to make ensure that B.fs links to A.fs because of the contents of A.
                // If A didn't have the AutoOpen attribute, as a last resort it would be linked to A anyway because of the ghost dependency mechanism.
                sourceFile
                    "Ghost.fs"
                    """
namespace A
"""
                    Set.empty
                sourceFile
                    "A.fs"
                    """
[<AutoOpen>]
module A.B

let a = 0
"""
                    Set.empty
                sourceFile
                    "B.fs"
                    """
module Library

open A
let b = a + 1
"""
                    (set [| 1 |])
            ]
        scenario
            "Top level module with AutoOpen attribute and namespace prefix"
            [
                sourceFile
                    "A.fs"
                    """
[<AutoOpen>]
module X.Y.Z

type A = { A : int }
"""
                    Set.empty
                sourceFile
                    "Library.fs"
                    """
module Library

open X.Y

let fn (a: A) = ()
"""
                    (set [| 0 |])
            ]
        scenario
            "Nested AutoOpen module in namespace is accessed via namespace open"
            [
                sourceFile
                    "Z.fs"
                    """
namespace X.Y

[<AutoOpen>]
module Z =

    type A = { A: int }
"""
                    Set.empty
                sourceFile
                    "Library.fs"
                    """
module Library

open X.Y

let fn (a: A) = ()
"""
                    (set [| 0 |])
            ]
        scenario
            "Implementation uses something defined above and in signature"
            [
                sourceFile
                    "A.fsi"
                    """
module Bar

type Bar =
    new: unit -> Bar
    static member Foo: unit -> unit

val Foo: unit -> unit
"""
                    Set.empty
                sourceFile
                    "A.fs"
                    """
module Bar

type Bar() =
    static member Foo () : unit =
        failwith ""

let Foo () : unit = 
    Bar.Foo ()
"""
                    (set [| 0 |])
            ]
        scenario
            "Ghost dependency takes file index into account"
            [
                sourceFile "X.fs" "module X" Set.empty
                // opened namespace 'System.IO' will be found in the Trie.
                // However, we should not link A.fs to X.fs (because of the ghost dependency mechanism)
                // because B.fs introduces nodes `System` and `IO` and comes after A.fs.
                sourceFile
                    "A.fs"
                    """
module A

open System.IO
                    """
                    Set.empty
                sourceFile "B.fs" "namespace System.IO" Set.empty
            ]
        scenario
            "Ghost dependency that is already linked via module"
            [
                sourceFile "X.fs" "module Foo.Bar.X" Set.empty
                sourceFile "Y.fs" "module Foo.Bar.Y" Set.empty
                // This file is linked to Y.fs due to opening the module `Foo.Bar.Y`
                // The link to Y.fs should also satisfy the ghost dependency created after opening `Foo.Bar`.
                // There is no need to add an additional link to the lowest index in node `Foo.Bar`.
                sourceFile
                    "Z.fs"
                    """
module Z

open Foo.Bar // ghost dependency
open Foo.Bar.Y // Y.fs
"""
                    (set [| 1 |])
            ]
    ]
