module FSharp.Compiler.Service.Tests.CompletionTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Test.Assert
open FSharp.Test.Compiler.Assertions.TextBasedDiagnosticAsserts
open Xunit

let private assertItemsWithNames contains names (completionInfo: DeclarationListInfo) =
    let itemNames =
        completionInfo.Items
        |> Array.map _.NameInCode
        |> Array.map normalizeNewLines
        |> set

    for name in names do
        let name = normalizeNewLines name
        Set.contains name itemNames |> shouldEqual contains

let assertHasItemWithNames names (completionInfo: DeclarationListInfo) =
    assertItemsWithNames true names completionInfo

let assertHasNoItemsWithNames names (completionInfo: DeclarationListInfo) =
    assertItemsWithNames false names completionInfo

[<Fact>]
let ``Expr - After record decl 01`` () =
    let info = Checker.getCompletionInfo """
type Record = { Field: int }

{ Fi{caret} }
"""
    assertHasItemWithNames ["ignore"] info

[<Fact>]
let ``Expr - After record decl 02`` () =
    let info = Checker.getCompletionInfo """
type Record = { Field: int }

{caret}
"""
    assertHasItemWithNames ["ignore"] info

[<Fact>]
let ``Expr - record - field 01 - anon module`` () =
    let info = Checker.getCompletionInfo """
type Record = { Field: int }

{ Fi{caret} }
"""
    assertHasItemWithNames ["Field"] info

[<Fact>]
let ``Expr - record - field 02 - anon module`` () =
    let info = Checker.getCompletionInfo """
type Record = { Field: int }

let record = { Field = 1 }

{ Fi{caret} }
"""
    assertHasItemWithNames ["Field"] info

[<Fact>]
let ``Expr - record - empty 01`` () =
    let info = Checker.getCompletionInfo """
type Record = { Field: int }

{ {caret} }
"""
    assertHasItemWithNames ["Field"] info

[<Fact>]
let ``Expr - record - empty 02`` () =
    let info = Checker.getCompletionInfo """
type Record = { Field: int }

let record = { Field = 1 }

{ {caret} }
"""
    assertHasItemWithNames ["Field"; "record"] info

[<Fact>]
let ``Underscore dot lambda - completion 01`` () =
    let info = Checker.getCompletionInfo """
"" |> _.Len{caret}"""

    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Underscore dot lambda - completion 02`` () =
    let info = Checker.getCompletionInfo """
System.DateTime.Now |> _.TimeOfDay.Mill{caret}"""

    assertHasItemWithNames ["Milliseconds"] info

[<Fact>]
let ``Underscore dot lambda - completion 03`` () =
    let info = Checker.getCompletionInfo """
"" |> _.ToString().Len{caret}"""

    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Underscore dot lambda - completion 04`` () =
    let info = Checker.getCompletionInfo """
"" |> _.Len{caret}gth.ToString()"""

    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Underscore dot lambda - completion 05`` () =
    let info = Checker.getCompletionInfo """
"" |> _.Length.ToString().Chars("".Len{caret})"""

    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Underscore dot lambda - completion 06`` () =
    let info = Checker.getCompletionInfo """
"" |> _.Chars(System.DateTime.UtcNow.Tic{caret}).ToString()"""

    assertHasItemWithNames ["Ticks"] info

[<Fact>]
let ``Underscore dot lambda - completion 07`` () =
    let info = Checker.getCompletionInfo """
"" |> _.Length.ToString().Len{caret}"""

    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Underscore dot lambda - completion 08`` () =
    let info = Checker.getCompletionInfo """
System.DateTime.Now |> _.TimeOfDay
                        .Mill{caret}"""

    assertHasItemWithNames ["Milliseconds"] info

[<Fact>]
let ``Underscore dot lambda - completion 09`` () =
    let info = Checker.getCompletionInfo """
"" |> _.Length.ToSt{caret}.Length"""

    assertHasItemWithNames ["ToString"] info

[<Fact>]
let ``Underscore dot lambda - completion 10`` () =
    let info = Checker.getCompletionInfo """
"" |> _.Chars(0).ToStr{caret}.Length"""

    assertHasItemWithNames ["ToString"] info

