module FSharp.Compiler.Service.Tests.GotoDefinitionClassesTests

open System
open Xunit

let private classFieldSource =
    String.concat
        "\n"
        [ "let id77 = 0"
          "type C ="
          "  val id77{caret} (*loc-77*) : int" ]

[<Fact>]
let ``GotoDefinition.InsideClass.Bug3176`` () =
    assertGoToDefinitionOnLine
        "val id77 (*loc-77*) : int"
        classFieldSource

let private classSource =
    String.concat
        "\n"
        [ "type Class{caret1} () = (*loc-62*)"
          "  member c.Method () = () (*loc-63*)"
          "  static member Foo () = () (*loc-64*)"
          "let _ ="
          "  let c = Class{caret2} () (*loc-65*)"
          "  c.Method () (*loc-66*)"
          "  Class.Foo () (*loc-67*)" ]

[<Fact>]
let ``GotoDefinition.ObjectOriented.ClassNameDefAndConstructorUse`` () =
    classSource
    |> assertGoToDefinitionOnLines (List.replicate 2 "type Class () = (*loc-62*)")
