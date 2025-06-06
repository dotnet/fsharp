module FSharp.Compiler.Service.Tests.CompletionTests

open FSharp.Compiler.EditorServices
open Xunit

let private assertItemsWithNames contains names (completionInfo: DeclarationListInfo) =
    let itemNames = completionInfo.Items |> Array.map _.NameInCode |> set

    for name in names do
        Assert.True(Set.contains name itemNames = contains)

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
