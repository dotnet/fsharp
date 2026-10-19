module FSharp.Compiler.Service.Tests.GotoDefinitionTypeProvidersTests

open Xunit

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute`` () =
    let targetLine = "// A0(*ColumnMarker*)1234567890"
    assertGoToDefinitionOnLine targetLine
        "\nlet a = typeof<N.T{caret}(*GotoValDef*)>\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionOnLine targetLine
        "\nlet a = typeof<N.``T T{caret}``>\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionOnLine targetLine
        "\nlet foo = new N.T{caret}(*GotoValDef*)()\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionOnLine targetLine
        "\nlet t = new N.T.M{caret}(*GotoValDef*)()\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionOnLine targetLine
        "\nlet p = N.T.StaticProp{caret}(*GotoValDef*)\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionOnLine targetLine
        "\nlet t = new N.T()\nt.Event1{caret}(*GotoValDef*)\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 "

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.ProvidedTypeNoDefinitionLocationAttribute`` () =
    let source = "\ntype T = N1.T{caret}<\"\", 1>\n"
    assertGoToDefinitionFails source

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``GotoDefinition.ProvidedMemberNoDefinitionLocationAttribute`` () =
    assertGoToDefinitionFails "\ntype T = N1.T<\"\", 1>\nT.Param1{caret}\n"
    assertGoToDefinitionFails "\ntype T = N1.T1\nT.M1{caret}(1)\n"

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Type.FileDoesnotExist`` () =
    let source = "\nlet a = typeof<N.T{caret}(*GotoValDef*)>\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails source

[<Fact(Skip = "Need some work to detect the line does not exist.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Type.LineDoesnotExist`` () =
    let source = "\nlet a = typeof<N.T{caret}(*GotoValDef*)>\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails source

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Constructor.FileDoesnotExist`` () =
    let source = "\nlet foo = new N.T{caret}(*GotoValDef*)()\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails source

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Method.FileDoesnotExist`` () =
    let source = "\nlet t = new N.T.M{caret}(*GotoValDef*)()\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails source

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Property.FileDoesnotExist`` () =
    let source = "\nlet p = N.T.StaticProp{caret}(*GotoValDef*)\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails source

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Event.FileDoesnotExist`` () =
    let source = "\nlet t = new N.T()\nt.Event1{caret}(*GotoValDef*)\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails source
