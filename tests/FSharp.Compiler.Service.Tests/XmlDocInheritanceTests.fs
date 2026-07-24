module FSharp.Compiler.Service.Tests.XmlDocInheritanceTests

open System.Text.RegularExpressions
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open FSharp.Compiler.XmlDocInheritance
open Xunit

let expandWith (crefMap: (string * string) list) (implicitTarget: string option) (xml: string) : string =
    let map = Map.ofList crefMap
    let resolve cref = Map.tryFind cref map
    expandInheritDocFromXmlText resolve implicitTarget range0 Set.empty xml

let getTooltipXml (markedSource: string) =
    let _, xml, _ = Checker.getTooltip markedSource |> TooltipTests.assertAndExtractTooltip
    xml

let getCompletionXml name markedSource =
    let completionInfo = Checker.getCompletionInfo markedSource

    let item =
        completionInfo.Items
        |> Array.find (fun item -> item.NameInCode = name)

    let _, xml, _ = item.Description |> TooltipTests.assertAndExtractTooltip
    xml

let getSymbolXml name markedSource =
    let _, checkResults = Checker.getCheckedResolveContext markedSource
    let symbol = XmlDocTests.findSymbolByName name checkResults

    match symbol with
    | :? FSharpEntity as entity -> entity.XmlDoc
    | :? FSharpMemberOrFunctionOrValue as value -> value.XmlDoc
    | :? FSharpUnionCase as unionCase -> unionCase.XmlDoc
    | :? FSharpField as field -> field.XmlDoc
    | :? FSharpActivePatternCase as activePatternCase -> activePatternCase.XmlDoc
    | _ -> failwith $"Unexpected symbol type {symbol.GetType()}"

let xmlText (xml: FSharpXmlDoc) =
    match xml with
    | FSharpXmlDoc.FromXmlText xmlDoc -> xmlDoc.GetXmlText()
    | other -> failwith $"Expected FromXmlText, got {other}"

[<Fact>]
let ``engine recursively expands multi-level inheritdoc chain`` () =
    let result =
        expandWith
            [
                "B", """<inheritdoc cref="C"/>"""
                "C", """<summary>Leaf summary text</summary>"""
            ]
            None
            """<inheritdoc cref="B"/>"""

    Assert.Contains("Leaf summary text", result)
    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``engine expands shared diamond target in each branch`` () =
    let result =
        expandWith
            [
                "B", """<inheritdoc cref="C"/>"""
                "C", """<v>shared</v>"""
                "D", """<inheritdoc cref="C"/>"""
            ]
            None
            """<part1><inheritdoc cref="B"/></part1><part2><inheritdoc cref="D"/></part2>"""

    Assert.Equal(2, Regex.Matches(result, "shared").Count)
    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``engine removes self-cycle inheritdoc`` () =
    let result =
        expandWith
            [ "A", """<inheritdoc cref="A"/>""" ]
            None
            """<inheritdoc cref="A"/>"""

    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``engine removes indirect cycle inheritdoc`` () =
    let result =
        expandWith
            [
                "A", """<inheritdoc cref="B"/>"""
                "B", """<inheritdoc cref="A"/>"""
            ]
            None
            """<inheritdoc cref="A"/>"""

    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``engine path selects summary without remarks`` () =
    let result =
        expandWith
            [
                "A", """<summary>Selected summary</summary><remarks>Skipped remarks</remarks>"""
            ]
            None
            """<inheritdoc cref="A" path="/summary"/>"""

    Assert.Contains("Selected summary", result)
    Assert.DoesNotContain("Skipped remarks", result)
    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``engine default inheritdoc excludes top-level overloads`` () =
    let result =
        expandWith
            [
                "A", """<overloads>Skipped overload text</overloads><summary>Kept summary text</summary>"""
            ]
            None
            """<inheritdoc cref="A"/>"""

    Assert.Contains("Kept summary text", result)
    Assert.DoesNotContain("Skipped overload text", result)
    Assert.DoesNotContain("<overloads", result)
    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``engine removes unresolvable cref inheritdoc and preserves surrounding content`` () =
    let result =
        expandWith [] None """<summary>Before <inheritdoc cref="Missing"/> After</summary>"""

    Assert.Contains("Before", result)
    Assert.Contains("After", result)
    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``engine removes invalid XPath inheritdoc without inherited content`` () =
    let result =
        expandWith
            [ "A", """<summary>Inherited summary</summary>""" ]
            None
            """<summary>Before <inheritdoc cref="A" path="///["/> After</summary>"""

    Assert.Contains("Before", result)
    Assert.Contains("After", result)
    Assert.DoesNotContain("Inherited summary", result)
    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``engine removes malformed inherited content inheritdoc`` () =
    let result =
        expandWith
            [ "A", """<summary>Malformed summary""" ]
            None
            """Before <inheritdoc cref="A"/> After"""

    Assert.Contains("Before", result)
    Assert.Contains("After", result)
    Assert.DoesNotContain("Malformed summary", result)
    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``engine removes implicit inheritdoc without target`` () =
    let result = expandWith [] None """<summary>Before <inheritdoc/> After</summary>"""

    Assert.Contains("Before", result)
    Assert.Contains("After", result)
    Assert.DoesNotContain("<inheritdoc", result)

