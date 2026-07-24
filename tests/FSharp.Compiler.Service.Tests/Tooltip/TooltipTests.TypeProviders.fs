module FSharp.Compiler.Service.Tests.TooltipTypeProvidersTests

open Xunit

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProviders.NestedTypesOrder`` () =
    assertTooltipContainsInOrder
        [ "A"; "X"; "Z" ]
        """type t = N1.TypeWithNestedTypes{caret}"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Type.Comment`` () =
    assertTooltipContains
        "This is a synthetic type created by me!"
        """let a = typeof<N.T{caret}>"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Type.WithLongComment`` () =
    assertTooltipContains
        "This is a synthetic type created by me!. Which is used to test the tool tip of the typeprovider type to check if it shows the right message or not."
        """let a = typeof<N.T{caret}>"""

[<Fact(Skip = "This is not outputting 'member', but 'event'.")>]
let ``TypeProvider.XmlDocAttribute.Type.WithNullComment`` () =
    assertTooltipContains
        "type T =\n  new: unit -> T\n  static member M: unit -> int []\n  static member StaticProp: decimal\n  member Event1: EventHandler"
        """let a = typeof<N.T{caret}>"""

[<Fact(Skip = "This is not outputting 'member', but 'event'.")>]
let ``TypeProvider.XmlDocAttribute.Type.WithEmptyComment`` () =
    assertTooltipContains
        "type T =\n  new : unit -> T\n  static member M: unit -> int []\n  static member StaticProp: decimal\n  member Event1: EventHandler"
        """let a = typeof<N.T{caret}>"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Type.LocalizedComment`` () =
    assertTooltipContains
        "This is a synthetic type Localized! ኤፍ ሻርፕ"
        """let a = typeof<N.T{caret}>"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Constructor.Comment`` () =
    assertTooltipContains
        "This is a synthetic .ctor created by me for N.T"
        """let foo = new N.T{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Constructor.WithLongComment`` () =
    assertTooltipContains
        "This is a synthetic .ctor created by me for N.T. Which is used to test the tool tip of the typeprovider Constructor to check if it shows the right message or not."
        """let foo = new N.T{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Constructor.WithNullComment`` () =
    assertTooltipContains
        "N.T() : N.T"
        """let foo = new N.T{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Constructor.WithEmptyComment`` () =
    assertTooltipContains
        "N.T() : N.T"
        """let foo = new N.T{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Constructor.LocalizedComment`` () =
    assertTooltipContains
        "This is a synthetic .ctor Localized! ኤፍ ሻርፕ for N.T"
        """let foo = new N.T{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Event.Comment`` () =
    assertTooltipContains
        "This is a synthetic *event* created by me for N.T"
        """let t = new N.T()
t.Event1{caret}"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Event.LocalizedComment`` () =
    assertTooltipContains
        "This is a synthetic *event* Localized! ኤፍ ሻርፕ for N.T"
        """let t = new N.T()
t.Event1{caret}"""

[<Fact>]
let ``TypeProvider.ParamsAttributeTest`` () =
    assertTooltipContains
        "[<System.ParamArray>] separator"
        """let t = "a".Spl{caret}it('c', 'd')"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Event.WithLongComment`` () =
    assertTooltipContains
        "This is a synthetic *event* created by me for N.T. Which is used to test the tool tip of the typeprovider Event to check if it shows the right message or not.!"
        """let t = new N.T()
t.Event1{caret}"""

[<Fact(Skip = "This is not outputting 'member', but 'event'.")>]
let ``TypeProvider.XmlDocAttribute.Event.WithNullComment`` () =
    assertTooltipContains
        "member N.T.Event1: IEvent<System.EventHandler,System.EventArgs>"
        """let t = new N.T()
t.Event1{caret}"""

[<Fact(Skip = "This is not outputting 'member', but 'event'.")>]
let ``TypeProvider.XmlDocAttribute.Event.WithEmptyComment`` () =
    assertTooltipContains
        "member N.T.Event1: IEvent<System.EventHandler,System.EventArgs>"
        """let t = new N.T()
t.Event1{caret}"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Method.Comment`` () =
    assertTooltipContains
        "This is a synthetic *method* created by me!!"
        """let t = new N.T.M{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Method.LocalizedComment`` () =
    assertTooltipContains
        "This is a synthetic *method* Localized! ኤፍ ሻርፕ"
        """let t = new N.T.M{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Method.WithLongComment`` () =
    assertTooltipContains
        "This is a synthetic *method* created by me!!. Which is used to test the tool tip of the typeprovider Method to check if it shows the right message or not.!"
        """let t = new N.T.M{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Method.WithNullComment`` () =
    assertTooltipContains
        "N.T.M() : int array"
        """let t = new N.T.M{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Method.WithEmptyComment`` () =
    assertTooltipContains
        "N.T.M() : int array"
        """let t = new N.T.M{caret}()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Property.Comment`` () =
    assertTooltipContains
        "This is a synthetic *property* created by me for N.T"
        """let p = N.T.StaticProp{caret}"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Property.LocalizedComment`` () =
    assertTooltipContains
        "This is a synthetic *property* Localized! ኤፍ ሻርፕ for N.T"
        """let p = N.T.StaticProp{caret}"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Property.WithLongComment`` () =
    assertTooltipContains
        "This is a synthetic *property* created by me for N.T. Which is used to test the tool tip of the typeprovider Property to check if it shows the right message or not.!"
        """let p = N.T.StaticProp{caret}"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Property.WithNullComment`` () =
    assertTooltipContains
        "property N.T.StaticProp: decimal"
        """let p = N.T.StaticProp{caret}"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.XmlDocAttribute.Property.WithEmptyComment`` () =
    assertTooltipContains
        "property N.T.StaticProp: decimal"
        """let p = N.T.StaticProp{caret}"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.StaticParameters.Correct`` () =
    assertTooltipContains
        "type foo = N1.T"
        """type foo{caret} = N1.T< const "Hello World",2>"""

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``TypeProvider.StaticParameters.Negative.Invalid`` () =
    assertTooltipContains
        "type foo"
        """type foo{caret} = N1.T< const 100,2>"""

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``TypeProvider.StaticParameters.XmlComment`` () =
    assertTooltipContains
        "XMLComment"
        """///XMLComment
type foo{caret} = N1.T< const "Hello World",2>"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.StaticParameters.QuickInfo.OnTheErasedType`` () =
    assertTooltipContains
        "type TTT = Samples.FSharp.RegexTypeProvider.RegexTyped<...>\nFull name: File1.TTT"
        """type TTT{caret} = Samples.FSharp.RegexTypeProvider.RegexTyped< @"(?<AreaCode>^\d{3})-(?<PhoneNumber>\d{3}-\d{7}$)">"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.StaticParameters.QuickInfo.OnNestedErasedTypeProperty`` () =
    assertTooltipContains
        "property Samples.FSharp.RegexTypeProvider.RegexTyped<...>.MatchType.AreaCode: System.Text.RegularExpressions.Group"
        """type T = Samples.FSharp.RegexTypeProvider.RegexTyped< @"(?<AreaCode>^\d{3})-(?<PhoneNumber>\d{3}-\d{7}$)">
let reg = T()
let r = reg.Match("425-123-2345").A{caret}reaCode.Value"""
