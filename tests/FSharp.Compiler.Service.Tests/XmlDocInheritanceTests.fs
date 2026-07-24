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