[<Fact>]
let ``Underscore dot lambda - completion 11`` () =
    let info = Checker.getCompletionInfo """
open System.Linq

[[""]] |> _.Select(_.Head.ToL{caret})"""

    assertHasItemWithNames ["ToLower"] info

[<Fact>]
let ``Underscore dot lambda - completion 12`` () =
    let info = Checker.getCompletionInfo """
open System.Linq

[[[""]]] |> _.Head.Select(_.Head.ToL{caret})"""

    assertHasItemWithNames ["ToLower"] info

[<Fact>]
let ``Underscore dot lambda - completion 13`` () =
    let info = Checker.getCompletionInfo """
let myFancyFunc (x:string) =
    x
    |> _.ToL{caret}"""
    assertHasItemWithNames ["ToLower"] info

[<Fact>]
let ``Underscore dot lambda - completion 14`` () =
    let info = Checker.getCompletionInfo """
let myFancyFunc (x:System.DateTime) =
    x
    |> _.TimeOfDay.Mill{caret}
    |> id"""
    assertHasItemWithNames ["Milliseconds"] info

[<Fact>]
let ``Underscore dot lambda - completion 15`` () =
    let info = Checker.getCompletionInfo """
let _a = 5
"" |> _{caret}.Length.ToString() """
    assertHasItemWithNames ["_a"] info

[<Fact>]
let ``Underscore dot lambda - No prefix 01`` () =
    let info = Checker.getCompletionInfo """
let s = ""
[s] |> List.map _.{caret}
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Underscore dot lambda - No prefix 02`` () =
    let info = Checker.getCompletionInfo """
System.DateTime.Now |> _.TimeOfDay.{caret}"""

    assertHasItemWithNames ["Milliseconds"] info

[<Fact>]
let ``Underscore dot lambda - No prefix 03`` () =
    let info = Checker.getCompletionInfo """
"" |> _.Length.ToString().{caret}"""

    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Type decl - Record - Field type 01`` () =
    let info = Checker.getCompletionInfo """
type Record = { Field: {caret} }
"""
    assertHasItemWithNames ["string"] info


[<Fact>]
let ``Expr - Qualifier 01`` () =
    let info = Checker.getCompletionInfo """
let f (s: string) =
    s.Trim().{caret}
    s.Trim()
    s.Trim()
    ()
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 02`` () =
    let info = Checker.getCompletionInfo """
let f (s: string) =
    s.Trim()
    s.Trim().{caret}
    s.Trim()
    ()
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 03`` () =
    let info = Checker.getCompletionInfo """
let f (s: string) =
    s.Trim()
    s.Trim()
    s.Trim().{caret}
    ()
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 04`` () =
    let info = Checker.getCompletionInfo """
type T() =
    do
        System.String.Empty.ToString().L{caret}
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 05`` () =
    let info = Checker.getCompletionInfo """
System.String.Empty.ToString().{caret}
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 06`` () =
    let info = Checker.getCompletionInfo """
System.String.Empty.ToString().L{caret}
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Expr - Qualifier 07`` () =
    let info = Checker.getCompletionInfo """
type T() =
    do
        System.String.Empty.ToString().L{caret}
        ()
"""
    assertHasItemWithNames ["Length"] info

[<Fact>]
let ``Import - Ns 01`` () =
    let info = Checker.getCompletionInfo """
namespace Ns

type Rec1 = { F: int }


namespace Ns

type Rec2 = { F: int }

module M =

    type Rec3 = { F: int }

    let _: R{caret} = ()
"""
    assertHasItemWithNames ["Rec1"; "Rec2"; "Rec3"] info

[<Fact>]
let ``Import - Ns 02 - Rec`` () =
    let info = Checker.getCompletionInfo """
namespace Ns

type Rec1 = { F: int }


namespace rec Ns

type Rec2 = { F: int }

module M =

    type Rec3 = { F: int }

    let _: R{caret} = ()
"""
    assertHasItemWithNames ["Rec1"; "Rec2"; "Rec3"] info

[<Fact>]
let ``Import - Ns 03 - Rec`` () =
    let info = Checker.getCompletionInfo """
namespace Ns

type Rec1 = { F: int }


namespace rec Ns

type Rec2 = { F: int }

module rec M =

    type Rec3 = { F: int }

    let _: R{caret} = ()
