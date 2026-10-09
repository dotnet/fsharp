module FSharp.Compiler.Service.Tests.CompletionTypeAnnotationsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``Inherit.CompletionInConstructorArguments1`` () =
    let info =
        Checker.getCompletionInfo
            """type A(a : int) = class end
type B() = inherit A(a{caret})"""

    assertHasItemWithNames [ "abs" ] info

[<Fact>]
let ``Inherit.CompletionInConstructorArguments2`` () =
    let info =
        Checker.getCompletionInfo
            """type A(a : int) = class end
type B() = inherit A(System.String.{caret})"""

    assertHasItemWithNames [ "Empty" ] info
    assertHasNoItemsWithNames [ "Array"; "Collections" ] info

[<Fact>]
let ``ProtectedMembers.BaseClass`` () =
    let info =
        Checker.getCompletionInfo
            """type T() = 
   inherit exn()
   member this.Run(x : exn) = x.{caret}"""

    assertHasItemWithNames [ "Message"; "HResult" ] info

[<Fact>]
let ``BasicLocalMemberList`` () =
    let info =
        Checker.getCompletionInfo
            """let MyFunction (s:string) = 
    let y="dog"
    y.{caret}
    ()"""

    assertHasItemWithNames [ "Substring"; "GetHashCode" ] info

[<Fact>]
let ``LocalMemberList.WithPartialMemberEntry1`` () =
    let info =
        Checker.getCompletionInfo
            """let MyFunction (s:string) = 
    let y="dog"
    y.Substri{caret}
    ()"""

    assertHasItemWithNames [ "Substring"; "GetHashCode" ] info

[<Fact>]
let ``LocalMemberList.WithPartialMemberEntry2`` () =
    let info =
        Checker.getCompletionInfo
            """let MyFunction (s:string) = 
    let y="dog"
    y.{caret}Substri
    ()"""

    assertHasItemWithNames [ "Substring"; "GetHashCode" ] info

[<Fact>]
let ``MemberInfoCompileErrorsShowInDataTip`` () =
    let info =
        Checker.getCompletionInfo
            """type Foo = 
    member x.Bar() = 0
let foovalue:Foo = unbox null
foovalue.B{caret}"""

    assertHasItemWithNames [ "Bar" ] info

[<Fact>]
let ``Identifier.Invalid.Bug876b`` () =
    let info =
        Checker.getCompletionInfo
            """let f (x:System.Exception) = x.{caret}
  for x = 0 to 0 do () done"""

    assertHasItemWithNames [ "Message"; "StackTrace" ] info

[<Fact>]
let ``Identifier.Invalid.Bug876c`` () =
    let info =
        Checker.getCompletionInfo
            """let f (x:System.Exception) = x.{caret}
  12"""

    assertHasItemWithNames [ "Message" ] info

[<Theory>]
[<InlineData("""
                type ClassLetBindIn(x:int) = 
                    let m_field = x.{caret} """)>]
[<InlineData("
let funcNestedLetBinding (x:int) = 
    let funcNested (x:int) = x.{caret}
    () 
")>]
[<InlineData("
module ModuleLetBindIn =
    let f (x:int) = x.{caret}
")>]
let ``Identifier.IntBinderDot`` (source: string) =
    let info = Checker.getCompletionInfo source

    assertHasItemWithNames [ "ToString"; "Equals" ] info

[<Theory>]
[<InlineData("""
                let f (x : string) = ()
                f ("1" + "1").{caret}
                """)>]
[<InlineData("""
                let f (x : string) = ()
                // And in many different contexts where the atomic expression occurs at the end of the expression, e.g.
                let x = y in f ("1" + "1").{caret}
                """)>]
[<InlineData("""
                let f (x : string) = ()
                while true do
                    f ("1" + "1").{caret} 
                """)>]
let ``Expression.AtomicStringDot`` (source: string) =
    let info = Checker.getCompletionInfo source

    assertHasItemWithNames [ "CompareTo"; "ToString" ] info

[<Fact>]
let ``Expression.Nested.InLetBind`` () =
    let info =
        Checker.getCompletionInfo
            """
                let f (x : string) = ()
                // Nested expressions
                let x = 42 |> ignore; f ("1" + "1").{caret}
                """

    assertHasItemWithNames [ "Chars"; "Length" ] info

[<Fact>]
let ``Expression.Nested.InWhileLoop`` () =
    let info =
        Checker.getCompletionInfo
            """
                let f (x : string) = ()
                while true do
                    ignore (f ("1" + "1").{caret}) 
                """

    assertHasItemWithNames [ "Chars"; "Length" ] info

[<Fact>]
let ``LongIdent.PInvoke.AsReturnType`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System.IO
                open System.Runtime.InteropServices
                // Get two temp files, write data into one of them
                let tempFile1, tempFile2 = Path.GetTempFileName(), Path.GetTempFileName()
                let writer = new StreamWriter (tempFile1)
                writer.WriteLine("Some Data")
                writer.Close()
                // Original signature
                //[<DllImport("kernel32.dll")>]
                //extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);
                [<DllImport("kernel32.dll", EntryPoint="CopyFile")>]
                extern System.{caret} CopyFile_Arrays(char[] lpExistingFileName, char[] lpNewFileName, bool bFailIfExists);
                let result = CopyFile_Arrays(tempFile1.ToCharArray(), tempFile2.ToCharArray(), false)
                printfn "Array %A" result"""

    assertHasItemWithNames [ "Boolean"; "Int32" ] info

[<Fact>]
let ``LongIdent.PInvoke.AsParameterType`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System.IO
                open System.Runtime.InteropServices
                [<DllImport("kernel32.dll", EntryPoint="CopyFile")>]
                extern bool CopyFile_ArraySpaces(char [] lpExistingFileName, char []lpNewFileName, System.{caret} bFailIfExists);
                let result2 = CopyFile_Arrays(tempFile1.ToCharArray(), tempFile2.ToCharArray(), false)
                printfn "Array Space %A" result2"""

    assertHasItemWithNames [ "Boolean"; "Int32" ] info
