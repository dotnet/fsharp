module TypeChecks.TrieMappingTests

open NUnit.Framework
open FSharp.Compiler.GraphChecking
open TestUtils

let private noDependencies = Set.empty<int>

[<Test>]
let ``Basic trie`` () =
    let sampleFiles =
        [|
            "A.fs",
            """
module X.Y.A

let a = []
    """
            "B.fs",
            """
module X.Y.B

let b = []
    """
            "C.fs",
            """
namespace X.Y

type C = { CX: int; CY: int }
    """
        |]

    let files =
        sampleFiles
        |> Array.mapi (fun idx (fileName, code) ->
            {
                Idx = idx
                FileName = fileName
                ParsedInput = parseSourceCode (fileName, code)
            } : FileInProject)

    let trie = TrieMapping.mkTrie files

    match trie.Current with
    | TrieNodeInfo.Root _ -> ()
    | current -> Assert.Fail($"mkTrie should always return a TrieNodeInfo.Root, got {current}")

    let xNode = trie.Children.["X"]
    Assert.AreEqual(1, xNode.Children.Count)
    Assert.True(Seq.isEmpty xNode.Files)

    let yNode = xNode.Children["Y"]
    Assert.AreEqual(2, yNode.Children.Count)
    Assert.AreEqual(set [| 2 |], yNode.Files)

    let aNode = yNode.Children["A"]
    Assert.AreEqual(0, aNode.Children.Count)
    Assert.AreEqual(set [| 0 |], aNode.Files)

    let bNode = yNode.Children["B"]
    Assert.AreEqual(0, bNode.Children.Count)
    Assert.AreEqual(set [| 1 |], bNode.Files)

[<Test>]
let ``Toplevel AutoOpen module with prefixed namespace`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fs"
                    ParsedInput =
                        parseSourceCode (
                            "A.fs",
                            """
[<AutoOpen>]
module A.B

let a = 0
"""
                        )
                }
            |]

    // Assert that both A and B expose file index 0
    let aNode = trie.Children.["A"]
    Assert.AreEqual(set [| 0 |], aNode.Files)
    let bNode = aNode.Children.["B"]
    Assert.AreEqual(set [| 0 |], bNode.Files)

[<Test>]
let ``Toplevel AutoOpen module with multi prefixed namespace`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fsi"
                    ParsedInput =
                        parseSourceCode (
                            "A.fsi",
                            """
[<AutoOpen>]
module A.B.C

let a = 0
"""
                        )
                }
            |]

    // Assert that B and C expose file index 0, namespace A should not.
    let aNode = trie.Children.["A"]
    Assert.AreEqual(noDependencies, aNode.Files)
    let bNode = aNode.Children.["B"]
    Assert.AreEqual(set [| 0 |], bNode.Files)
    let cNode = bNode.Children.["C"]
    Assert.AreEqual(set [| 0 |], cNode.Files)

[<Test>]
let ``Global namespace should link files to the root node`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fs"
                    ParsedInput =
                        parseSourceCode (
                            "A.fs",
                            """
namespace global

type A = { A : int }
"""
                        )
                }
                {
                    Idx = 1
                    FileName = "B.fsi"
                    ParsedInput =
                        parseSourceCode (
                            "B.fsi",
                            """
namespace global

type B = { Y : int }
"""
                        )
                }
            |]

    Assert.AreEqual(set [| 0; 1 |], trie.Files)

[<Test>]
let ``Module with a single ident and AutoOpen attribute should link files to root`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fs"
                    ParsedInput =
                        parseSourceCode (
                            "A.fs",
                            """
[<AutoOpen>]
module A

type A = { A : int }
"""
                        )
                }
                {
                    Idx = 1
                    FileName = "B.fsi"
                    ParsedInput =
                        parseSourceCode (
                            "B.fsi",
                            """
[<AutoOpen>]
module B

type B = { Y : int }
"""
                        )
                }
            |]

    Assert.AreEqual(set [| 0; 1 |], trie.Files)
    Assert.AreEqual(0, trie.Children.Count)

[<Test>]
let ``Module with AutoOpen attribute and two ident should expose file at two levels`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "Y.fs"
                    ParsedInput =
                        parseSourceCode (
                            "Y.fs",
                            """
[<AutoOpen>]
module X.Y

type A = { A : int }
"""
                        )
                }
            |]

    Assert.AreEqual(noDependencies, trie.Files)
    let xNode = trie.Children.["X"]
    Assert.AreEqual(set [| 0 |], xNode.Files)
    let yNode = xNode.Children.["Y"]
    Assert.AreEqual(set [| 0 |], yNode.Files)

[<Test>]
let ``Module with AutoOpen attribute and three ident should expose file at last two levels`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "Z.fsi"
                    ParsedInput =
                        parseSourceCode (
                            "Z.fsi",
                            """
[<AutoOpen>]
module X.Y.Z

type A = { A : int }
"""
                        )
                }
            |]

    Assert.AreEqual(noDependencies, trie.Files)
    let xNode = trie.Children.["X"]
    Assert.AreEqual(noDependencies, xNode.Files)
    let yNode = xNode.Children.["Y"]
    Assert.AreEqual(set [| 0 |], yNode.Files)
    let zNode = yNode.Children.["Z"]
    Assert.AreEqual(set [| 0 |], zNode.Files)