"""
    assertHasItemWithNames ["Rec1"; "Rec2"; "Rec3"] info

[<Fact>]
let ``Not in scope 01`` () =
    let info = Checker.getCompletionInfo """
namespace Ns1

type E =
    | A = 1
    | B = 2
    | C = 3

namespace Ns2

module Module =
    match Ns1.E.A with
    | {caret}

"""
    assertHasNoItemsWithNames ["E"] info

#if NETCOREAPP
[<Fact>]
let ``Span appears in completion and is not marked obsolete`` () =
    let info = Checker.getCompletionInfo """
let test = System.Sp{caret}
"""
    assertHasItemWithNames ["Span"] info
#endif

module Options =
    let private assertItemWithOptions getOption (options: FSharpCodeCompletionOptions list) name source =
        options
        |> List.iter (fun options ->
            let contains = getOption options
            let info = Checker.getCompletionInfoWithOptions options source
            assertItemsWithNames contains [name] info
        )

    module AllowObsolete =
        let private allowObsoleteOptions = { FSharpCodeCompletionOptions.Default with SuggestObsoleteSymbols = true }
        let private disallowObsoleteOptions = { FSharpCodeCompletionOptions.Default with SuggestObsoleteSymbols = false }

        let private assertItemWithOptions =
            assertItemWithOptions _.SuggestObsoleteSymbols

        let assertItem (name: string) source =
            assertItemWithOptions [allowObsoleteOptions; disallowObsoleteOptions] name source

        let assertItemAllowed name source =
            assertItemWithOptions [allowObsoleteOptions] name source

        let assertItemNotAllowed name source =
            assertItemWithOptions [disallowObsoleteOptions] name source

        [<Fact>]
        let ``Prop - Instance 01`` () =
            assertItem "Prop" """
type T() =
    [<System.Obsolete>]
    member this.Prop = 1

T().{caret}
"""

        [<Fact>]
        let ``Prop - Instance 02`` () =
            assertItem "Prop" """
type T() =
    [<System.Obsolete>]
    member this.Prop = 1

let t = T()
t.{caret}
"""

        [<Fact>]
        let ``Prop - Instance 03`` () =
            assertItem "Prop" """
type T() =
    [<System.Obsolete>]
    member val Prop = 1

T().{caret}
"""

        [<Fact>]
        let ``Prop - Static 01`` () =
            assertItemAllowed "Prop" """
type T() =
    [<System.Obsolete>]
    static member Prop = 1

T.{caret}
"""

        [<Fact>]
        let ``Prop - Static 02`` () =
            assertItemAllowed "Prop" """
type T() =
    [<System.Obsolete>]
    static member val Prop = 1

T.{caret}
"""

        [<Fact>]
        let ``Prop - Extension 01`` () =
            assertItemAllowed "Prop" """
type System.String with
    [<System.Obsolete>]
    member _.Prop = 1

"".{caret}
"""

        [<Fact>]
        let ``Prop - Extension 02`` () =
            assertItemAllowed "Prop" """
type System.String with
    [<System.Obsolete>]
    static member Prop = 1

System.String.{caret}
"""

        [<Fact>]
        let ``Method - Instance 01`` () =
            assertItem "Method" """
type T() =
    [<System.Obsolete>]
    member _.Method() = 1

T().{caret}
"""

        [<Fact>]
        let ``Method - Instance 02`` () =
            assertItem "Method" """
type T() =
    [<System.Obsolete>]
    member _.Method() = 1

let t = T()
t.{caret}
"""
        
        [<Fact>]
        let ``Method - Static 01`` () =
            assertItemAllowed "Method" """
type T() =
    [<System.Obsolete>]
    static member Method() = 1

T.{caret}
"""

        [<Fact>]
        let ``Union 01`` () =
            assertItemAllowed "A" """
[<System.Obsolete>]
type T =
    | A

T.{caret}
"""

        [<Fact>]
        let ``Module - Value 01`` () =
            assertItemAllowed "x" """
[<System.Obsolete>]
let x = 1

{caret}
"""
        [<Fact>]
        let ``Module - Value 02`` () =
            assertItemAllowed "x" """
[<System.Obsolete>]
let x = 1

do
    {caret}
