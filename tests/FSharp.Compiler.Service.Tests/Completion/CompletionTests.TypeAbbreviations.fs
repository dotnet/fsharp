module FSharp.Compiler.Service.Tests.CompletionTypeAbbreviationsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``Completion.DetectClasses`` () =
    let sources =
        [ """type X = class
    inherit {caret}"""
          """[<Class>]
type X =
    inherit {caret}"""
          """[<Class>]
type X = class
    inherit {caret}"""
          """[<AbstractClass>]
type X() = 
    inherit {caret}""" ]

    for source in sources do
        let info = Checker.getCompletionInfo source
        assertHasItemWithNames [ "obj" ] info

[<Fact>]
let ``Completion.DetectUnknownCompletionContext`` () =
    let info =
        Checker.getCompletionInfo
            """type X = 
    inherit {caret}"""

    assertHasItemWithNames [ "obj"; "seq" ] info

[<Theory>]
[<InlineData("""
                namespace NS1
                module MyModule =
                    [<System.ObsoleteAttribute>]
                    type ObsoleteType() =
                        member this.TestMethod() = 10
                    [<CompilerMessage("This construct is for ML compatibility.", 62, IsHidden=true)>]
                    type CompilerMessageType() =
                        member this.TestMethod() = 10
                    type TestType() =
                        member this.TestMethod() = 100
                        [<System.ObsoleteAttribute>]
                        member this.ObsoleteMethod() = 100
                        [<CompilerMessage("This construct is for ML compatibility.", 62, IsHidden=true)>]
                        member this.CompilerMessageMethod() = 100
                        [<CompilerMessage("This construct is hidden", 1023, IsHidden=true)>]
                        member this.HiddenMethod() = 10
                        [<CompilerMessage("This construct is not hidden", 1023, IsHidden=false)>]
                        member this.VisibleMethod() = 10
                        [<CompilerMessage("This construct is not hidden", 1023)>]
                        member this.VisibleMethod2() = 10
                namespace NS2
                module m2 =
                    type x = NS1.MyModule.{caret}
                    let b = (new NS1.MyModule.TestType())(*MarkerMethod*)
                """,
             true, "TestType")>]
[<InlineData("""
                namespace NS1
                module MyModule =
                    [<System.ObsoleteAttribute>]
                    type ObsoleteType() =
                        member this.TestMethod() = 10
                    [<CompilerMessage("This construct is for ML compatibility.", 62, IsHidden=true)>]
                    type CompilerMessageType() =
                        member this.TestMethod() = 10
                    type TestType() =
                        member this.TestMethod() = 100
                        [<System.ObsoleteAttribute>]
                        member this.ObsoleteMethod() = 100
                        [<CompilerMessage("This construct is for ML compatibility.", 62, IsHidden=true)>]
                        member this.CompilerMessageMethod() = 100
                        [<CompilerMessage("This construct is hidden", 1023, IsHidden=true)>]
                        member this.HiddenMethod() = 10
                        [<CompilerMessage("This construct is not hidden", 1023, IsHidden=false)>]
                        member this.VisibleMethod() = 10
                        [<CompilerMessage("This construct is not hidden", 1023)>]
                        member this.VisibleMethod2() = 10
                namespace NS2
                module m2 =
                    type x = NS1.MyModule.{caret}
                    let b = (new NS1.MyModule.TestType())(*MarkerMethod*)
                """,
             false, "ObsoleteType;CompilerMessageType")>]
[<InlineData("""
                namespace NS1
                module MyModule =
                    [<System.ObsoleteAttribute>]
                    type ObsoleteType() =
                        member this.TestMethod() = 10
                    [<CompilerMessage("This construct is for ML compatibility.", 62, IsHidden=true)>]
                    type CompilerMessageType() =
                        member this.TestMethod() = 10
                    type TestType() =
                        member this.TestMethod() = 100
                        [<System.ObsoleteAttribute>]
                        member this.ObsoleteMethod() = 100
                        [<CompilerMessage("This construct is for ML compatibility.", 62, IsHidden=true)>]
                        member this.CompilerMessageMethod() = 100
                        [<CompilerMessage("This construct is hidden", 1023, IsHidden=true)>]
                        member this.HiddenMethod() = 10
                        [<CompilerMessage("This construct is not hidden", 1023, IsHidden=false)>]
                        member this.VisibleMethod() = 10
                        [<CompilerMessage("This construct is not hidden", 1023)>]
                        member this.VisibleMethod2() = 10
                namespace NS2
                module m2 =
                    type x = NS1.MyModule(*MarkerType*)
                    let b = (new NS1.MyModule.TestType()).{caret}
                """,
             true, "TestMethod;VisibleMethod;VisibleMethod2")>]
[<InlineData("""
                namespace NS1
                module MyModule =
                    [<System.ObsoleteAttribute>]
                    type ObsoleteType() =
                        member this.TestMethod() = 10
                    [<CompilerMessage("This construct is for ML compatibility.", 62, IsHidden=true)>]
                    type CompilerMessageType() =
                        member this.TestMethod() = 10
                    type TestType() =
                        member this.TestMethod() = 100
                        [<System.ObsoleteAttribute>]
                        member this.ObsoleteMethod() = 100
                        [<CompilerMessage("This construct is for ML compatibility.", 62, IsHidden=true)>]
                        member this.CompilerMessageMethod() = 100
                        [<CompilerMessage("This construct is hidden", 1023, IsHidden=true)>]
                        member this.HiddenMethod() = 10
                        [<CompilerMessage("This construct is not hidden", 1023, IsHidden=false)>]
                        member this.VisibleMethod() = 10
                        [<CompilerMessage("This construct is not hidden", 1023)>]
                        member this.VisibleMethod2() = 10
                namespace NS2
                module m2 =
                    type x = NS1.MyModule(*MarkerType*)
                    let b = (new NS1.MyModule.TestType()).{caret}
                """,
             false, "ObsoleteMethod;CompilerMessageMethod;HiddenMethod")>]
let ``DefInDiffNameSpace`` (markedSource: string) (shouldContain: bool) (names: string) =
    let info = Checker.getCompletionInfo markedSource
    let expected = names.Split(';') |> List.ofArray

    assertItemsWithNames shouldContain expected info

[<Fact>]
let ``Regression1067.InstanceOfGenericType`` () =
    let info =
        Checker.getCompletionInfo
            """
                type GT<'a> =
                    static member P = 12
                    static member Q = 13
                let _ = GT<int>(*Marker1*)
                type gt_int = GT<int>
                gt_int.{caret}
                type D =
                 class
                 end
                let x = typeof<D>(*Marker3*)
                let y = typeof<D>
                y(*Marker4*)
                """

    assertHasItemWithNames [ "P"; "Q" ] info

[<Fact>]
let ``Regression1067.ClassUsingGenericTypeAsAttribute`` () =
    let info =
        Checker.getCompletionInfo
            """
                type GT<'a> =
                    static member P = 12
                    static member Q = 13
                let _ = GT<int>(*Marker1*)
                type gt_int = GT<int>
                gt_int(*Marker2*)
                type D =
                 class
                 end
                let x = typeof<D>(*Marker3*)
                let y = typeof<D>
                y.{caret}
                """

    assertHasItemWithNames [ "Assembly"; "FullName"; "GUID" ] info