[<Fact>]
let ``tooltip expands implicit inheritdoc from base class`` () =
    let xml =
        getTooltipXml
            """
module Test
/// <summary>Base summary text</summary>
type Base() = class end
/// <inheritdoc/>
type Derive{caret}d() = inherit Base()
"""

    Assert.Contains("Base summary text", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip expands implicit inheritdoc from implemented interface`` () =
    let xml =
        getTooltipXml
            """
module Test
/// <summary>Interface summary text</summary>
type IThing =
    abstract member Do: unit -> unit
/// <inheritdoc/>
type Thin{caret}g() =
    interface IThing with
        member _.Do() = ()
"""

    Assert.Contains("Interface summary text", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip expands implicit inheritdoc on overriding method`` () =
    let xml =
        getTooltipXml
            """
module Test
type Base() =
    /// <summary>Base method summary</summary>
    abstract member Foo: unit -> unit
    default _.Foo() = ()
type Derived() =
    inherit Base()
    /// <inheritdoc/>
    override _.Foo() = ()
let d = Derived()
d.Fo{caret}o()
"""

    Assert.Contains("Base method summary", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip expands implicit inheritdoc on overriding property`` () =
    let xml =
        getTooltipXml
            """
module Test
type Base() =
    /// <summary>Base property summary</summary>
    abstract member Value: int
    default _.Value = 0
type Derived() =
    inherit Base()
    /// <inheritdoc/>
    override _.Value = 1
let d = Derived()
d.Val{caret}ue
"""

    Assert.Contains("Base property summary", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``completion expands implicit inheritdoc from base class`` () =
    let xml =
        getCompletionXml
            "Derived"
            """
module Test
/// <summary>Base summary text</summary>
type Base() = class end
/// <inheritdoc/>
type Derived() = inherit Base()
let _ : Deri{caret} = failwith ""
"""

    Assert.Contains("Base summary text", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``symbol resolves explicit cref inheritdoc to a same-file type`` () =
    let xml =
        getSymbolXml
            "Derived"
            """
module Test
/// <summary>Base summary text</summary>
type Base() = class end
/// <inheritdoc cref="T:Test.Base"/>
type Derived() = class end
let _ = Derived(){caret}
"""

    Assert.Contains("Base summary text", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact(Skip = "documented limitation: explicit cref not resolvable at InfoReader/tooltip layer; use FSharpSymbol.XmlDoc")>]
let ``tooltip expands explicit cref inheritdoc to a same-file type`` () =
    let xml =
        getTooltipXml
            """
module Test
/// <summary>Base summary text</summary>
type Base() = class end
/// <inheritdoc cref="T:Test.Base"/>
type Derive{caret}d() = class end
"""

    Assert.Contains("Base summary text", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)
