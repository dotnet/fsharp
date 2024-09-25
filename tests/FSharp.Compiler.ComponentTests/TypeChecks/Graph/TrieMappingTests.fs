module TypeChecks.TrieMappingTests

open Xunit
open FSharp.Compiler.GraphChecking
open TestUtils

let private noDependencies = Set.empty<int>

let private getLastTrie files = TrieMapping.mkTrie files |> Array.last |> snd

[<Fact>]
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
            "D.fs", "module D"
        |]

    let files =
        sampleFiles
        |> Array.mapi (fun idx (fileName, code) ->
            {
                Idx = idx
                FileName = fileName
                ParsedInput = parseSourceCode (fileName, code)
            } : FileInProject)

    let trie = getLastTrie files

    match trie.Current with
    | TrieNodeInfo.Root _ -> ()
    | current -> Assert.Fail($"mkTrie should always return a TrieNodeInfo.Root, got {current}")

    let xNode = trie.Children.["X"]
    Assert.Equal(1, xNode.Children.Count)
    Assert.True(Seq.isEmpty xNode.Files)

    let yNode = xNode.Children["Y"]
    Assert.Equal(2, yNode.Children.Count)
    Assert.Equal<Set<FileIndex>>(set [| 2 |], yNode.Files)

    let aNode = yNode.Children["A"]
    Assert.Equal(0, aNode.Children.Count)
    Assert.Equal<Set<FileIndex>>(set [| 0 |], aNode.Files)

    let bNode = yNode.Children["B"]
    Assert.Equal(0, bNode.Children.Count)
    Assert.Equal<Set<FileIndex>>(set [| 1 |], bNode.Files)

[<Fact>]
let ``Toplevel AutoOpen module with prefixed namespace`` () =
    let trie =
        getLastTrie
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
    Assert.Equal<Set<FileIndex>>(set [| 0 |], aNode.Files)
    let bNode = aNode.Children.["B"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], bNode.Files)

[<Fact>]
let ``Toplevel AutoOpen module with multi prefixed namespace`` () =
    let trie =
        getLastTrie
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
    Assert.Equal<Set<FileIndex>>(noDependencies, aNode.Files)
    let bNode = aNode.Children.["B"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], bNode.Files)
    let cNode = bNode.Children.["C"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], cNode.Files)

[<Fact>]
let ``Global namespace should link files to the root node`` () =
    let trie =
        getLastTrie
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
                {
                    Idx = 2
                    FileName = "B.fs"
                    // The last file shouldn't be processed
                    ParsedInput = Unchecked.defaultof<FSharp.Compiler.Syntax.ParsedInput> 
                }
            |]

    Assert.Equal<Set<FileIndex>>(set [| 0; 1 |], trie.Files)

[<Fact>]
let ``Module with a single ident and AutoOpen attribute should link files to root`` () =
    let trie =
        getLastTrie
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
                {
                    Idx = 2
                    FileName = "B.fs"
                    // The last file shouldn't be processed
                    ParsedInput = Unchecked.defaultof<FSharp.Compiler.Syntax.ParsedInput> 
                }
            |]

    Assert.Equal<Set<FileIndex>>(set [| 0; 1 |], trie.Files)
    Assert.Equal(0, trie.Children.Count)

[<Fact>]
let ``Module with AutoOpen attribute and two ident should expose file at two levels`` () =
    let trie =
        getLastTrie
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

    Assert.Equal<Set<FileIndex>>(noDependencies, trie.Files)
    let xNode = trie.Children.["X"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], xNode.Files)
    let yNode = xNode.Children.["Y"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], yNode.Files)

[<Fact>]
let ``Module with AutoOpen attribute and three ident should expose file at last two levels`` () =
    let trie =
        getLastTrie
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

    Assert.Equal<Set<FileIndex>>(noDependencies, trie.Files)
    let xNode = trie.Children.["X"]
    Assert.Equal<Set<FileIndex>>(noDependencies, xNode.Files)
    let yNode = xNode.Children.["Y"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], yNode.Files)
    let zNode = yNode.Children.["Z"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], zNode.Files)

[<Fact>]
let ``Nested AutoOpen module in namespace will expose the file to the namespace node`` () =
    let trie =
        getLastTrie
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

    Assert.Equal<Set<FileIndex>>(noDependencies, trie.Files)
    let xNode = trie.Children.["X"]
    Assert.Equal<Set<FileIndex>>(noDependencies, xNode.Files)
    let yNode = xNode.Children.["Y"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], yNode.Files)
    let zNode = yNode.Children.["Z"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], zNode.Files)

[<Fact>]
let ``Two modules with the same name, only the first file exposes the index`` () =
    let trie =
        getLastTrie
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

    Assert.Equal(1, trie.Children.Count)
    let aNode = trie.Children.["A"]
    Assert.Equal<Set<FileIndex>>(set [| 0 |], aNode.Files)

[<Fact>]
let ``Two nested modules with the same name, in named namespace`` () =
    let trie =
        getLastTrie
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

    Assert.Equal(1, trie.Children.Count)
    let node = trie.Children.["N"]
    Assert.Equal(1, node.Children.Count)

[<Fact>]
let ``Two nested modules with the same name, in namespace global`` () =
    let trie =
        getLastTrie
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
    Assert.Equal<Set<FileIndex>>(set [| 0 |], trie.Files)

[<Fact>]
let ``Two nested modules with the same name, in anonymous module`` () =
    let trie =
        getLastTrie
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

    Assert.Equal(1, trie.Children.Count)
    Assert.True(trie.Children.ContainsKey("module"))

[<Fact>]
let ``Two nested modules with the same name, in nested module`` () =
    let trie =
        getLastTrie
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
    Assert.Equal(1, bNode.Children.Count)
    Assert.True(bNode.Children.ContainsKey("module"))

[<Fact>]
let ``Two nested modules with the same name, in nested module in signature file`` () =
    let trie =
        getLastTrie
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
    Assert.Equal(1, bNode.Children.Count)
    Assert.True(bNode.Children.ContainsKey("module"))

[<Fact>]
let ``Two namespaces with the same name in the same implementation file`` () =
    let trie =
        getLastTrie
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
    Assert.Equal(2, aNode.Children.Count)

[<Fact>]
let ``Two namespaces with the same name in the same signature file`` () =
    let trie =
        getLastTrie
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
    Assert.Equal(2, aNode.Children.Count)

[<Fact>]
let ``Tries are built up incrementally`` () =
    let trie =
        TrieMapping.mkTrie
            [|
                {
                    Idx = 0
                    FileName = "A.fs"
                    ParsedInput = parseSourceCode ("A.fs", "module A") 
                }
                {
                    Idx = 1
                    FileName = "B.fs"
                    ParsedInput = parseSourceCode ("B.fs", "module B") 
                }
                {
                    Idx = 2
                    FileName = "C.fs"
                    ParsedInput = parseSourceCode ("C.fs", "module C")
                }
                {
                    Idx = 3
                    FileName = "D.fs"
                    ParsedInput = parseSourceCode ("D.fs", "module D")
                }
            |]

    for idx, t in trie do
        Assert.Equal(idx + 1, t.Children.Count)


module InvalidSyntax =

    [<Fact>]
    let ``Unnamed module`` () =
        let trie =
            getLastTrie
                [| { Idx = 0
                     FileName = "A.fs"
                     ParsedInput =
                       parseSourceCode (
                           "A.fs",
                           """
                        module

                        ()
                        """
                       ) } |]

        Assert.True trie.Children.IsEmpty