"""
        [<Fact>]
        let ``Module - Value 03`` () =
            assertItemAllowed "x" """
module Module1 =
    [<System.Obsolete>]
    let x = 1

module Module2 =
    do Module1.{caret}
"""

        [<Fact>]
        let ``Module - Value 04`` () =
            assertItemAllowed "x" """
module Module1 =
    [<System.Obsolete>]
    let x = 1

module Module2 =
    open Module1
    do {caret}
"""
     
        [<Fact>]
        let ``Module - Value 05`` () =
            assertItemAllowed "x" """
[<System.Obsolete>]
let x = 1

x{caret}
"""
        [<Fact>]
        let ``Module - Value 06`` () =
            assertItemAllowed "x" """
[<System.Obsolete>]
let x = 1

do
    x{caret}
"""
        [<Fact>]
        let ``Module - Value 07`` () =
            assertItemAllowed "x" """
module Module1 =
    [<System.Obsolete>]
    let x = 1

module Module2 =
    do Module1.x{caret}
"""

        [<Fact>]
        let ``Module - Value 08`` () =
            assertItemAllowed "x" """
module Module1 =
    [<System.Obsolete>]
    let x = 1

module Module2 =
    open Module1
    do x{caret}
"""

        [<Fact>]
        let ``Type 01`` () =
            assertItemAllowed "T" """
[<System.Obsolete>]
type T() =
    class end

let _: {caret}
"""

        [<Fact>]
        let ``Type 02`` () =
            assertItemAllowed "T" """
[<System.Obsolete>]
type T() =
    class end

{caret}
"""

        [<Fact>]
        let ``Type 03`` () =
            assertItemAllowed "T" """
[<System.Obsolete>]
type T() =
    class end

do {caret}
"""

        [<Fact>]
        let ``Record - Field 01`` () =
            assertItemAllowed "F" """
type R =
    { [<System.Obsolete>]
      F: int }

let r = { {caret} }
"""

        [<Fact>]
        let ``Record - Field 02`` () =
            assertItemAllowed "F" """
[<System.Obsolete>]
type R =
    { F: int }

let r = { {caret} }
"""

        [<Fact>]
        let ``Record - Field 03`` () =
            assertItemAllowed "F" """
[<System.Obsolete>]
type R =
    { F: int }

let r: R = { F = 1 }
r.{caret}
"""

        [<Fact>]
        let ``Exception 01`` () =
            assertItemAllowed "E" """
[<System.Obsolete>]
exception E

{caret}
"""
        [<Fact>]
        let ``Exception 02`` () =
            assertItemAllowed "E" """
[<System.Obsolete>]
exception E

E{caret}
"""

        [<Fact>]
        let ``Exception 03`` () =
            assertItemAllowed "E" """
[<System.Obsolete>]
exception E

try () with {caret}
"""

        [<Fact>]
        let ``Exception 04`` () =
            assertItemAllowed "E" """
[<System.Obsolete>]
exception E

try () with E{caret}
"""


    module PatternNameSuggestions =
        let private suggestPatternNames = { FSharpCodeCompletionOptions.Default with SuggestPatternNames = true }
        let private doNotSuggestPatternNames = { FSharpCodeCompletionOptions.Default with SuggestPatternNames = false }

        let assertItemWithOptions =
            assertItemWithOptions _.SuggestPatternNames

        let assertItem name source =
            assertItemWithOptions [suggestPatternNames; doNotSuggestPatternNames] name source

        [<Fact>]
        let ``Union case field 01`` () =
            assertItem "named" """
type U =
    | A of named: int

match A 1 with
| A n{caret}
"""

    module OverrideSuggestions =
        let private suggestOverrides = { FSharpCodeCompletionOptions.Default with SuggestGeneratedOverrides = true }
        let private doNotSuggestOverrides = { FSharpCodeCompletionOptions.Default with SuggestGeneratedOverrides = false }

        let assertItemWithOptions =
            assertItemWithOptions _.SuggestGeneratedOverrides

        let assertItem name source =
            assertItemWithOptions [suggestOverrides; doNotSuggestOverrides] name source

        [<Fact>]
        let ``Override 01`` () =
            assertItem "this.ToString (): string = \n        base.ToString()" """
type T() =
    override {caret}
"""