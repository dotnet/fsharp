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

// --- Phase 4: implicit resolution priority parity (Roslyn GetCandidateSymbol) ---

[<Fact>]
let ``engine does not leak implicit target across cref chains`` () =
    // A explicitly inherits from B; B has a bare <inheritdoc/> (implicit). When expanding B's
    // content, the implicit target must be B's (unknown at the text-only engine layer -> None),
    // NOT A's implicit target. The bogus implicit target below must never be consulted.
    let result =
        expandWith
            [ "B", """<summary>B summary</summary><inheritdoc/>""" ]
            (Some "SHOULD_NOT_BE_USED")
            """<inheritdoc cref="B"/>"""

    Assert.Contains("B summary", result)
    Assert.DoesNotContain("<inheritdoc", result)
    Assert.DoesNotContain("SHOULD_NOT_BE_USED", result)

[<Fact>]
let ``tooltip drops implicit inheritdoc on class with object base and no interface`` () =
    // Roslyn would inherit System.Object's docs here; F# intentionally treats a bare object base
    // as "nothing useful to inherit" (documented deviation) and drops the tag silently.
    let xml =
        getTooltipXml
            """
module Test
/// <inheritdoc/>
type Lon{caret}e() = class end
"""

    Assert.DoesNotContain("<inheritdoc", xmlText xml)
    Assert.DoesNotContain("Supports all classes", xmlText xml)

[<Fact>]
let ``symbol drops implicit inheritdoc on a struct (no ValueType inheritance)`` () =
    // A struct's only supertype is System.ValueType. Roslyn (and the inheritdoc spec) return no
    // candidate for structs/enums/delegates, so nothing is inherited. Guards against the Path A
    // resolver reaching System.ValueType's external documentation.
    let xml =
        getSymbolXml
            "S"
            """
module Test
/// <inheritdoc/>
[<Struct>]
type S =
    val X: int
let f (x: S) = x{caret}
"""

    Assert.DoesNotContain("<inheritdoc", xmlText xml)
    Assert.DoesNotContain("base class for value", xmlText xml)

[<Fact>]
let ``symbol drops implicit inheritdoc on a delegate`` () =
    let xml =
        getSymbolXml
            "D"
            """
module Test
/// <inheritdoc/>
type D = delegate of int -> int
let f (x: D) = x{caret}
"""

    Assert.DoesNotContain("<inheritdoc", xmlText xml)
    Assert.DoesNotContain("invocation list", xmlText xml)

[<Fact>]
let ``tooltip does not inherit for a non-override member sharing a base name`` () =
    // A new (non-override) member that merely shares a name with a base member has no
    // inheritance candidate in Roslyn (method -> interface impl only). F# must not fall back
    // to the base member's docs just because the names collide.
    let xml =
        getTooltipXml
            """
module Test
type Base() =
    /// <summary>base foo docs</summary>
    member _.Foo(x: int) = x
type Derived() =
    inherit Base()
    /// <inheritdoc/>
    member _.Foo(x: int) = x + 1
let d = Derived()
let _ = d.Fo{caret}o(0)
"""

    Assert.DoesNotContain("<inheritdoc", xmlText xml)
    Assert.DoesNotContain("base foo docs", xmlText xml)

