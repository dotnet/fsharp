module FSharp.Compiler.Service.Tests.CompletionTuplesTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``NotShowInfo.ClassMemberDeclA.Bug3602`` () =
    let info =
        Checker.getCompletionInfo
            """type Foo() =
    member this.Func (x, y) = ()
    member (*marker*) this.{caret}
()"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``NotShowInfo.ClassMemberDeclB.Bug3602`` () =
    let info =
        Checker.getCompletionInfo
            """type Foo() =
    member this.Func (x, y) = ()
    member this.{caret}
()"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Expression.InLetScope`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest

                let p4 = 
                    let isPalindrome x = 
                        let chars = (string x).ToCharArray()
                        let len = chars.{caret}
                        chars 
                        |> Array.mapi (fun i c -> (i(*Marker2*), c(*Marker3*))"""

    assertHasItemWithNames [ "IsFixedSize"; "Initialize" ] info

[<Fact>]
let ``Expression.InFunScope.FirstParameter`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                let p4 = 
                    let isPalindrome x = 
                        let chars = (string x).ToCharArray()
                        let len = chars(*Marker1*)
                        chars 
                        |> Array.mapi (fun i c -> (i.{caret}, c(*Marker3*))"""

    assertHasItemWithNames [ "CompareTo" ] info

[<Fact>]
let ``Expression.InFunScope.SecParameter`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest

                let p4 = 
                    let isPalindrome x = 
                        let chars = (string x).ToCharArray()
                        let len = chars(*Marker1*)
                        chars 
                        |> Array.mapi (fun i c -> (i(*Marker2*), c.{caret})"""

    assertHasItemWithNames [ "GetType"; "ToString" ] info
