module FSharp.Compiler.Service.Tests.GotoDefinitionTypeAnnotationsTests

open System
open Xunit

let private bug2516SpacedSource =
    """
                //regression test for bug 2516
                type One (*Marker1*) = One
                let f (x : One (*Marker2*)) = 2
                """

[<Fact>]
let ``OnTypeDefinition`` () =
    assertGoToDefinitionOnLine
        "type One (*Marker1*) = One"
        (markCaretAfterLeadingIdent bug2516SpacedSource "One (*Marker1*)")

[<Fact>]
let ``Parameter`` () =
    assertGoToDefinitionOnLine
        "type One (*Marker1*) = One"
        (markCaretAfterLeadingIdent bug2516SpacedSource "One (*Marker2*)")

let private overloadResolutionSource =
    String.concat
        "\n"
        [ "type D() ="
          "   override this.ToString() (*#3#*) = System.String.Empty"
          "   member this.ToString(s : string) (*#4#*) = ()"
          ""
          "   member this.Foo() (*#1#*) = ()"
          "   member this.Foo(x) (*#2#*) = ()"
          ""
          "let d = new D()"
          "d.Foo() (*$1$*)"
          "d.Foo(1) (*$2$*)"
          "d.ToString() (*$3$*)"
          "d.ToString(\"aaa\") (*$4$*)" ]

[<Fact>]
let ``GotoDefinition.OverloadResolution`` () =
    assertGoToDefinitionOnLine
        "member this.Foo() (*#1#*) = ()"
        (markCaretAfterLeadingIdent overloadResolutionSource "Foo() (*$1$*)")

    assertGoToDefinitionOnLine
        "member this.Foo(x) (*#2#*) = ()"
        (markCaretAfterLeadingIdent overloadResolutionSource "Foo(1) (*$2$*)")

    assertGoToDefinitionOnLine
        "override this.ToString() (*#3#*) = System.String.Empty"
        (markCaretAfterLeadingIdent overloadResolutionSource "ToString() (*$3$*)")

    assertGoToDefinitionOnLine
        "member this.ToString(s : string) (*#4#*) = ()"
        (markCaretAfterLeadingIdent overloadResolutionSource "ToString(\"aaa\") (*$4$*)")

let private overloadStaticsSource =
    String.concat
        "\n"
        [ "type T ="
          "   static member Foo(i : int) (*#1#*) = ()"
          "   static member Foo(s : string) (*#2#*) = ()"
          ""
          "T.Foo 1 (*$1$*)"
          "T.Foo \"abc\" (*$2$*)" ]

[<Fact>]
let ``GotoDefinition.OverloadResolutionStatics`` () =
    assertGoToDefinitionOnLine
        "static member Foo(i : int) (*#1#*) = ()"
        (markCaretAfterLeadingIdent overloadStaticsSource "Foo 1 (*$1$*)")

    assertGoToDefinitionOnLine
        "static member Foo(s : string) (*#2#*) = ()"
        (markCaretAfterLeadingIdent overloadStaticsSource "Foo \"abc\" (*$2$*)")

let private constructorsSource =
    String.concat
        "\n"
        [ "type B() (*#1#*) ="
          "  new(i : int) (*#2#*) = B()"
          "  new(s : string) (*#3#*) = B()"
          ""
          "B()"
          "B(1)"
          "B(\"abc\")"
          ""
          "new B() (*$1b$*)"
          "new B(1) (*$2b$*)"
          "new B(\"abc\") (*$3b$*)"
          ""
          "type D1() ="
          "    inherit B() (*$1c$*)"
          ""
          "type D2() ="
          "    inherit B(1) (*$2c$*)"
          ""
          "type D3() ="
          "    inherit B(\"abc\") (*$3c$*)"
          ""
          "let o1 = { new B() (*$1d$*) with"
          "             override this.ToString() = \"\""
          "         }"
          "let o2 = { new B(1) (*$2d$*) with"
          "             override this.ToString() = \"\""
          "         }"
          "let o3 = { new B(\"aaa\") (*$3d$*) with"
          "             override this.ToString() = \"\""
          "         }" ]

[<Fact>]
let ``GotoDefinition.Constructors`` () =
    assertGoToDefinitionOnLine
        "type B() (*#1#*) ="
        (markCaretAfterLeadingIdent constructorsSource "B() (*$1b$*)")

    assertGoToDefinitionOnLine
        "new(i : int) (*#2#*) = B()"
        (markCaretAfterLeadingIdent constructorsSource "B(1) (*$2b$*)")

    assertGoToDefinitionOnLine
        "new(s : string) (*#3#*) = B()"
        (markCaretAfterLeadingIdent constructorsSource "B(\"abc\") (*$3b$*)")

    assertGoToDefinitionOnLine
        "type B() (*#1#*) ="
        (markCaretAfterLeadingIdent constructorsSource "B() (*$1c$*)")

    assertGoToDefinitionOnLine
        "new(i : int) (*#2#*) = B()"
        (markCaretAfterLeadingIdent constructorsSource "B(1) (*$2c$*)")

    assertGoToDefinitionOnLine
        "new(s : string) (*#3#*) = B()"
        (markCaretAfterLeadingIdent constructorsSource "B(\"abc\") (*$3c$*)")

    assertGoToDefinitionOnLine
        "type B() (*#1#*) ="
        (markCaretAfterLeadingIdent constructorsSource "B() (*$1d$*)")

    assertGoToDefinitionOnLine
        "new(i : int) (*#2#*) = B()"
        (markCaretAfterLeadingIdent constructorsSource "B(1) (*$2d$*)")

    assertGoToDefinitionOnLine
        "new(s : string) (*#3#*) = B()"
        (markCaretAfterLeadingIdent constructorsSource "B(\"aaa\") (*$3d$*)")

let private simplePolymorphSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let a = 2"
          "  let id (x : 'a) (*loc-33*)"
          "    : 'a = x (*loc-34*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Polymorph.Leftmost`` () =
    assertGoToDefinitionOnLine
        "let id (x : 'a) (*loc-33*)"
        (markCaretAfterLeadingIdent simplePolymorphSource "a) (*loc-33*)")

[<Fact>]
let ``GotoDefinition.Simple.Polymorph.NotLeftmost`` () =
    assertGoToDefinitionOnLine
        "let id (x : 'a) (*loc-33*)"
        (markCaretAfterLeadingIdent simplePolymorphSource "a = x (*loc-34*)")

let private bug2516ModuleSource =
    """
            module GotoDefinition
            type One(*Mark1*) = One
            let f (x : One(*Mark2*)) = 2"""

[<Fact>]
let ``Identifier.IsConstructor.Bug2516`` () =
    assertGoToDefinitionOnLine
        "type One(*Mark1*) = One"
        (markCaretAfterLeadingIdent bug2516ModuleSource "One(*Mark1*)")

[<Fact>]
let ``Identifier.IsTypeName.Bug2516`` () =
    assertGoToDefinitionOnLine
        "type One(*Mark1*) = One"
        (markCaretAfterLeadingIdent bug2516ModuleSource "One(*Mark2*)")