[<Test>]
let ``Nested AutoOpen module in namespace will expose the file to the namespace node`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "Z.fs"
                    ParsedInput =
                        parseSourceCode (
                            "Z.fs",
                            """
namespace X.Y

[<AutoOpen>]
module Z =

    type A = { A: int }
"""
                        )
                }
            |]

    Assert.AreEqual(noDependencies, trie.Files)
    let xNode = trie.Children.["X"]
    Assert.AreEqual(noDependencies, xNode.Files)
    let yNode = xNode.Children.["Y"]
    Assert.AreEqual(set [| 0 |], yNode.Files)
    let zNode = yNode.Children.["Z"]
    Assert.AreEqual(set [| 0 |], zNode.Files)

[<Test>]
let ``Two modules with the same name, only the first file exposes the index`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fs"
                    ParsedInput =
                        parseSourceCode (
                            "A.fs",
                            """
module A

type B = { C: int }
"""
                        )
                }
                {
                    Idx = 1
                    FileName = "A2.fs"
                    ParsedInput =
                        parseSourceCode (
                            "A2.fs",
                            """
module A

let _ = ()
"""
                        )
                }
            |]

    Assert.AreEqual(1, trie.Children.Count)
    let aNode = trie.Children.["A"]
    Assert.AreEqual(set [| 0 |], aNode.Files)

[<Test>]
let ``Two nested modules with the same name, in named namespace`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fs"
                    ParsedInput =
                        parseSourceCode (
                            "A.fs",
                            """
namespace N

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 
module ``module`` = 
            let f x = x + 1
module ``module`` =
            let g x = x + 1
"""                     )
                }
            |]

    Assert.AreEqual(1, trie.Children.Count)
    let node = trie.Children.["N"]
    Assert.AreEqual(1, node.Children.Count)

[<Test>]
let ``Two nested modules with the same name, in namespace global`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fs"
                    ParsedInput =
                        parseSourceCode (
                            "A.fs",
                            """
namespace global

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 
module ``module`` = 
            let f x = x + 1
module ``module`` =
            let g x = x + 1
"""                     )
                }
            |]

    // namespace global leads to a Root entry, no further processing will be done.
    Assert.AreEqual(set [| 0 |], trie.Files)

[<Test>]
let ``Two nested modules with the same name, in anonymous module`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 1
                    FileName = "Program.fs"
                    ParsedInput =
                        parseSourceCode (
                            "Program.fs",
                            """
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 
module ``module`` = 
            let f x = x + 1
module ``module`` =
            let g x = x + 1
"""                     )
                }
            |]

    Assert.AreEqual(1, trie.Children.Count)
    Assert.True(trie.Children.ContainsKey("module"))

[<Test>]
let ``Two nested modules with the same name, in nested module`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fs"
                    ParsedInput =
                        parseSourceCode (
                            "A.fs",
                            """
namespace A

module B =

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 
    module ``module`` = 
                let f x = x + 1
    module ``module`` =
                let g x = x + 1
"""                     )
                }
            |]

    let bNode = trie.Children["A"].Children["B"]
    Assert.AreEqual(1, bNode.Children.Count)
    Assert.True(bNode.Children.ContainsKey("module"))

[<Test>]
let ``Two nested modules with the same name, in nested module in signature file`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fsi"
                    ParsedInput =
                        parseSourceCode (
                            "A.fsi",
                            """
namespace A

module B =

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 
    module ``module`` = begin end
    module ``module`` = begin end
"""                     )
                }
            |]

    let bNode = trie.Children["A"].Children["B"]
    Assert.AreEqual(1, bNode.Children.Count)
    Assert.True(bNode.Children.ContainsKey("module"))

[<Test>]
let ``Two namespaces with the same name in the same implementation file`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fs"
                    ParsedInput =
                        parseSourceCode (
                            "A.fs",
                            """
namespace A

module B = begin end

namespace A

module C = begin end
"""                     )
                }
            |]

    let aNode = trie.Children["A"]
    Assert.AreEqual(2, aNode.Children.Count)

[<Test>]
let ``Two namespaces with the same name in the same signature file`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fsi"
                    ParsedInput =
                        parseSourceCode (
                            "A.fsi",
                            """
namespace A

module B = begin end

namespace A

module C = begin end
"""                     )
                }
            |]

    let aNode = trie.Children["A"]
    Assert.AreEqual(2, aNode.Children.Count)
