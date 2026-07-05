module FSharp.Compiler.Service.Tests.GotoDefinitionTypeProvidersTests

open Xunit

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute`` () =
    let targetLine = "// A0(*ColumnMarker*)1234567890"
    assertGoToDefinitionOnLine targetLine
        (markCaretAfterLeadingIdent "\nlet a = typeof<N.T(*GotoValDef*)>\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 " "T(*GotoValDef*)")
    assertGoToDefinitionOnLine targetLine
        (markCaretAfterLeadingIdent "\nlet a = typeof<N.``T T``>\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 " "T``")
    assertGoToDefinitionOnLine targetLine
        (markCaretAfterLeadingIdent "\nlet foo = new N.T(*GotoValDef*)()\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 " "T(*GotoValDef*)")
    assertGoToDefinitionOnLine targetLine
        (markCaretAfterLeadingIdent "\nlet t = new N.T.M(*GotoValDef*)()\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 " "M(*GotoValDef*)")
    assertGoToDefinitionOnLine targetLine
        (markCaretAfterLeadingIdent "\nlet p = N.T.StaticProp(*GotoValDef*)\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 " "StaticProp(*GotoValDef*)")
    assertGoToDefinitionOnLine targetLine
        (markCaretAfterLeadingIdent "\nlet t = new N.T()\nt.Event1(*GotoValDef*)\n// A0(*ColumnMarker*)1234567890\n// B01234567890\n// C01234567890 " "Event1(*GotoValDef*)")

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.ProvidedTypeNoDefinitionLocationAttribute`` () =
    let source = "\ntype T = N1.T<\"\", 1>\n"
    assertGoToDefinitionFails (markCaretAfterLeadingIdent source "T<")

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``GotoDefinition.ProvidedMemberNoDefinitionLocationAttribute`` () =
    assertGoToDefinitionFails (markCaretAfterLeadingIdent "\ntype T = N1.T<\"\", 1>\nT.Param1\n" "ram1")
    assertGoToDefinitionFails (markCaretAfterLeadingIdent "\ntype T = N1.T1\nT.M1(1)\n" "1(")

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Type.FileDoesnotExist`` () =
    let source = "\nlet a = typeof<N.T(*GotoValDef*)>\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails (markCaretAfterLeadingIdent source "T(*GotoValDef*)")

[<Fact(Skip = "Need some work to detect the line does not exist.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Type.LineDoesnotExist`` () =
    let source = "\nlet a = typeof<N.T(*GotoValDef*)>\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails (markCaretAfterLeadingIdent source "T(*GotoValDef*)")

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Constructor.FileDoesnotExist`` () =
    let source = "\nlet foo = new N.T(*GotoValDef*)()\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails (markCaretAfterLeadingIdent source "T(*GotoValDef*)")

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Method.FileDoesnotExist`` () =
    let source = "\nlet t = new N.T.M(*GotoValDef*)()\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails (markCaretAfterLeadingIdent source "M(*GotoValDef*)")

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Property.FileDoesnotExist`` () =
    let source = "\nlet p = N.T.StaticProp(*GotoValDef*)\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails (markCaretAfterLeadingIdent source "StaticProp(*GotoValDef*)")

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Event.FileDoesnotExist`` () =
    let source = "\nlet t = new N.T()\nt.Event1(*GotoValDef*)\n// A0(*Marker*)1234567890\n// B01234567890\n// C01234567890 "
    assertGoToDefinitionFails (markCaretAfterLeadingIdent source "Event1(*GotoValDef*)")
