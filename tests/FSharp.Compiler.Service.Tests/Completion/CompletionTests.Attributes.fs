module FSharp.Compiler.Service.Tests.CompletionAttributesTests

open Xunit

[<Theory>]
[<InlineData("""
                    open System
                    [<Attr{caret}     // expect AttributeUsage from System namespace
                    let f() = 4""")>]
[<InlineData("""
                    open System
                    [<Attr{caret}     // expect AttributeUsage from System namespace
                    type MyAttr() = inherit Attribute()""")>]
[<InlineData("""
                    namespace Foo
                    open System
                    [<Attr{caret}     // expect AttributeUsage from System namespace
                    let f() = 4""")>]
[<InlineData("""
                    namespace Foo
                    open System
                    [<Attr{caret}     // expect AttributeUsage from System namespace
                    type MyAttr() = inherit Attribute()""")>]
[<InlineData("""
                    namespace Foo
                    open System
                    [<Attr{caret}     // expect AttributeUsage from System namespace
                    // nothing here""")>]
[<InlineData("""
                    namespace Foo
                    open System
                    [<Attr{caret}     // expect AttributeUsage from System namespace
                    module Foo =
                        let x = 42""")>]
[<InlineData("""
                    open System
                    [<Attr{caret}     // expect AttributeUsage from System namespace
                    module Foo =
                        let x = 42""")>]
let ``Attribute.WhenAttachedTo.Bug70080`` (noneTargetSource: string) =
    for prefix in [ ""; "type:"; "module:" ] do
        noneTargetSource.Replace("[<Attr", "[<" + prefix + "Attr")
        |> Checker.getCompletionInfo
        |> assertHasItemWithNames [ "AttributeUsage" ]

[<Fact>]
let ``ObsoleteAndOCamlCompatDontAppear`` () =
    let info =
        Checker.getCompletionInfo
            """open System
type X = 
    static member private Private() = ()
    [<Obsolete>]
    static member Obsolete() = ()
    [<CompilerMessage("This construct is for ML compatibility.", 62, IsHidden=true)>]
    static member CompilerMessageTest() = ()
X.{caret}"""

    assertHasNoItemsWithNames [ "Obsolete"; "CompilerMessageTest" ] info

[<Fact>]
let ``Attributes.CanSeeOpenNamespaces.Bug268290.Case1`` () =
    let info =
        Checker.getCompletionInfo
            """
                    module Foo
                    open System
                    [<{caret}
             """

    assertHasItemWithNames [ "AttributeUsage" ] info

[<Fact>]
let ``LongIdent.AsAttribute`` () =
    let info =
        Checker.getCompletionInfo
            """
                [<System.{caret}>]
                type TestAttribute() =
                    member x.print() = "print" """

    assertHasItemWithNames [ "ObsoleteAttribute" ] info

[<Fact(Skip = "Bug 3627 - Completion lists should be filtered in many contexts")>]
let ``NotShowAttribute`` () =
    let info1 =
        Checker.getCompletionInfo
            """
                open System
                [<System.ObsoleteAttribute.{caret}>]
                type testclass() =
                    member x.Name() = "test"
                [<ObsoleteAttribute("stuff")(*Mattribute2*)>]
                type testattribute() =
                    member x.Empty = 0
                """

    Assert.Equal(0, info1.Items.Length)

    let info2 =
        Checker.getCompletionInfo
            """
                open System
                [<System.ObsoleteAttribute(*Mattribute1*)>]
                type testclass() =
                    member x.Name() = "test"
                [<ObsoleteAttribute("stuff").{caret}>]
                type testattribute() =
                    member x.Empty = 0
                """

    Assert.Equal(0, info2.Items.Length)

