namespace UnitTests.Tests.ProjectSystem

open System
open NUnit.Framework
open UnitTests.TestLib.Utils
open Microsoft.VisualStudio.FSharp.ProjectSystem

module RoseTreeTestHelpers =
    open RoseTree
        
    let treeAssertEquals expected actual =
        let msg =
            [ ["Expected"]
              (draw (sprintf "%A") expected)
              ["but was"]
              (draw (sprintf "%A") actual) ]
            |> List.collect id
            |> String.concat Environment.NewLine
        Asserts.AssertEqualMsg expected actual msg

    let treeAssertForestEquals expected actual =
        actual
        |> List.zip expected
        |> List.iter (fun (e, a) -> treeAssertEquals e a)

    let node name children = RoseTree.Node (name, children)
    let leaf name = node name []

[<TestFixture>]
[<Category("wip")>]
module RoseTreeTest =
    open RoseTree
    open RoseTreeTestHelpers

    [<Test>]
    let append () =
        let n,l = node,leaf

        l"a"
        |> appendTree (l"~")
        |> treeAssertEquals (n"~" [ l"a" ])

        l"b"
        |> appendTree (n"~" [ l"a" ])
        |> treeAssertEquals (n"~" [ l"a"; l"b" ])

        n "c" [ l"d" ]
        |> appendTree (n"~" [ l"a" ])
        |> treeAssertEquals (
            n"~" [ 
                l"a"
                n "c" [ 
                    l"d" 
                ]
            ])

        l"a"
        |> appendTree (n"~" [ l"a" ])
        |> treeAssertEquals (n"~" [ l"a"; l"a" ])

        l"a"
        |> appendTree (n"~" [ l"f"; l"g"; l"h" ])
        |> treeAssertEquals (n"~" [ l"f"; l"g"; l"h"; l"a" ])

    [<Test>]
    let mapForest () =
        node "~" [ 
            node "a" [ 
                leaf "1"
                leaf "2" 
                leaf "3" 
            ] 
            node "b" [ 
                leaf "33"
                leaf "22"
                leaf "11"
            ]
            leaf "c"
        ]
        |> mapForest List.rev
        |> treeAssertEquals (
            node "~" [ 
                leaf "c"
                node "b" [ 
                    leaf "11"
                    leaf "22" 
                    leaf "33" 
                ]
                node "a" [ 
                    leaf "3"
                    leaf "2"
                    leaf "1"
                ]
            ])


    [<Test>]
    let mergeFolders () =
        let n,l = node, leaf

        []
        |> foldSiblings (fun _ _ -> Assert.Fail("unexpected"); None)
        |> Asserts.AssertEqual []

        [ l"a" ]
        |> foldSiblings (fun _ _ -> Assert.Fail("unexpected"); None)
        |> treeAssertForestEquals [ l"a" ]

        // use the new node label
        [ l"h"; l"h"; ]
        |> foldSiblings (fun x y -> match x, y with "h", "h" -> Some "k" | _ -> None)
        |> treeAssertForestEquals [ l"k" ]

        // fold
        [ l"a"; l"a"; l"b"; l"a"; l"a"; l"a"; l"g" ]
        |> foldSiblings (fun x y -> if x = y then Some x else None)
        |> treeAssertForestEquals [ l"a"; l"b"; l"a"; l"g" ]

        [ l"a"; l"a"; l"b"; l"a"; l"a"; l"a"; l"g" ]
        |> foldSiblings (fun x y -> match x, y with "a", "a" -> Some "d" | _ -> None)
        |> treeAssertForestEquals [ l"d"; l"b"; l"d"; l"a"; l"g" ]

        [ l"a"; l"a"; l"b"; l"a"; l"a"; l"a"; l"g" ]
        |> foldSiblings (fun x y -> if x = y then Some x else None)
        |> treeAssertForestEquals [ l"a"; l"b"; l"a"; l"g" ]

        // using result of previour run
        [ l"a"; l"a"; l"b"; l"c" ]
        |> foldSiblings (fun x y -> if x = y then Some "b" else None)
        |> treeAssertForestEquals [ l"b"; l"c" ]

        // merge children
        [ l"b"; (n"c" [ l"1"; l"2" ]); (n"c" [ l"3"; l"4" ]); l"o"]
        |> foldSiblings (fun x y -> if x = y then Some x else None)
        |> treeAssertForestEquals [ l"b"; (n"c" [ l"1"; l"2"; l"3"; l"4" ]); l"o" ]

    [<Test>]
    let addPath () = 
        leaf "a"
        |> addPath
        |> treeAssertEquals (leaf ([],"a"))

        node "a" [
            leaf "b"
        ]
        |> addPath
        |> treeAssertEquals (
            node ([],"a") [
                leaf (["a"], "b")
            ])


        node "a" [
            node "b" [
                node "c" [
                    leaf "d"
                    leaf "e"
                ]
            ]
            node "f" [
                node "g" [
                    leaf "h"
                ]
                leaf "i"
            ]
        ]
        |> addPath
        |> treeAssertEquals (
            node ([],"a") [
                node (["a"],"b") [
                    node (["a";"b"],"c") [
                        leaf (["a";"b";"c"],"d")
                        leaf (["a";"b";"c"],"e")
                    ]
                ]
                node (["a"],"f") [
                    node (["a";"f"],"g") [
                        leaf (["a";"f";"g"],"h")
                    ]
                    leaf (["a";"f"],"i")
                ]
            ])

    [<Test>]
    let map () =

        leaf "15"
        |> map Int32.Parse
        |> treeAssertEquals (leaf 15)

        node "5" [
            leaf "66"
            node "77" [
                leaf "888"
            ]
            leaf "99"
        ]
        |> map Int32.Parse
        |> treeAssertEquals (
            node 5 [
                leaf 66
                node 77 [
                    leaf 888
                ]
                leaf 99
            ])


    [<Test>]
    let fold () =

        leaf 8
        |> fold (fun acc x -> x :: acc) [0]
        |> Asserts.AssertEqual [8; 0]

        node 1 [
            leaf 2
            node 3 [
                leaf 4
            ]
            leaf 5
        ]
        |> fold (fun acc x -> x :: acc) [0]
        |> Asserts.AssertEqual [5; 4; 3; 2; 1; 0]
