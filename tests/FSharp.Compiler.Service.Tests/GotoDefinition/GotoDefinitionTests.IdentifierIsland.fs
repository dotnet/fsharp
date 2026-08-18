module FSharp.Compiler.Service.Tests.GotoDefinitionIdentifierIslandTests

open Xunit

[<Theory>]
[<InlineData("let $ThisIsAnIdentifier = ()", "ThisIsAnIdentifier")>]
[<InlineData("let This$IsAnIdentifier = ()", "ThisIsAnIdentifier")>]
[<InlineData("let ThisIsAnIdentifier = Te$st.Moo.Foo.bar", "Test")>]
[<InlineData("let ThisIsAnIdentifier = Test.Mo$o.Foo.bar", "Test.Moo")>]
[<InlineData("let ThisIsAnIdentifier = Test.Moo.Fo$o.bar", "Test.Moo.Foo")>]
[<InlineData("let ThisIsAnIdentifier = Test.Moo.Foo.ba$r", "Test.Moo.Foo.bar")>]
[<InlineData("let ThisIsAnIdentifier = 3 +$ 4", null)>]
let ``GetCompleteIdTest source-only`` (source: string) (expected: string) =
    assertCompleteIdentifierIsland (Option.ofObj expected) source

[<Fact>]
let ``GetCompleteIdTest.TrivialEnd`` () =
    assertCompleteIdentifierIslandWithTolerate true (Some "ThisIsAnIdentifier") "let ThisIsAnIdentifier$ = ()"
    assertCompleteIdentifierIslandWithTolerate false None "let ThisIsAnIdentifier$ = ()"

[<Fact>]
let ``GetCompleteIdTest.GetsUpToDot5`` () =
    assertCompleteIdentifierIslandWithTolerate true (Some "Test.Moo.Foo.bar") "let ThisIsAnIdentifier = Test.Moo.Foo.bar$"
    assertCompleteIdentifierIslandWithTolerate false None "let ThisIsAnIdentifier = Test.Moo.Foo.bar$"