[<Theory>]
[<InlineData(true, "Attributes;CallingConvention;IsFamily")>]
[<InlineData(false, "value__")>]
let ``Regression2296.DirectResultsOfMethodCall`` (shouldContain: bool) (names: string) =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                // regression test of bug 2296: No completion lists on the direct results of a method call
                // This is a function that has a custom attribute on the return type.
                let foo(a) : [<Core.OCamlCompatibilityAttribute("Attribute on return type!")>] int
                   = a + 5
                // The rest of the code is a mere verification that the compiler thru reflection
                let executingAssembly = System.Reflection.Assembly.GetExecutingAssembly()
                let programType = executingAssembly.GetType("Program")
                let message = programType.GetMethod("foo").{caret}
                """

    let expected = names.Split(';') |> List.ofArray

    assertItemsWithNames shouldContain expected info

[<Theory>]
[<InlineData(true, "CompareTo;GetType;ToString")>]
[<InlineData(false, "value__")>]
let ``Regression2296.Identifier.String.Reflection01`` (shouldContain: bool) (names: string) =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                // regression test of bug 2296: No completion lists on the direct results of a method call
                // This is a function that has a custom attribute on the return type.
                let foo(a) : [<Core.OCamlCompatibilityAttribute("Attribute on return type!")>] int
                    = a + 5
                // The rest of the code is a mere verification that the compiler thru reflection
                let executingAssembly = System.Reflection.Assembly.GetExecutingAssembly()
                let programType = executingAssembly.GetType("Program")
                let message = programType.GetMethod("foo")(*Marker1*)
                let x = ""
                let _ = x.Contains("a").{caret}"""

    let expected = names.Split(';') |> List.ofArray

    assertItemsWithNames shouldContain expected info

[<Theory>]
[<InlineData(true, "CompareTo;GetType;ToString")>]
[<InlineData(false, "value__")>]
let ``Regression2296.Identifier.String.Reflection02`` (shouldContain: bool) (names: string) =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                // regression test of bug 2296: No completion lists on the direct results of a method call
                // This is a function that has a custom attribute on the return type.
                let foo(a) : [<Core.OCamlCompatibilityAttribute("Attribute on return type!")>] int
                   = a + 5
                // The rest of the code is a mere verification that the compiler thru reflection
                let executingAssembly = System.Reflection.Assembly.GetExecutingAssembly()
                let programType = executingAssembly.GetType("Program")
                let message = programType.GetMethod("foo")(*Marker1*)
                let x = ""
                let _ = x.Contains("a")(*Marker2*)
                let _ = x.CompareTo("a").{caret}"""

    let expected = names.Split(';') |> List.ofArray

    assertItemsWithNames shouldContain expected info

[<Theory>]
[<InlineData(true, "CompareTo;GetType;ToString")>]
[<InlineData(false, "value__")>]
let ``Regression2296.System.StaticMethod.Reflection`` (shouldContain: bool) (names: string) =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                // regression test of bug 2296: No completion lists on the direct results of a method call
                // This is a function that has a custom attribute on the return type.
                let foo(a) : [<Core.OCamlCompatibilityAttribute("Attribute on return type!")>] int
                   = a + 5
                // The rest of the code is a mere verification that the compiler thru reflection
                let executingAssembly = System.Reflection.Assembly.GetExecutingAssembly()
                let programType = executingAssembly.GetType("Program")
                let message = programType.GetMethod("foo")(*Marker1*)
                let x = ""
                let _ = x.Contains("a")(*Marker2*)
                let _ = x.CompareTo("a")(*Marker3*)
                open System.IO
                let GetFileSize (filePath: string) = File.GetAttributes(filePath).{caret}"""

    let expected = names.Split(';') |> List.ofArray

    assertItemsWithNames shouldContain expected info

[<Fact>]
let ``LongIdent.PInvoke.AsAttribute`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System.IO
                open System.Runtime.InteropServices

                module mymodule =
                    type SomeAttrib() =
                        inherit System.Attribute()
                    type myclass() =
                        member x.name() = "test case"
                module mymodule2 =
                    [<DllImport("kernel32.dll", EntryPoint="CopyFile")>]
                    extern bool CopyFile_Attrib([<mymodule.{caret}>] char [] lpExistingFileName, char []lpNewFileName, [<mymodule.SomeAttrib>] bool & bFailIfExists);

                    let result5 = CopyFile_Arrays(tempFile1.ToCharArray(), tempFile2.ToCharArray(), false)
                    printfn "WithAttribute %A" result5"""

    assertHasItemWithNames [ "SomeAttrib" ] info
