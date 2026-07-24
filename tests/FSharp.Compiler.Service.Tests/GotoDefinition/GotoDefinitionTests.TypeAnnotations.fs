module FSharp.Compiler.Service.Tests.GotoDefinitionTypeAnnotationsTests

open Xunit

let private bug2516SpacedSource =
    """
                //regression test for bug 2516
                type One{caret1} (*Marker1*) = One
                let f (x : One{caret2}) = 2
                """

[<Fact>]
let ``OnTypeDefinitionAndParameter`` () =
    bug2516SpacedSource
    |> assertGoToDefinitionOnLines (List.replicate 2 "type One (*Marker1*) = One")

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
          "d.Foo{caret1}()"
          "d.Foo{caret2}(1)"
          "d.ToString{caret3}()"
          "d.ToString{caret4}(\"aaa\")" ]

[<Fact>]
let ``GotoDefinition.OverloadResolution`` () =
    overloadResolutionSource
    |> assertGoToDefinitionOnLines
        [ "member this.Foo() (*#1#*) = ()"
          "member this.Foo(x) (*#2#*) = ()"
          "override this.ToString() (*#3#*) = System.String.Empty"
          "member this.ToString(s : string) (*#4#*) = ()" ]

let private overloadStaticsSource =
    String.concat
        "\n"
        [ "type T ="
          "   static member Foo(i : int) (*#1#*) = ()"
          "   static member Foo(s : string) (*#2#*) = ()"
          ""
          "T.Foo{caret1} 1"
          "T.Foo{caret2} \"abc\"" ]

[<Fact>]
let ``GotoDefinition.OverloadResolutionStatics`` () =
    overloadStaticsSource
    |> assertGoToDefinitionOnLines
        [ "static member Foo(i : int) (*#1#*) = ()"
          "static member Foo(s : string) (*#2#*) = ()" ]

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
          "new B{caret1}()"
          "new B{caret2}(1)"
          "new B{caret3}(\"abc\")"
          ""
          "type D1() ="
          "    inherit B{caret4}()"
          ""
          "type D2() ="
          "    inherit B{caret5}(1)"
          ""
          "type D3() ="
          "    inherit B{caret6}(\"abc\")"
          ""
          "let o1 = { new B{caret7}() with"
          "             override this.ToString() = \"\""
          "         }"
          "let o2 = { new B{caret8}(1) with"
          "             override this.ToString() = \"\""
          "         }"
          "let o3 = { new B{caret9}(\"aaa\") with"
          "             override this.ToString() = \"\""
          "         }" ]

[<Fact>]
let ``GotoDefinition.Constructors`` () =
    constructorsSource
    |> assertGoToDefinitionOnLines
        [ "type B() (*#1#*) ="
          "new(i : int) (*#2#*) = B()"
          "new(s : string) (*#3#*) = B()"
          "type B() (*#1#*) ="
          "new(i : int) (*#2#*) = B()"
          "new(s : string) (*#3#*) = B()"
          "type B() (*#1#*) ="
          "new(i : int) (*#2#*) = B()"
          "new(s : string) (*#3#*) = B()" ]

let private simplePolymorphSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let a = 2"
          "  let id (x : 'a{caret1}) (*loc-33*)"
          "    : 'a{caret2} = x"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Polymorph`` () =
    simplePolymorphSource
    |> assertGoToDefinitionOnLines (List.replicate 2 "let id (x : 'a) (*loc-33*)")

let private bug2516ModuleSource =
    """
            module GotoDefinition
            type One{caret1}(*Mark1*) = One
            let f (x : One{caret2}) = 2"""

[<Fact>]
let ``Identifier.Bug2516`` () =
    bug2516ModuleSource
    |> assertGoToDefinitionOnLines (List.replicate 2 "type One(*Mark1*) = One")