[<Fact>]
let ``tooltip override inherits the matching base overload docs`` () =
    // With multiple base overloads, an override's <inheritdoc/> must inherit the docs of the
    // overload it actually overrides (by signature), not the first documented same-named overload.
    let xml =
        getTooltipXml
            """
module Test
type Base() =
    /// <summary>int overload docs</summary>
    abstract M: int -> unit
    /// <summary>string overload docs</summary>
    abstract M: string -> unit
    default _.M(_: int) = ()
    default _.M(_: string) = ()
type Derived() =
    inherit Base()
    /// <inheritdoc/>
    override _.M(x: string) = ()
let d = Derived()
let _ = d.M{caret}("")
"""

    Assert.Contains("string overload docs", xmlText xml)
    Assert.DoesNotContain("int overload docs", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip constructor inherits matching base constructor docs`` () =
    // Roslyn GetCandidateSymbol: a constructor inherits documentation from the base-type
    // constructor with a matching signature (constructors are not overrides).
    let xml =
        getTooltipXml
            """
module Test
type Base =
    val x: int
    /// <summary>base ctor docs</summary>
    new (x: int) = { x = x }
type Derived =
    inherit Base
    /// <inheritdoc/>
    new (x: int) = { inherit Base(x) }
let _ = Deri{caret}ved(0)
"""

    Assert.Contains("base ctor docs", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip constructor inherits the matching base constructor overload docs`` () =
    // With multiple base constructors, <inheritdoc/> must inherit the docs of the base
    // constructor whose signature matches, not the first documented one.
    let xml =
        getTooltipXml
            """
module Test
type Base =
    val x: int
    /// <summary>int ctor docs</summary>
    new (x: int) = { x = x }
    /// <summary>string ctor docs</summary>
    new (s: string) = { x = s.Length }
type Derived =
    inherit Base
    /// <inheritdoc/>
    new (s: string) = { inherit Base(s) }
let _ = Deri{caret}ved("")
"""

    Assert.Contains("string ctor docs", xmlText xml)
    Assert.DoesNotContain("int ctor docs", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip constructor inherits from a generic base constructor`` () =
    // The base type is generic (Base<'T>) instantiated as Base<int>. The base constructor's
    // parameter 'T must be seen as int so it matches the derived new(x: int) by signature.
    let xml =
        getTooltipXml
            """
module Test
type Base<'T> =
    val x: 'T
    /// <summary>generic base ctor docs</summary>
    new (x: 'T) = { x = x }
type Derived =
    inherit Base<int>
    /// <inheritdoc/>
    new (x: int) = { inherit Base<int>(x) }
let _ = Deri{caret}ved(0)
"""

    Assert.Contains("generic base ctor docs", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip constructor with no matching base overload drops the tag silently`` () =
    // The derived constructor's signature (string) matches no base constructor (only int exists),
    // so nothing is inherited: the tag is dropped silently, without fabricating the wrong docs.
    let xml =
        getTooltipXml
            """
module Test
type Base =
    val x: int
    /// <summary>base int ctor docs</summary>
    new (x: int) = { x = x }
type Derived =
    inherit Base
    /// <inheritdoc/>
    new (s: string) = { inherit Base(s.Length) }
let _ = Deri{caret}ved("")
"""

    Assert.DoesNotContain("base int ctor docs", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip struct constructor inheritdoc does not leak ValueType docs`` () =
    // A struct has no inheritance candidate (Roslyn returns null). A struct constructor with
    // <inheritdoc/> must silently drop the tag, never surfacing System.ValueType's ctor docs.
    let xml =
        getTooltipXml
            """
module Test
[<Struct>]
type S =
    val X: int
    /// <inheritdoc/>
    new (x: int) = { X = x }
let _ = S{caret}(0)
"""

    Assert.DoesNotContain("<inheritdoc", xmlText xml)
    Assert.DoesNotContain("ValueType", xmlText xml)

[<Fact>]
let ``tooltip picks the called constructor overload when the derived type has several`` () =
    // The derived type declares two <inheritdoc/> constructors. Each call site must expand against
    // the base constructor matching THAT overload, proving Path B receives the resolved ctor minfo
    // for the call, not merely the first constructor in the group.
    let source =
        """
module Test
type Base =
    val x: int
    /// <summary>base int ctor docs</summary>
    new (x: int) = { x = x }
    /// <summary>base string ctor docs</summary>
    new (s: string) = { x = s.Length }
type Derived =
    inherit Base
    /// <inheritdoc/>
    new (x: int) = { inherit Base(x) }
    /// <inheritdoc/>
    new (s: string) = { inherit Base(s) }
"""

    let intCall = getTooltipXml (source + "let _ = Deri{caret}ved(0)\n")
    Assert.Contains("base int ctor docs", xmlText intCall)
    Assert.DoesNotContain("base string ctor docs", xmlText intCall)
    Assert.DoesNotContain("<inheritdoc", xmlText intCall)

    let stringCall = getTooltipXml (source + "let _ = Deri{caret}ved(\"\")\n")
    Assert.Contains("base string ctor docs", xmlText stringCall)
    Assert.DoesNotContain("base int ctor docs", xmlText stringCall)
    Assert.DoesNotContain("<inheritdoc", xmlText stringCall)

[<Fact>]
let ``tooltip type inherits docs from a generic base class`` () =
    // "Inheriting generics": a generic derived type inheriting a generic base type's docs.
    let xml =
        getTooltipXml
            """
module Test
/// <summary>generic base type docs</summary>
type Base<'T>() =
    member _.M() = ()
/// <inheritdoc/>
type Deri{caret}ved<'T>() =
    inherit Base<'T>()
"""

    Assert.Contains("generic base type docs", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip type inherits docs from a generic interface`` () =
    // A type whose <inheritdoc/> resolves through a generic implemented interface.
    let xml =
        getTooltipXml
            """
module Test
/// <summary>generic iface docs</summary>
type IThing<'T> =
    abstract member Do: 'T -> unit
/// <inheritdoc/>
type Thin{caret}g() =
    interface IThing<int> with
        member _.Do(_) = ()
"""

    Assert.Contains("generic iface docs", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip method override inherits from a generic base method`` () =
    // Override of a method declared on a generic base (Get: unit -> 'T instantiated to int):
    // signature matching must still find the overridden slot.
    let xml =
        getTooltipXml
            """
module Test
type Base<'T>() =
    /// <summary>generic base method docs</summary>
    abstract member Get: unit -> 'T
    default _.Get() = Unchecked.defaultof<'T>
type Derived() =
    inherit Base<int>()
    /// <inheritdoc/>
    override _.Get() = 0
let d = Derived()
let _ = d.Ge{caret}t()
"""

    Assert.Contains("generic base method docs", xmlText xml)
    Assert.DoesNotContain("<inheritdoc", xmlText xml)

[<Fact>]
let ``tooltip inherited markup is spliced as XML, not escaped text`` () =
    // Regression: the expanded doc must round-trip as real XML. A previous defect stored the
    // engine output as a single line beginning with whitespace, so XmlDoc elaboration re-wrapped
    // it in an implicit <summary> and XML-escaped the inherited markup (&lt;summary&gt;...), which
    // an IDE would render as literal angle brackets instead of formatted documentation.
    let text =
        getTooltipXml
            """
module Test
type Base<'T>() =
    /// <summary>Clones a <typeparamref name="T"/> value</summary>
    abstract member Clone: unit -> 'T
    default _.Clone() = Unchecked.defaultof<'T>
type Derived() =
    inherit Base<int>()
    /// <inheritdoc/>
    override _.Clone() = 0
let d = Derived()
let _ = d.Clo{caret}ne()
"""
        |> xmlText

    Assert.Contains("<summary>", text)
    Assert.Contains("Clones a", text)
    Assert.Contains("<typeparamref", text)
    Assert.DoesNotContain("&lt;", text)
    Assert.DoesNotContain("&gt;", text)
    Assert.DoesNotContain("<inheritdoc", text)

[<Fact>]
let ``symbol inherited markup is spliced as XML, not escaped text`` () =
    // Same regression guard on the FSharpSymbol.XmlDoc (Path A) resolver.
    let text =
        getSymbolXml
            "Derived"
            """
module Test
/// <summary>Base docs with <c>inline code</c></summary>
type Base() = class end
/// <inheritdoc/>
type Derived() =
    inherit Base()
let _ = Derived(){caret}
"""
        |> xmlText

    Assert.Contains("<summary>", text)
    Assert.Contains("<c>inline code</c>", text)
    Assert.DoesNotContain("&lt;", text)
    Assert.DoesNotContain("&gt;", text)
    Assert.DoesNotContain("<inheritdoc", text)

[<Fact>]
let ``engine path filter selecting text nodes degrades gracefully`` () =
    // A user-authored path attribute whose XPath selects non-element (text) nodes must not throw
    // out of the tooltip/completion pipeline. XPathSelectElements raises InvalidOperationException
    // on text-node results, which is neither XPathException nor XmlException; the engine must
    // swallow it and degrade to dropping the directive rather than crashing.
    let result =
        expandWith
            [ "B", "<summary>Hello <b>world</b></summary>" ]
            None
            """<inheritdoc cref="B" path="/summary/node()"/>"""

    Assert.DoesNotContain("<inheritdoc", result)
