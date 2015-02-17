namespace UnitTests.Tests.ProjectSystem

open System
open NUnit.Framework
open UnitTests.TestLib.Utils
open Microsoft.VisualStudio.FSharp.ProjectSystem

[<TestFixture>]
[<Category("wip")>]
module ProjectTreeTest =
    open ProjectTree
    open RoseTreeTestHelpers

    [<Test>]
    let displayHierarchyPath () =
        let obj = new Object()
        
        let uri path = Uri(path, UriKind.Relative)

        displayHierarchyPath (obj, (uri "readme.txt"))
        |> treeAssertEquals (
            leaf (ProjectTreeNode.Item ("readme.txt", obj))
            )

        displayHierarchyPath (obj, (uri "path/to/file.txt"))
        |> treeAssertEquals (
            node (ProjectTreeNode.Folder "path") [
                node (ProjectTreeNode.Folder "to") [
                    leaf (ProjectTreeNode.Item ("file.txt", obj))
                ]
            ])

        displayHierarchyPath (obj, (uri "some/dir/"))
        |> treeAssertEquals (
            node (ProjectTreeNode.Folder "some") [
                leaf (ProjectTreeNode.Item ("dir", obj))
            ])

        displayHierarchyPath (obj, (uri "with WhiteSpace/caseSensitive/file Domain.fs"))
        |> treeAssertEquals (
            node (ProjectTreeNode.Folder "with WhiteSpace") [
                node (ProjectTreeNode.Folder "caseSensitive") [
                    leaf (ProjectTreeNode.Item ("file Domain.fs", obj))
                ]
            ])

    [<Test>]
    let normalizeFolder () =
        let uri x = Uri(x, UriKind.Relative)
        let folder name = ProjectTreeNode.Folder name
        let item name = ProjectTreeNode.Item (name, { Item = 0; Content = MSBuildProjectItem.None; Include = uri name; Link = None })
        let itemFolder name = ProjectTreeNode.Item (name, { Item = 0; Content = MSBuildProjectItem.Folder; Include = uri name; Link = None })

        // generic items doesn't merge
        normalizeFolder (item "a") (item "a") |> Asserts.AssertEqual None

        // folders merge
        normalizeFolder (folder "c") (folder "c")
        |> Asserts.AssertEqual (Some ((folder "c"), [folder "c"]))
        
        normalizeFolder (folder "d") (folder "c") |> Asserts.AssertEqual None

        // msbuild Folder item merge
        normalizeFolder (itemFolder "h") (itemFolder "h" )
        |> Asserts.AssertEqual (Some (itemFolder "h", [itemFolder "h"]))

        // folders and items doesn't merge
        normalizeFolder (folder "o") (item "o" ) |> Asserts.AssertEqual None
        normalizeFolder (item "k") (folder "k") |> Asserts.AssertEqual None

        // folders and msbuild Folder item merge
        normalizeFolder (folder "p") (itemFolder "p")
        |> Asserts.AssertEqual (Some (folder "p", [itemFolder "p"]))
        
        normalizeFolder (itemFolder "q") (folder "q")
        |> Asserts.AssertEqual (Some (folder "q", [itemFolder "q"]))

        // case insensitive
        normalizeFolder (folder "DireCtorY") (folder "diReCtorY")
        |> Asserts.AssertEqual (Some (folder "DireCtorY", [folder "diReCtorY"]))
         
        normalizeFolder (itemFolder "ASD") (folder "aSd")
        |> Asserts.AssertEqual (Some (folder "aSd", [itemFolder "ASD"]))
         
        normalizeFolder (folder "eUd") (itemFolder "Eud")
        |> Asserts.AssertEqual (Some (folder "eUd", [itemFolder "Eud"]))
         
        normalizeFolder (itemFolder "eUd") (itemFolder "Eud")
        |> Asserts.AssertEqual (Some (itemFolder "eUd", [itemFolder "Eud"]))

    [<Test>]
    let isAlreadyRenderedFolder () =

        node "a" [ 
            leaf "b"
            node "e" [
                leaf "d"
            ]
            leaf "b"
        ]
        |> checkAlreadyRenderedFolder Some
        |> Asserts.AssertEqual [["a"],"b"]

        node "a" [ 
            leaf "b"
            leaf "b"
        ]
        |> checkAlreadyRenderedFolder (fun _ -> None)
        |> Asserts.AssertEqual []

        node "~" [
            node "a" [
                node "b" [
                    leaf "c"
                ]
            ]
            leaf "b" 
            leaf "a"
        ]
        |> checkAlreadyRenderedFolder Some
        |> Asserts.AssertEqual [["~"],"a"]

        node "~" [
            node "a" [
                node "b" [
                    leaf "c"
                ]
            ] 
            leaf "b"
            node "a" [
                leaf "b"
            ]
        ]
        |> checkAlreadyRenderedFolder Some
        |> Asserts.AssertEqual [["~"],"a"]

        node "~" [
            node "a" [
                node "b" [
                    leaf "c"
                ]
            ]
            leaf "b"
            node "a" [
                node "b" [
                    leaf "d"
                ]
            ]
        ]
        |> checkAlreadyRenderedFolder Some
        |> Asserts.AssertEqual [["~"],"a"]

        node "~" [
            node "a" [
                node "b" [
                    node "c" [
                        leaf "d"
                        leaf "e"
                        leaf "d"
                    ]
                ]
            ]
        ]
        |> checkAlreadyRenderedFolder Some
        |> Asserts.AssertEqual [["~";"a";"b";"c"],"d"]

    [<Test>]
    let ``createTree valid`` () =
        let uri x = Uri(x, UriKind.Relative)
        let compile x = { Item = x; Include = uri x; Content = MSBuildProjectItem.Compile; Link = None }
        let getPath { Include = x } = x

        let folder' name = ProjectTreeNode.Folder name
        let item' name x = ProjectTreeNode.Item (name, x)

        let items =
            [ compile @"a\e\file1.fs"
              compile @"a\file2.fs"
              compile @"a\b\file3.fs" ]

        let tree, removed = items |> createTree getPath

        Asserts.AssertEqual [] removed

        tree
        |> treeAssertEquals (
            node (folder' "/") [
                node (folder' "a") [
                    node (folder' "e") [
                        leaf (item' "file1.fs" (items.[0]))
                    ]
                    leaf (item' "file2.fs" (items.[1]))
                    node (folder' "b") [
                        leaf (item' "file3.fs" (items.[2]))
                    ]
                ]
            ])
        

    [<Test>]
    let ``createTree remove unused folder`` () =
        let uri x = Uri(x, UriKind.Relative)
        let compile id p = { Include = uri p; Item = id; Content = MSBuildProjectItem.Compile; Link = None }
        let folder id p = { Include = uri p; Item = id; Content = MSBuildProjectItem.Folder; Link = None }
        let getPath { Include = x } = x

        let folder' name = ProjectTreeNode.Folder name
        let item' name x = ProjectTreeNode.Item (name, x)
        
        let items =
            [ compile 0   @"base\shared folder\file2.fs"
              folder  1   @"base\shared folder\"
              compile 2   @"base\shared folder\file1.fs" 
              folder  3   @"base\shared folder\"
              folder  4   @"base\shared folder\"
              folder  5   @"base\"
              compile 6   @"base\shared folder\file3.fs"
              compile 7   @"base\file4.fs"
              folder  8   @"base\"
              folder  9   @"valid\"
              folder  10  @"valid\subDir1"
              folder  11  @"docs\" ]

        let tree, removed = items |> createTree getPath

        removed
        |> List.sortBy (fun { Item = x } -> x)
        |> Asserts.AssertEqual [ items.[1]; items.[3]; items.[4]; items.[5]; items.[8]; items.[9] ] 

        tree
        |> treeAssertEquals (
            node (folder' "/") [
                node (folder' "base") [
                    node (folder' "shared folder") [
                        leaf (item' "file2.fs" (items.[0]))
                        leaf (item' "file1.fs" (items.[2]))
                        leaf (item' "file3.fs" (items.[6]))
                    ]
                    leaf (item' "file4.fs" (items.[7]))
                ]
                node (folder' "valid") [
                    leaf (item' "subDir1" (items.[10]))
                ]
                leaf (item' "docs" (items.[11]))
            ])
