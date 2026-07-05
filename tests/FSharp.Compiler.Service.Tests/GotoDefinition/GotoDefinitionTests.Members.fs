module FSharp.Compiler.Service.Tests.GotoDefinitionMembersTests

open System
open Xunit

[<Fact>]
let ``GotoDefinition.NoSourceCodeAvailable`` () =
    let source = """System.String.Format("")"""

    assertGoToDefinitionIsExternal (markCaretAfterLeadingIdent source "ormat")

let private orPatSource =
    String.concat
        "\n"
        [ "type Nat ="
          "  | Suc of Nat"
          "  | Zro"
          "let _ ="
          "  let f x ="
          "    match x with"
          "    | Suc x (*loc-44*)"
          "    | x (*loc-45*) -> "
          "        x"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.OrPatLeft`` () =
    assertGoToDefinitionOnLine
        "| Suc x (*loc-44*)"
        (markCaretAfterLeadingIdent orPatSource "x (*loc-44*)")

[<Fact>]
let ``GotoDefinition.Simple.Tricky.OrPatRight`` () =
    assertGoToDefinitionOnLine
        "| Suc x (*loc-44*)"
        (markCaretAfterLeadingIdent orPatSource "x (*loc-45*)")

let private consPatSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let f xs ="
          "    match xs with"
          "    | x :: xs (*loc-54*)"
          "      when xs <> [] -> (*loc-52*)"
          "        x :: xs (*loc-53*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.ConsPatWhenClauseInWhenRhsX`` () =
    assertGoToDefinitionOnLine
        "| x :: xs (*loc-54*)"
        (markCaretAfterLeadingIdent consPatSource "x :: xs (*loc-53*)")

[<Fact>]
let ``GotoDefinition.Simple.Tricky.ConsPatWhenClauseInWhenRhsXs`` () =
    assertGoToDefinitionOnLine
        "| x :: xs (*loc-54*)"
        (markCaretAfterLeadingIdent consPatSource "xs (*loc-53*)")

let private inStringSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x = 2"
          "  \"x(*loc-72*)\"" ]

[<Fact(Skip = "non-FCS: goto-def suppression inside a string literal is an IDE editor-layer (tokenizer) decision; FCS GetDeclarationLocation is string-unaware and navigates the supplied island regardless.")>]
let ``GotoDefinition.Simple.Tricky.InStringFails`` () =
    assertGoToDefinitionFails (markCaretAfterLeadingIdent inStringSource "x(*loc-72*)")

let private inMultiLineStringSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x = 2"
          "  \"this is a string"
          "    x(*loc-73*)"
          "  \"" ]

[<Fact(Skip = "non-FCS: goto-def suppression inside a string literal is an IDE editor-layer (tokenizer) decision; FCS GetDeclarationLocation is string-unaware and navigates the supplied island regardless.")>]
let ``GotoDefinition.Simple.Tricky.InMultiLineStringFails`` () =
    assertGoToDefinitionFails (markCaretAfterLeadingIdent inMultiLineStringSource "x(*loc-73*)")

[<Fact(Skip = "This is currently broken for dev enlistments.")>]
let ``GotoDefinition.Library.InitialTest`` () =
    let source = "let _ = List.map (*loc-1*)"

    assertGoToDefinitionToExternalLine "map" (markCaretAfterLeadingIdent source "map (*loc-1*)")

let private ooClassSource =
    String.concat
        "\n"
        [ "type Class () = (*loc-62*)"
          "  member c.Method () = () (*loc-63*)"
          "  static member Foo () = () (*loc-64*)"
          "let _ ="
          "  let c = Class () (*loc-65*)"
          "  c.Method () (*loc-66*)"
          "  Class.Foo () (*loc-67*)" ]

[<Theory>]
[<InlineData("member c.Method () = () (*loc-63*)", " () = () (*loc-63*)")>]
[<InlineData("member c.Method () = () (*loc-63*)", ".Method () = () (*loc-63*)")>]
[<InlineData("static member Foo () = () (*loc-64*)", " () = () (*loc-64*)")>]
[<InlineData("member c.Method () = () (*loc-63*)", " () (*loc-66*)")>]
[<InlineData("static member Foo () = () (*loc-64*)", " () (*loc-67*)")>]
let ``GotoDefinition.ObjectOriented`` (definitionLine: string) (marker: string) =
    assertGoToDefinitionOnLine definitionLine (markCaretAfterLeadingIdent ooClassSource marker)

