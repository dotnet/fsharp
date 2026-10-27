module FSharp.Compiler.Service.Tests.CompletionTests

open System.ComponentModel
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Test.Assert
open FSharp.Test.Compiler.Assertions.TextBasedDiagnosticAsserts
open Xunit

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

[<Fact>]
let ``Pattern - Enum 01`` () =
    let info =
        Checker.getCompletionInfo """
namespace Ns1
type E =
    | A = 1
    | B = 2

namespace Ns2

open Ns1

module M =
    match E.A with
    | E.{caret}
"""
    assertHasItemWithNames ["A"] info

[<Fact>]
let ``Pattern - Enum 02`` () =
    let info =
        Checker.getCompletionInfo """
namespace Ns1
type E =
    | A = 1
    | B = 2

namespace Ns2

open Ns1

module M =
    match E.A with
    | E.{caret}
    | E.B -> ()
"""
    assertHasItemWithNames ["A"] info

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

    let private assertCSharpInteropItemWithOptions getOption (options: FSharpCodeCompletionOptions list) name source =
        let csharpAssembly = PathRelativeToTestAssembly "CSharp_Analysis.dll"
        let compilerOptions = [| $"-r:{csharpAssembly}" |]
        options
        |> List.iter (fun options ->
            let contains = getOption options
            let info = Checker.getCompletionInfoWithCompilerAndCompletionOptions compilerOptions options source
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

        // https://github.com/dotnet/fsharp/issues/13512
        [<Fact>]
        let ``Event - Instance 01`` () =
            assertItem "Ev" """
type T() =
    [<System.Obsolete; CLIEvent>]
    member _.Ev = Event<System.EventHandler, _>().Publish

T().{caret}
"""

        // https://github.com/dotnet/fsharp/issues/13512
        [<Fact>]
        let ``Event - Static 01`` () =
            assertItem "Ev" """
type T() =
    [<System.Obsolete; CLIEvent>]
    static member Ev = Event<System.EventHandler, _>().Publish

T.{caret}
"""

        let private assertCSharpInteropItem name source =
            assertCSharpInteropItemWithOptions _.SuggestObsoleteSymbols [allowObsoleteOptions; disallowObsoleteOptions] name source

        // https://github.com/dotnet/fsharp/issues/13512
        [<Fact>]
        let ``CSharp - Obsolete field is hidden`` () =
            assertCSharpInteropItem "ObsoleteField" """
open FSharp.Compiler.Service.Tests
ObsoleteMembersClass.{caret}
"""

        // https://github.com/dotnet/fsharp/issues/13512
        [<Fact>]
        let ``CSharp - Obsolete method is hidden`` () =
            assertCSharpInteropItem "ObsoleteMethod" """
open FSharp.Compiler.Service.Tests
ObsoleteMembersClass.{caret}
"""

        // https://github.com/dotnet/fsharp/issues/13512
        [<Fact>]
        let ``CSharp - Obsolete property is hidden`` () =
            assertCSharpInteropItem "ObsoleteProperty" """
open FSharp.Compiler.Service.Tests
ObsoleteMembersClass.{caret}
"""

        // https://github.com/dotnet/fsharp/issues/13512
        [<Fact>]
        let ``CSharp - Obsolete event is hidden`` () =
            assertCSharpInteropItem "ObsoleteEvent" """
open FSharp.Compiler.Service.Tests
ObsoleteMembersClass.{caret}
"""

        [<Fact>]
        let ``Nested module 01`` () =
            assertItem "x" """
module M =
    module N =
        [<System.Obsolete>]
        let x = 1

M.N.{caret}
"""

        [<Fact>]
        let ``Nested module 02`` () =
            assertItem "x" """
module M =
    [<System.Obsolete>]
    module N =
        [<System.Obsolete>]
        let x = 1

M.N.{caret}
"""

        [<Fact>]
        let ``Nested type 01`` () =
            Checker.getCompletionInfo """
module M =
    type T() =
        [<System.Obsolete>]
        static member Prop = 1

M.T.{caret}
"""
            |> assertHasItemWithNames ["Prop"]

        [<Fact>]
        let ``Nested module - Record field 01`` () =
            assertItem "R" """
module M =
    module N =
        [<System.Obsolete>]
        type R = { F: int }

let r = { M.N.{caret} }
"""

        [<Fact>]
        let ``Nested module - Record field 02`` () =
            Checker.getCompletionInfo """
module M =
    [<System.Obsolete>]
    module N =
        type R = { F: int }

let r = { M.N.{caret} }
"""
            |> assertHasItemWithNames ["R"]

        // https://github.com/dotnet/fsharp/issues/13512
        [<Fact>]
        let ``CSharp - Non-obsolete members are always shown`` () =
            for name in ["NonObsoleteField"; "NonObsoleteMethod"; "NonObsoleteProperty"; "NonObsoleteEvent"] do
                assertCSharpInteropItemWithOptions (fun _ -> true) [allowObsoleteOptions; disallowObsoleteOptions] name """
open FSharp.Compiler.Service.Tests
ObsoleteMembersClass.{caret}
"""


    module EditorBrowsableNever =
        let private allowOptions = { FSharpCodeCompletionOptions.Default with SuggestEditorBrowsableSymbols = EditorBrowsableState.Never }
        let private disallowOptions = { FSharpCodeCompletionOptions.Default with SuggestEditorBrowsableSymbols = EditorBrowsableState.Advanced }

        let private suggestsNever (options: FSharpCodeCompletionOptions) =
            options.SuggestEditorBrowsableSymbols = EditorBrowsableState.Never

        let private assertItemAlways (name: string) source =
            assertItemWithOptions (fun _ -> true) [allowOptions; disallowOptions] name source

        let private assertItemWithOptions =
            assertItemWithOptions suggestsNever

        let assertItem (name: string) source =
            assertItemWithOptions [allowOptions; disallowOptions] name source

        let private assertCSharpInteropItem contains name source =
            assertCSharpInteropItemWithOptions contains [allowOptions; disallowOptions] name source

        [<Fact>]
        let ``Prop - Instance 01`` () =
            assertItem "Prop" """
open System.ComponentModel

type T() =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    member this.Prop = 1

T().{caret}
"""

        [<Fact>]
        let ``Prop - Static 01`` () =
            assertItem "Prop" """
open System.ComponentModel

type T() =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member Prop = 1

T.{caret}
"""

        [<Fact>]
        let ``Prop - Always 01`` () =
            assertItemAlways "Prop" """
open System.ComponentModel

type T() =
    [<EditorBrowsable>]
    static member Prop = 1

T.{caret}
"""

        [<Fact>]
        let ``Method - Static 01`` () =
            assertItem "Method" """
open System.ComponentModel

type T() =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member Method() = 1

T.{caret}
"""

        [<Fact>]
        let ``Event - Instance 01`` () =
            assertItem "Ev" """
open System.ComponentModel

type T() =
    [<EditorBrowsable(EditorBrowsableState.Never); CLIEvent>]
    member _.Ev = Event<System.EventHandler, _>().Publish

T().{caret}
"""

        [<Fact>]
        let ``Union 01`` () =
            assertItem "A" """
open System.ComponentModel

type U =
    | [<EditorBrowsable(EditorBrowsableState.Never)>] A
    | B

U.{caret}
"""

        [<Fact>]
        let ``Union 02`` () =
            assertItem "A" """
open System.ComponentModel

type U =
    | [<EditorBrowsable(EditorBrowsableState.Never)>] A
    | B

let y = 1

do
    {caret}
"""

        [<Fact>]
        let ``Type 01`` () =
            assertItem "T" """
open System.ComponentModel

[<EditorBrowsable(EditorBrowsableState.Never)>]
type T() = class end

{caret}
"""

        [<Fact>]
        let ``Type 02`` () =
            assertItem "T" """
open System.ComponentModel

module M =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    type T() = class end

M.{caret}
"""

        [<Fact>]
        let ``Prop - Nested type 01`` () =
            assertItem "Prop" """
open System.ComponentModel

module M =
    type T() =
        [<EditorBrowsable(EditorBrowsableState.Never)>]
        static member Prop = 1

M.T.{caret}
"""

        [<Fact>]
        let ``Type - Members of hidden type are shown`` () =
            assertItemAlways "Prop" """
open System.ComponentModel

module M =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    type T() =
        static member Prop = 1

M.T.{caret}
"""

        [<Fact>]
        let ``Module 01`` () =
            assertItem "M" """
open System.ComponentModel

[<EditorBrowsable(EditorBrowsableState.Never)>]
module M =
    let x = 1

let y = 1

do
    {caret}
"""

        [<Fact>]
        let ``Module 02`` () =
            assertItem "N" """
open System.ComponentModel

module M =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    module N =
        let x = 1

M.{caret}
"""

        [<Fact>]
        let ``Module - Contents of hidden module are shown`` () =
            assertItemAlways "x" """
open System.ComponentModel

[<EditorBrowsable(EditorBrowsableState.Never)>]
module M =
    let x = 1

M.{caret}
"""

        [<Fact>]
        let ``Value 01`` () =
            assertItem "x" """
open System.ComponentModel

[<EditorBrowsable(EditorBrowsableState.Never)>]
let x = 1

{caret}
"""

        [<Fact>]
        let ``Value 02`` () =
            assertItem "x" """
open System.ComponentModel

module M =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    let x = 1

M.{caret}
"""

        [<Fact>]
        let ``Exception 01`` () =
            assertItem "E" """
open System.ComponentModel

[<EditorBrowsable(EditorBrowsableState.Never)>]
exception E

{caret}
"""

        [<Fact>]
        let ``Active pattern 01`` () =
            assertItem "Even" """
open System.ComponentModel

module M =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    let (|Even|Odd|) x = if x % 2 = 0 then Even else Odd

M.{caret}
"""

        [<Fact>]
        let ``CSharp - Type is hidden`` () =
            assertCSharpInteropItem suggestsNever "EditorBrowsableNeverClass" """
open FSharp.Compiler.Service.Tests
EditorBrowsable{caret}
"""

        [<Fact>]
        let ``CSharp - Members of hidden type are shown`` () =
            assertCSharpInteropItem (fun _ -> true) "Prop" """
open FSharp.Compiler.Service.Tests
EditorBrowsableNeverClass.{caret}
"""

        [<Fact>]
        let ``CSharp - Field is hidden`` () =
            assertCSharpInteropItem suggestsNever "NeverField" """
open FSharp.Compiler.Service.Tests
EditorBrowsableMembersClass.{caret}
"""

        [<Fact>]
        let ``CSharp - Method is hidden`` () =
            assertCSharpInteropItem suggestsNever "NeverMethod" """
open FSharp.Compiler.Service.Tests
EditorBrowsableMembersClass.{caret}
"""

        [<Fact>]
        let ``CSharp - Property is hidden`` () =
            assertCSharpInteropItem suggestsNever "NeverProperty" """
open FSharp.Compiler.Service.Tests
EditorBrowsableMembersClass.{caret}
"""

        [<Fact>]
        let ``CSharp - Event is hidden`` () =
            assertCSharpInteropItem suggestsNever "NeverEvent" """
open FSharp.Compiler.Service.Tests
EditorBrowsableMembersClass.{caret}
"""

        [<Fact>]
        let ``CSharp - Other states are always shown`` () =
            for name in ["AdvancedProperty"; "AlwaysProperty"; "VisibleProperty"] do
                assertCSharpInteropItem (fun _ -> true) name """
open FSharp.Compiler.Service.Tests
EditorBrowsableMembersClass.{caret}
"""

    module EditorBrowsableAdvanced =
        let private allowOptions = { FSharpCodeCompletionOptions.Default with SuggestEditorBrowsableSymbols = EditorBrowsableState.Advanced }
        let private disallowOptions = { FSharpCodeCompletionOptions.Default with SuggestEditorBrowsableSymbols = EditorBrowsableState.Always }

        let private suggestsAdvanced (options: FSharpCodeCompletionOptions) =
            options.SuggestEditorBrowsableSymbols <> EditorBrowsableState.Always

        let private assertItemNever (name: string) source =
            assertItemWithOptions (fun _ -> false) [allowOptions; disallowOptions] name source

        let private assertItemWithOptions =
            assertItemWithOptions suggestsAdvanced

        let assertItem (name: string) source =
            assertItemWithOptions [allowOptions; disallowOptions] name source

        let private assertCSharpInteropItem contains name source =
            assertCSharpInteropItemWithOptions contains [allowOptions; disallowOptions] name source

        [<Fact>]
        let ``Default shows advanced items`` () =
            FSharpCodeCompletionOptions.Default.SuggestEditorBrowsableSymbols |> shouldEqual EditorBrowsableState.Advanced

        [<Fact>]
        let ``Prop - Static 01`` () =
            assertItem "Prop" """
open System.ComponentModel

type T() =
    [<EditorBrowsable(EditorBrowsableState.Advanced)>]
    static member Prop = 1

T.{caret}
"""

        [<Fact>]
        let ``Never items are not affected`` () =
            assertItemNever "Prop" """
open System.ComponentModel

type T() =
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member Prop = 1

T.{caret}
"""

        [<Fact>]
        let ``CSharp - Advanced property`` () =
            assertCSharpInteropItem suggestsAdvanced "AdvancedProperty" """
open FSharp.Compiler.Service.Tests
EditorBrowsableMembersClass.{caret}
"""

        [<Fact>]
        let ``CSharp - Advanced type`` () =
            assertCSharpInteropItem suggestsAdvanced "EditorBrowsableAdvancedClass" """
open FSharp.Compiler.Service.Tests
EditorBrowsable{caret}
"""

            assertCSharpInteropItem (fun _ -> false) "NeverProperty" """
open FSharp.Compiler.Service.Tests
EditorBrowsableMembersClass.{caret}
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

// https://github.com/dotnet/fsharp/issues/13194
[<Fact>]
let ``Completion works for member whose name contains a single quote`` () =
    let info =
        Checker.getCompletionInfo
            """
/// Doc for normalize prime
let normalize' x = x + 1

normaliz{caret}
"""

    assertHasItemWithNames [ "normalize'" ] info

// Tests for https://github.com/dotnet/fsharp/issues/19906
// Named-argument completion must continue suggesting later named args after the first.

[<Fact>]
let ``Issue 19906 - non-overloaded method - second named arg suggested`` () =
    let info = Checker.getCompletionInfo """
type T() = member _.M(apple:int, banana:int, cherry:int) = ()
let t = T()
let _ = t.M(apple=1, b{caret})
"""
    assertHasItemWithNames ["banana"; "cherry"] info
    assertHasNoItemsWithNames ["apple"] info

[<Fact>]
let ``Issue 19906 - overloaded method - Task.Factory.StartNew second named arg suggested`` () =
    let info = Checker.getCompletionInfo """
open System.Threading.Tasks
let _ = Task.Factory.StartNew(action=(fun _ -> ()), s{caret})
"""
    assertHasItemWithNames ["state"; "cancellationToken"; "scheduler"; "creationOptions"] info
    assertHasNoItemsWithNames ["action"] info

[<Fact>]
let ``Issue 19906 - overloaded method - third named arg suggested`` () =
    let info = Checker.getCompletionInfo """
open System.Threading.Tasks
let _ = Task.Factory.StartNew(action=(fun _ -> ()), state=null, c{caret})
"""
    assertHasItemWithNames ["cancellationToken"; "creationOptions"] info

[<Fact>]
let ``Issue 19906 - optional args - second optional arg suggested`` () =
    let info = Checker.getCompletionInfo """
type T() = static member F(?x:int, ?y:int, ?z:int) = ()
let _ = T.F(?x=1, ?y{caret})
"""
    assertHasItemWithNames ["y"; "z"] info

[<Fact>]
let ``Issue 19906 - regression guard - zero-arg ctor with settable props still works`` () =
    let info = Checker.getCompletionInfo """
type R() =
    member val Apple = "" with get, set
    member val Banana = "" with get, set
    member val Cherry = "" with get, set
let _ = R(Apple="x", B{caret})
"""
    assertHasItemWithNames ["Banana"; "Cherry"] info

[<Fact>]
let ``Issue 19906 - regression guard - positional then partial still works`` () =
    let info = Checker.getCompletionInfo """
open System.Threading.Tasks
let _ = Task.Factory.StartNew((fun _ -> ()), s{caret})
"""
    assertHasItemWithNames ["state"; "cancellationToken"; "scheduler"; "creationOptions"] info

[<Fact>]
let ``Issue 19906 - regression guard - dotted completion inside named-arg RHS`` () =
    let info = Checker.getCompletionInfo """
let s = "x"
let _ = System.Uri(uriString = s.{caret}, kind = System.UriKind.Absolute)
"""
    assertHasItemWithNames ["Length"; "Substring"] info
    assertHasNoItemsWithNames ["uriString"; "kind"] info

module RecordSpreads =
    [<Literal>]
    let private SupportedLangVersion = "preview"

    let private getCompletionInfo markedSource =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| $"--langversion:{SupportedLangVersion}" |]
            FSharpCodeCompletionOptions.Default
            markedSource

    let private getCompletionInfoFor partialIdent markedSource =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| $"--langversion:{SupportedLangVersion}" |]
            FSharpCodeCompletionOptions.Default
            markedSource

    [<Fact>]
    let ``spread - completion fires inside nominal record type spread, no ident yet`` () =
        let info = getCompletionInfo """
type R1 = { A: int; B: int }
type R2 = class end
type R3 = { ...{caret} }
"""
        let names = info.Items |> Array.map _.NameInCode
        if not (Array.contains "R1" names) then
            failwith $"Expected completion at '{{ ...|caret| }}' to offer in-scope record type 'R1', but got %A{names}."

        if Array.contains "R2" names then
            failwith $"Expected completion at '{{ ...|caret| }}' not to offer in-scope non-record type 'R2', but got %A{names}."

    [<Fact>]
    let ``spread - completion fires inside nominal record type spread, partial ident`` () =
        let info = getCompletionInfo """
type R1 = { A: int; B: int }
type R2 = class end
type R3 = { ...R{caret} }
"""
        let names = info.Items |> Array.map _.NameInCode
        if not (Array.contains "R1" names) then
            failwith $"Expected completion at '{{ ...R|caret| }}' to offer in-scope record type 'R1', but got %A{names}."

        if Array.contains "R2" names then
            failwith $"Expected completion at '{{ ...R|caret| }}' not to offer in-scope non-record type 'R2', but got %A{names}."

    [<Fact>]
    let ``spread - completion fires inside nominal record expression spread, no ident yet`` () =
        let info = getCompletionInfo """
type R = { A: int; B: int }
let r1 = { A = 1; B = 2 }
let r2 = obj ()
let r3 = { ...{caret} }
"""
        let names = info.Items |> Array.map _.NameInCode
        if not (Array.contains "r1" names) then
            failwith $"Expected completion at '{{ ...r|caret| }}' to offer in-scope record value 'r1', but got %A{names}."

        if Array.contains "r2" names then
            failwith $"Expected completion at '{{ ...r|caret| }}' not to offer in-scope non-record value 'r2', but got %A{names}."

    [<Fact>]
    let ``spread - completion fires inside nominal record expression spread, partial ident`` () =
        let info = getCompletionInfo """
type R = { A: int; B: int }
let r1 = { A = 1; B = 2 }
let r2 = obj ()
let r3 = { ...r{caret} }
"""
        let names = info.Items |> Array.map _.NameInCode
        if not (Array.contains "r1" names) then
            failwith $"Expected completion at '{{ ...r|caret| }}' to offer in-scope record value 'r1', but got %A{names}."

        if Array.contains "r2" names then
            failwith $"Expected completion at '{{ ...r|caret| }}' not to offer in-scope non-record value 'r2', but got %A{names}."

    [<Fact>]
    let ``spread - completion fires inside anonymous record expression spread, no ident yet`` () =
        let info = getCompletionInfo """
let r1 = {| A = 1; B = 2 |}
let r2 = obj ()
let r3 = {| ...{caret} ; X = 1 |}
"""
        let names = info.Items |> Array.map _.NameInCode
        if not (Array.contains "r1" names) then
            failwith $"Expected completion at '{{| ...|caret| ; X = 1 |}}' to offer in-scope record value 'r1', but got %A{names}."

        if Array.contains "r2" names then
            failwith $"Expected completion at '{{ ...|caret| }}' not to offer in-scope non-record value 'r2', but got %A{names}."

    [<Fact>]
    let ``spread - completion fires inside anonymous record expression spread, partial ident`` () =
        let info = getCompletionInfo """
let r1 = {| A = 1; B = 2 |}
let r2 = obj ()
let r3 = {| ...r{caret} ; X = 1 |}
"""
        let names = info.Items |> Array.map _.NameInCode
        if not (Array.contains "r1" names) then
            failwith $"Expected completion at '{{| ...r|caret| ; X = 1 |}}' to offer in-scope record value 'r1', but got %A{names}."

        if Array.contains "r2" names then
            failwith $"Expected completion at '{{ ...r|caret| }}' not to offer in-scope non-record value 'r2', but got %A{names}."
