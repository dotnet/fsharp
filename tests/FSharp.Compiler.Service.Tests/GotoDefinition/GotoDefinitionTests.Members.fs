module FSharp.Compiler.Service.Tests.GotoDefinitionMembersTests

open Xunit

[<Fact>]
let ``GotoDefinition.NoSourceCodeAvailable`` () =
    let source = """System.String.Format{caret}("")"""

    assertGoToDefinitionIsExternal source

let private orPatSource =
    String.concat
        "\n"
        [ "type Nat ="
          "  | Suc of Nat"
          "  | Zro"
          "let _ ="
          "  let f x ="
          "    match x with"
          "    | Suc x{caret1} (*loc-44*)"
          "    | x{caret2} -> "
          "        x"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.OrPat`` () =
    orPatSource
    |> assertGoToDefinitionOnLines (List.replicate 2 "| Suc x (*loc-44*)")

let private consPatSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let f xs ="
          "    match xs with"
          "    | x :: xs (*loc-54*)"
          "      when xs <> [] -> (*loc-52*)"
          "        x{caret1} :: xs{caret2}"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.ConsPatWhenClauseInWhenRhs`` () =
    consPatSource
    |> assertGoToDefinitionOnLines (List.replicate 2 "| x :: xs (*loc-54*)")

let private inStringSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x = 2"
          "  \"x{caret}\"" ]

[<Fact(Skip = "non-FCS: goto-def suppression inside a string literal is an IDE editor-layer (tokenizer) decision; FCS GetDeclarationLocation is string-unaware and navigates the supplied island regardless.")>]
let ``GotoDefinition.Simple.Tricky.InStringFails`` () =
    assertGoToDefinitionFails inStringSource

let private inMultiLineStringSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x = 2"
          "  \"this is a string"
          "    x{caret}"
          "  \"" ]

[<Fact(Skip = "non-FCS: goto-def suppression inside a string literal is an IDE editor-layer (tokenizer) decision; FCS GetDeclarationLocation is string-unaware and navigates the supplied island regardless.")>]
let ``GotoDefinition.Simple.Tricky.InMultiLineStringFails`` () =
    assertGoToDefinitionFails inMultiLineStringSource

[<Fact(Skip = "This is currently broken for dev enlistments.")>]
let ``GotoDefinition.Library.InitialTest`` () =
    let source = "let _ = List.map{caret}"

    assertGoToDefinitionToExternalLine "map" source

let private ooClassSource =
    String.concat
        "\n"
        [ "type Class () = (*loc-62*)"
          "  member c{caret2}.Method{caret1} () = () (*loc-63*)"
          "  static member Foo{caret3} () = () (*loc-64*)"
          "let _ ="
          "  let c = Class () (*loc-65*)"
          "  c.Method{caret4} ()"
          "  Class.Foo{caret5} ()" ]

[<Fact>]
let ``GotoDefinition.ObjectOriented`` () =
    ooClassSource
    |> assertGoToDefinitionOnLines
        [ "member c.Method () = () (*loc-63*)"
          "member c.Method () = () (*loc-63*)"
          "static member Foo () = () (*loc-64*)"
          "member c.Method () = () (*loc-63*)"
          "static member Foo () = () (*loc-64*)" ]

let private ooClassPrimeSource =
    String.concat
        "\n"
        [ "type Class () = (*loc-62*)"
          "  member c.Method () = () (*loc-63*)"
          "  static member Foo () = () (*loc-64*)"
          "type Class' () ="
          "  member c.Method  () = c.Method{caret1} () (*loc-68*)"
          "  member c.Method1 () = c.Method2{caret2} ()"
          "  member c.Method2 () = c.Method1 () (*loc-70*)"
          "  member c.Method3  () ="
          "    let c = Class ()"
          "    c{caret3}.Method{caret4} ()" ]

[<Fact>]
let ``GotoDefinition.ObjectOriented.Prime`` () =
    ooClassPrimeSource
    |> assertGoToDefinitionOnLines
        [ "member c.Method  () = c.Method () (*loc-68*)"
          "member c.Method2 () = c.Method1 () (*loc-70*)"
          "let c = Class ()"
          "member c.Method () = () (*loc-63*)" ]

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
          "D().Foo{caret1} 1"
          "D().Foo{caret2} 1 <- 2"
          "D().Foo{caret3} \"abc\""
          "D().Foo{caret4} \"abc\" <- 2" ]

[<Fact>]
let ``GotoDefinition.OverloadResolutionForProperties`` () =
    overloadedPropertiesSource
    |> assertGoToDefinitionOnLines
        [ "member this.Foo (*loc-d1*)"
          "member this.Foo (*loc-d1*)"
          "member this.Foo (*loc-d3*)"
          "member this.Foo (*loc-d3*)" ]

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
          "d.Method{caret1} 12"
          "d.Method{caret2}()" ]

[<Fact>]
let ``GotoDefinition.OverloadResolutionWithOverrides`` () =
    overloadedMethodsSource
    |> assertGoToDefinitionOnLines
        [ "override this.Method (i:int) = () (*loc-d1*)"
          "member this.Method() = () (*loc-d2*)" ]

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
          "b.Method{caret1}()"
          "b.Property{caret2}" ]

[<Fact>]
let ``GotoDefinition.InheritedMembers`` () =
    inheritedMembersSource
    |> assertGoToDefinitionOnLines
        [ "override this.Method () = ()"
          "override this.Property = 1" ]