let private ooClassPrimeSource =
    String.concat
        "\n"
        [ "type Class () = (*loc-62*)"
          "  member c.Method () = () (*loc-63*)"
          "  static member Foo () = () (*loc-64*)"
          "type Class' () ="
          "  member c.Method  () = c.Method () (*loc-68*)"
          "  member c.Method1 () = c.Method2 () (*loc-69*)"
          "  member c.Method2 () = c.Method1 () (*loc-70*)"
          "  member c.Method3  () ="
          "    let c = Class ()"
          "    c.Method () (*loc-71*)" ]

[<Theory>]
[<InlineData("member c.Method  () = c.Method () (*loc-68*)", " () (*loc-68*)")>]
[<InlineData("member c.Method2 () = c.Method1 () (*loc-70*)", " () (*loc-69*)")>]
[<InlineData("let c = Class ()", ".Method () (*loc-71*)")>]
[<InlineData("member c.Method () = () (*loc-63*)", " () (*loc-71*)")>]
let ``GotoDefinition.ObjectOriented.Prime`` (definitionLine: string) (marker: string) =
    assertGoToDefinitionOnLine definitionLine (markCaretAfterLeadingIdent ooClassPrimeSource marker)

let private overloadedPropertiesSource =
    String.concat
        "\n"
        [ "type D() ="
          "  member this.Foo (*loc-d1*)"
          "    with get(i:int) = 1"
          "    and set (i:int) v = ()"
          ""
          "  member this.Foo (*loc-d3*)"
          "    with get (s:string) = 1"
          "    and  set (s:string) v = ()"
          ""
          "D().Foo 1 (*loc-u1*)"
          "D().Foo 1 <- 2 (*loc-u2*)"
          "D().Foo \"abc\" (*loc-u3*)"
          "D().Foo \"abc\" <- 2 (*loc-u4*)" ]

[<Fact>]
let ``GotoDefinition.OverloadResolutionForProperties`` () =
    assertGoToDefinitionOnLine
        "member this.Foo (*loc-d1*)"
        (markCaretAfterLeadingIdent overloadedPropertiesSource "Foo 1 (*loc-u1*)")

    assertGoToDefinitionOnLine
        "member this.Foo (*loc-d1*)"
        (markCaretAfterLeadingIdent overloadedPropertiesSource "Foo 1 <- 2 (*loc-u2*)")

    assertGoToDefinitionOnLine
        "member this.Foo (*loc-d3*)"
        (markCaretAfterLeadingIdent overloadedPropertiesSource "Foo \"abc\" (*loc-u3*)")

    assertGoToDefinitionOnLine
        "member this.Foo (*loc-d3*)"
        (markCaretAfterLeadingIdent overloadedPropertiesSource "Foo \"abc\" <- 2 (*loc-u4*)")

let private overloadedMethodsSource =
    String.concat
        "\n"
        [ "[<AbstractClass>]"
          "type Base<'T>() ="
          "   member this.Method() = () (*loc-d2*)"
          "   abstract Method : 'T -> unit"
          ""
          "type Derived() ="
          "   inherit Base<int>()"
          ""
          "   override this.Method (i:int) = () (*loc-d1*)"
          ""
          "let d = new Derived()"
          "d.Method 12 (*loc-u1*)"
          "d.Method() (*loc-u2*)" ]

[<Fact>]
let ``GotoDefinition.OverloadResolutionWithOverrides`` () =
    assertGoToDefinitionOnLine
        "override this.Method (i:int) = () (*loc-d1*)"
        (markCaretAfterLeadingIdent overloadedMethodsSource "Method 12 (*loc-u1*)")

    assertGoToDefinitionOnLine
        "member this.Method() = () (*loc-d2*)"
        (markCaretAfterLeadingIdent overloadedMethodsSource "Method() (*loc-u2*)")

let private inheritedMembersSource =
    String.concat
        "\n"
        [ "[<AbstractClass>]"
          "type Foo() ="
          "   abstract Method : unit -> unit"
          "   abstract Property : int"
          "type Bar() ="
          "   inherit Foo()"
          "   override this.Method () = ()"
          "   override this.Property = 1"
          "let b = Bar()"
          "b.Method(*loc-1*)()"
          "b.Property(*loc-2*)" ]

[<Fact>]
let ``GotoDefinition.InheritedMembers`` () =
    assertGoToDefinitionOnLine
        "override this.Method () = ()"
        (markCaretAfterLeadingIdent inheritedMembersSource "Method(*loc-1*)")

    assertGoToDefinitionOnLine
        "override this.Property = 1"
        (markCaretAfterLeadingIdent inheritedMembersSource "Property(*loc-2*)")
