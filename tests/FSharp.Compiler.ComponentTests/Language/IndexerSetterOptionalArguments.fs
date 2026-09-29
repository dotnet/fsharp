// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module Language.IndexerSetterOptionalArguments

open System.IO
open Xunit
open FSharp.Test.Compiler

let private run source =
    FSharp source |> asExe |> compileExeAndRun |> shouldSucceed

let private optionalParameter native name ty =
    if native then
        $"?{name}: {ty}"
    else
        $"[<OptionalArgument>] {name}: {ty} option"

let private consumerPath =
    Path.Combine(Path.GetTempPath(), "OptionalIndexerConsumer.fs")

let private getterPath =
    Path.Combine(Path.GetTempPath(), "OptionalIndexerGetter.fs")

[<Theory>]
[<InlineData(true, true)>]
[<InlineData(false, true)>]
[<InlineData(false, false)>]
let ``Issue 20046 - caller information and getter preservation`` native omitted =
    let path = optionalParameter native "path" "string"
    let line = optionalParameter native "line" "int"
    let caller = optionalParameter native "caller" "string"

    let omissions =
        if omitted then
            $"""
# 301 "{consumerPath}"
    a["key"] <- "payload"
    check ("key", Some @"{consumerPath}", Some 301, "payload")
# 311 "{consumerPath}"
    a.Item("path override", path = "explicit.fs") <- "second"
    check ("path override", Some "explicit.fs", Some 311, "second")
# 321 "{consumerPath}"
    a.Item("line override", line = 17) <- "third"
    check ("line override", Some @"{consumerPath}", Some 17, "third")
    m.Item() <- "member payload"
    if m.Seen <> (Some "consume", "member payload") then failwithf "CallerMemberName: %%A" m.Seen
"""
        else
            ""

    run
        $"""
module Program
open System.Runtime.CompilerServices
# 51 "OptionalIndexerDeclaration.fs"
type A() =
    member val Seen = ("", None, None, "") with get, set
    member val Got = ("", None, None) with get, set
    member a.Item
        with get (key: string, [<CallerFilePath>] {path}, [<CallerLineNumber>] {line}) =
            a.Got <- key, path, line
            "getter"
        and set (key: string, [<CallerFilePath>] {path}, [<CallerLineNumber>] {line}) (value: string) =
            a.Seen <- key, path, line, value
type M() =
    member val Seen = (None, "") with get, set
    member m.Item with set ([<CallerMemberName>] {caller}) (v: string) = m.Seen <- caller, v

let consume () =
    let a, m = A(), M()
    let check expected = if a.Seen <> expected then failwithf "Setter: %%A <> %%A" a.Seen expected
# 201 "{getterPath}"
    let result = a["getter key"]
    if result <> "getter" || a.Got <> ("getter key", Some @"{getterPath}", Some 201) then
        failwithf "Getter: %%A, %%A" result a.Got
    a.Item("forward none", ?path = None, ?line = None) <- "none payload"
    check ("forward none", None, None, "none payload")
    a.Item("forward some", ?path = Some "forward.fs", ?line = Some 19) <- "some payload"
    check ("forward some", Some "forward.fs", Some 19, "some payload")
    m.Item(?caller = None) <- "explicit none"
    if m.Seen <> (None, "explicit none") then failwithf "Explicit member: %%A" m.Seen
{omissions}
[<EntryPoint>]
let main _ = consume (); 0
"""

[<Theory>]
[<InlineData(true, true)>]
[<InlineData(false, true)>]
[<InlineData(false, false)>]
let ``Issue 20046 - native option binding modes and index named value`` native omitted =
    let value = optionalParameter native "value" "int"
    let tag = optionalParameter native "tag" "string"

    let omissions =
        if omitted then
            """
    t.Item() <- "omitted"
    check (None, None, "omitted")
    t.Item(value = 41) <- "partial"
    check (Some 41, None, "partial")
    t.Item(tag = "only tag") <- "partial reordered"
    check (None, Some "only tag", "partial reordered")
"""
        else
            ""

    run
        $"""
module Program
type T() =
    member val Seen = (None, None, "") with get, set
    member t.Item with set ({value}, {tag}) (v: string) = t.Seen <- value, tag, v
[<EntryPoint>]
let main _ =
    let t = T()
    let check expected = if t.Seen <> expected then failwithf "Arguments: %%A <> %%A" t.Seen expected
    t[11, "positional"] <- "first"
    check (Some 11, Some "positional", "first")
    t.Item(value = 12, tag = "named") <- "second"
    check (Some 12, Some "named", "second")
    t.[tag = "reordered", value = 13] <- "third"
    check (Some 13, Some "reordered", "third")
    t.Item(?value = Some 14, ?tag = Some "forward") <- "fourth"
    check (Some 14, Some "forward", "fourth")
    t.Item(?value = None, ?tag = None) <- "fifth"
    check (None, None, "fifth")
    t.set_Item(15, "accessor", "sixth")
    check (Some 15, Some "accessor", "sixth")
{omissions}
    0
"""

[<Fact>]
let ``Issue 20046 - minimum setter-only omission`` () =
    run
        """
module Program
type T() =
    member val Seen: int option * string = Some -1, "" with get, set
    member t.Item with set (?index: int) (v: string) = t.Seen <- index, v
[<EntryPoint>]
let main _ =
    let t = T()
    t.Item() <- "payload"
    if t.Seen <> (None, "payload") then failwithf "Arguments: %A" t.Seen
    0
"""

[<Theory>]
[<InlineData("intrinsic", true)>]
[<InlineData("extension", true)>]
[<InlineData("static", true)>]
[<InlineData("intrinsic", false)>]
[<InlineData("extension", false)>]
[<InlineData("static", false)>]
let ``Issue 20046 - property forms and nested lambda lifetime`` shape native =
    let declaration, receiver, property =
        match shape with
        | "extension" ->
            "type T() =\n    member val Seen = (None, \"\") with get, set\nmodule Extensions =\n  type T with",
            "t",
            "member t.Item"
        | "static" ->
            "type T() =\n    static member val Seen = (None, \"\") with get, set", "T", "static member Indexed"
        | _ -> "type T() =\n    member val Seen = (None, \"\") with get, set", "t", "member t.Item"

    let openExtension = if shape = "extension" then "open Extensions" else ""
    let propertyName = if shape = "static" then "Indexed" else "Item"
    let index = optionalParameter native "index" "int"
    let omittedCall = if native then "" else "?index = None"

    run
        $"""
module Program
{declaration}
    {property}
        with set ({index}) (v: string) =
            let finish suffix = (fun text -> text + suffix) v
            {receiver}.Seen <- index, finish "-nested"
{openExtension}
[<EntryPoint>]
let main _ =
    let t = T()
    {receiver}.{propertyName}({omittedCall}) <- "omitted"
    if {receiver}.Seen <> (None, "omitted-nested") then failwith "Omitted nested setter"
    {receiver}.{propertyName}(23) <- "explicit"
    if {receiver}.Seen <> (Some 23, "explicit-nested") then failwith "Explicit nested setter"
    0
"""

[<Fact>]
let ``Issue 20046 - struct optional setter modes`` () =
    run
        """
module Program
type T() =
    member val Seen = (ValueNone, "") with get, set
    member t.Item with set ([<Struct>] ?index: int) (v: string) = t.Seen <- index, v
[<EntryPoint>]
let main _ =
    let t = T()
    let check expected = if t.Seen <> expected then failwithf "Struct arguments: %A" t.Seen
    t.Item() <- "omitted"
    check (ValueNone, "omitted")
    t[21] <- "ordinary"
    check (ValueSome 21, "ordinary")
    t.Item(?index = ValueSome 22) <- "some"
    check (ValueSome 22, "some")
    t.Item(?index = ValueNone) <- "none"
    check (ValueNone, "none")
    0
"""

[<Theory>]
[<InlineData(true, true, true)>]
[<InlineData(false, true, true)>]
[<InlineData(true, false, true)>]
[<InlineData(false, false, true)>]
[<InlineData(false, true, false)>]
[<InlineData(false, false, false)>]
let ``Issue 20046 - signature generic tuple value and metadata`` native separateAssembly omitted =
    let index = optionalParameter native "index" "int"
    let path = optionalParameter native "path" "string"
    let line = optionalParameter native "line" "int"

    let library =
        Fsi
            """
module Lib
open System.Runtime.CompilerServices
type T<'T> =
    new: unit -> T<'T>
    member Seen: int option * string option * int option * ('T * int)
    member Item:
        ?index: int * [<CallerFilePath>] ?path: string * [<CallerLineNumber>] ?line: int -> ('T * int) with set
"""
        |> withFileName "Lib.fsi"
        |> withAdditionalSourceFile (
            FsSourceWithFileName
                "Lib.fs"
                $"""
module Lib
open System.Runtime.CompilerServices
# 51 "OptionalIndexerLibrary.fs"
type T<'T>() =
    let mutable seen = (None, None, None, (Unchecked.defaultof<'T>, -1))
    member _.Seen = seen
    member _.Item
        with set ({index}, [<CallerFilePath>] {path}, [<CallerLineNumber>] {line}) (v: 'T * int) =
            seen <- index, path, line, v
        """
        )
        |> withName "OptionalIndexerLibrary"
        |> asLibrary

    let omissions =
        if omitted then
            $"""
# 401 "{consumerPath}"
    t.Item() <- ("omitted tuple", 101)
    check (None, Some @"{consumerPath}", Some 401, ("omitted tuple", 101))
# 411 "{consumerPath}"
    t.Item(index = 31) <- ("explicit index", 102)
    check (Some 31, Some @"{consumerPath}", Some 411, ("explicit index", 102))
"""
        else
            ""

    let consumer =
        $"""
module Consumer
open System.Runtime.CompilerServices
open Lib
[<EntryPoint>]
let main _ =
    let t = T<string>()
    let check expected = if t.Seen <> expected then failwithf "Library arguments: %%A <> %%A" t.Seen expected
    t.Item(32, "explicit.fs", 12) <- ("explicit tuple", 103)
    check (Some 32, Some "explicit.fs", Some 12, ("explicit tuple", 103))
{omissions}
    let parameters = typeof<T<string>>.GetMethod("set_Item").GetParameters()
    let types = [| typeof<int option>; typeof<string option>; typeof<int option>; typeof<string * int> |]
    if parameters.Length <> 4 then failwithf "Parameter count: %%d" parameters.Length
    for i = 0 to 3 do
        let p = parameters[i]
        if p.Position <> i || p.ParameterType <> types[i] || p.IsOptional then failwithf "Parameter shape: %%A" p
        if p.IsDefined(typeof<OptionalArgumentAttribute>, false) <> (i < 3) then failwithf "Optional metadata: %%A" p
        if p.IsDefined(typeof<CallerFilePathAttribute>, false) <> (i = 1) then failwithf "File metadata: %%A" p
        if p.IsDefined(typeof<CallerLineNumberAttribute>, false) <> (i = 2) then failwithf "Line metadata: %%A" p
    if parameters[0].Name <> "index" || parameters[1].Name <> "path" || parameters[2].Name <> "line" then
        failwith "Index order"
    0
"""

    (if separateAssembly then
         FSharp consumer |> withReferences [ library ]
     else
         library
         |> withAdditionalSourceFile (FsSourceWithFileName "Consumer.fs" consumer))
    |> asExe
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(true, true)>]
[<InlineData(false, true)>]
[<InlineData(false, false)>]
let ``Issue 20046 - ordinary optional interface setter`` native omitted =
    let index = optionalParameter native "index" "int"

    let omissions =
        if omitted then
            """
    i.Item() <- ("omitted", 51)
    if t.Seen <> (None, ("omitted", 51)) then failwith "Interface omission"
"""
        else
            ""

    run
        $"""
module Program
type I =
    abstract Item: ?index: int -> (string * int) with set
type T() =
    member val Seen = (None, ("", 0)) with get, set
    interface I with
        member t.Item with set ({index}) (v: string * int) = t.Seen <- index, v
[<EntryPoint>]
let main _ =
    let t = T()
    let i = t :> I
    i.Item(42) <- ("explicit", 52)
    if t.Seen <> (Some 42, ("explicit", 52)) then failwith "Interface explicit"
{omissions}
    0
"""

let private csharpLibrary =
    CSharp
        """
using System;
using System.Runtime.CompilerServices;
public class CliIndexer
{
    public string Key = "", Path = "", Payload = "";
    public int Index, Line;
    public string this[string key, int index = 7, [CallerFilePath] string path = "", [CallerLineNumber] int line = 0]
    {
        set { Key = key; Index = index; Path = path; Line = line; Payload = value; }
    }
    public static void SelfUse()
    {
        var t = new CliIndexer();
#line 71 "CSharpSelf.cs"
        t["self"] = "self payload";
        if (t.Key != "self" || t.Index != 7 || t.Line != 71 ||
            !t.Path.EndsWith("CSharpSelf.cs", StringComparison.Ordinal) || t.Payload != "self payload")
            throw new Exception("C# optional indexer self-use");
        t["explicit", 8, "self override", 72] = "override payload";
        if (t.Key != "explicit" || t.Index != 8 || t.Path != "self override" ||
            t.Line != 72 || t.Payload != "override payload")
            throw new Exception("C# explicit indexer self-use");
    }
}
"""
    |> withName "OptionalCliIndexer"

[<Fact>]
let ``Issue 20046 - CSharp self-use control`` () =
    FSharp "module Program\n[<EntryPoint>]\nlet main _ = CliIndexer.SelfUse(); 0"
    |> withReferences [ csharpLibrary ]
    |> asExe
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false, true)>]
[<InlineData(true, true)>]
[<InlineData(false, false)>]
[<InlineData(true, false)>]
let ``Issue 20046 - CLI defaults caller information and None forwarding`` csharp omitted =
    let declaration =
        if csharp then
            ""
        else
            """
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
type CliIndexer() =
    member val Key = "" with get, set
    member val Index = 0 with get, set
    member val Path = "" with get, set
    member val Line = 0 with get, set
    member val Payload = "" with get, set
    member t.Item
        with set (key: string, [<Optional; DefaultParameterValue(7)>] index: int,
                  [<Optional; DefaultParameterValue(""); CallerFilePath>] path: string,
                  [<Optional; DefaultParameterValue(0); CallerLineNumber>] line: int) (v: string) =
            t.Key <- key
            t.Index <- index
            t.Path <- path
            t.Line <- line
            t.Payload <- v
"""

    let omissions =
        if omitted then
            $"""
# 501 "{consumerPath}"
    t["omitted"] <- "default payload"
    check ("omitted", 7, @"{consumerPath}", 501, "default payload")
# 511 "{consumerPath}"
    t.Item("partial", index = 9, path = "override.fs") <- "partial payload"
    check ("partial", 9, "override.fs", 511, "partial payload")
"""
        else
            ""

    FSharp
        $"""
module Program
{declaration}
[<EntryPoint>]
let main _ =
    let t = CliIndexer()
    let check expected =
        let actual = t.Key, t.Index, t.Path, t.Line, t.Payload
        if actual <> expected then failwithf "CLI arguments: %%A <> %%A" actual expected
    t.Item("explicit", 8, "explicit.fs", 61) <- "explicit payload"
    check ("explicit", 8, "explicit.fs", 61, "explicit payload")
# 521 "{consumerPath}"
    t.Item("forward none", 10, "forward.fs", ?line = None) <- "forward payload"
    check ("forward none", 10, "forward.fs", 521, "forward payload")
{omissions}
    0
"""
    |> withReferences (if csharp then [ csharpLibrary ] else [])
    |> asExe
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Issue 20046 - named binding can consume the actual RHS formal`` () =
    run
        """
module Program
type T() =
    member val Seen = (None, "") with get, set
    member t.Item with set ([<OptionalArgument>] index: int option) (v: string) = t.Seen <- index, v
[<EntryPoint>]
let main _ =
    let t = T()
    t.Item(v = "named rhs") <- 63
    if t.Seen <> (Some 63, "named rhs") then failwithf "Named RHS: %A" t.Seen
    0
"""

[<Theory>]
[<InlineData(true, true)>]
[<InlineData(false, true)>]
[<InlineData(false, false)>]
let ``Issue 20046 - overload preference and single evaluation`` native omitted =
    let index = optionalParameter native "index" "int"

    let call =
        if omitted then
            """(receiver()).Item(effect "key" "key") <- effect "rhs" "payload" """
        else
            """(receiver()).Item(effect "key" "key", effect "index" 73) <- effect "rhs" "payload" """

    let expectedIndex = if omitted then "None" else "Some 73"

    let expectedEvents =
        if omitted then
            """["receiver"; "key"; "rhs"; "setter"]"""
        else
            """["receiver"; "key"; "index"; "rhs"; "setter"]"""

    run
        $"""
module Program
let events = ResizeArray<string>()
let effect label value = events.Add label; value
type T() =
    member val Seen = ("", None, "") with get, set
    member t.Item with set (key: string, {index}) (v: string) =
        events.Add "setter"
        t.Seen <- key, index, v
type Overloaded() =
    member val Seen = "" with get, set
    member t.Item with set (key: string) (v: string) = t.Seen <- key + ":" + v
    member t.Item with set (key: string, {index}) (v: string) = failwith "Optional overload selected"
[<EntryPoint>]
let main _ =
    let t = T()
    let receiver () = effect "receiver" t
    {call}
    if t.Seen <> ("key", {expectedIndex}, "payload") then failwithf "Evaluation arguments: %%A" t.Seen
    if List.ofSeq events <> {expectedEvents} then failwithf "Evaluation order/count: %%A" events
    let overloaded = Overloaded()
    overloaded["preferred"] <- "non-optional"
    if overloaded.Seen <> "preferred:non-optional" then failwith "Overload preference"
    0
"""

[<Theory>]
[<InlineData(true)>]
[<InlineData(false)>]
let ``Issue 20046 - CLI optional before ParamArray`` omitted =
    let omission =
        if omitted then
            """
    t.Item() <- "omitted"
    check (7, [||], "omitted")
"""
        else
            ""

    run
        $"""
module Program
open System
open System.Runtime.InteropServices
type T() =
    member val Seen = (0, [||], "") with get, set
    member t.Item
        with set ([<Optional; DefaultParameterValue(7)>] offset: int, [<ParamArray>] rest: int[]) (v: string) =
            t.Seen <- offset, rest, v
[<EntryPoint>]
let main _ =
    let t = T()
    let check expected = if t.Seen <> expected then failwithf "ParamArray arguments: %%A <> %%A" t.Seen expected
    t[11, 12, 13] <- "expanded"
    check (11, [|12; 13|], "expanded")
    t[21] <- "empty"
    check (21, [||], "empty")
    t[31, [|32; 33|]] <- "array"
    check (31, [|32; 33|], "array")
{omission}
    0
"""

[<Fact>]
let ``Issue 20046 - non-indexed tuple value control`` () =
    run
        """
module Program
type T() =
    member val Seen = (0, "", 0) with get, set
    member t.Value with set (a: int, b: string, c: int) = t.Seen <- a, b, c
[<EntryPoint>]
let main _ =
    let t = T()
    t.Value <- (81, "tuple value", 83)
    if t.Seen <> (81, "tuple value", 83) then failwithf "Tuple value: %A" t.Seen
    0
"""

[<Theory>]
[<InlineData("member _.M(?x:int, y:string) = ()", 1212, 3, 15, 33)>]
[<InlineData("member _.Item with set (?x:int, y:string) (v:int) = ()", 1212, 3, 28, 54)>]
[<InlineData("member _.Value with set (a:int, ?b:int, c:int) = ()", 1212, 3, 29, 51)>]
[<InlineData("member _.Item with set (k:int) ((a:int, ?b:int)) = ()", 718, 3, 45, 47)>]
[<InlineData("member _.M() =\n        let f (?x:int) = ()\n        ()", 718, 4, 16, 18)>]
[<InlineData("member _.Item with set (k:int, [<OptionalArgument>] index:int option) (v:int) =\n        let f = fun (?x:int, y:int) -> ()\n        f (None, 1)",
             1212,
             4,
             21,
             36)>]
[<InlineData("member _.Item with set (?offset:int, [<System.ParamArray>] rest:int[]) (v:int) = ()", 1212, 3, 28, 83)>]
let ``Issue 20046 - optional pattern validation boundaries`` (memberSource: string) code line startColumn endColumn =
    let message =
        match code with
        | 1212 -> "Optional arguments must come at the end of the argument list, after any non-optional arguments"
        | _ -> "Optional arguments are only permitted on type members"

    FSharp $"module Program\ntype T() =\n    {memberSource}\n"
    |> typecheck
    |> shouldFail
    |> withDiagnostics [ (Error code, Line line, Col startColumn, Line line, Col endColumn, message) ]

[<Theory>]
[<InlineData("t.Item() <- \"payload\"",
             501,
             1,
             22,
             "The member or object constructor 'Item' takes 3 argument(s) but is here given 1. The required signature is 'member T.Item: key: string * ?index: int -> string with set'.")>]
[<InlineData("t.Item(\"key\", 1, 2) <- \"payload\"",
             501,
             1,
             33,
             "The member or object constructor 'Item' takes 3 argument(s) but is here given 4. The required signature is 'member T.Item: key: string * ?index: int -> string with set'.")>]
[<InlineData("t.Item(\"key\", index = \"wrong\") <- \"payload\"",
             1,
             23,
             30,
             "This expression was expected to have type\n    'int'    \nbut here has type\n    'string'    ")>]
[<InlineData("t.Item(\"key\", 1) <- 42",
             1,
             21,
             23,
             "This expression was expected to have type\n    'string'    \nbut here has type\n    'int'    ")>]
let ``Issue 20046 - invalid setter calls have ordinary diagnostics`` (call: string) code startColumn endColumn message =
    FSharp
        $"""module Program
type T() =
    member _.Item with set (key:string, [<OptionalArgument>] index:int option) (v:string) = ()
let t = T()
{call}
"""
    |> typecheck
    |> shouldFail
    |> withDiagnostics [ (Error code, Line 5, Col startColumn, Line 5, Col endColumn, message) ]

[<Fact>]
let ``Issue 20046 - struct optional forwarding retains language version 9 restriction`` () =
    FSharp
        """module Program
type T() =
    static member M([<Struct>] ?index:int) = ()
T.M(?index = ValueSome 1)
"""
    |> withLangVersion90
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ Error 1,
          Line 4,
          Col 14,
          Line 4,
          Col 25,
          "This expression was expected to have type\n    'int option'    \nbut here has type\n    ''a voption'    " ]

[<Fact>]
let ``Issue 20046 - ordinary optional function remains invalid`` () =
    FSharp "module Program\nlet f (?index:int) = ()\n"
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ Error 718, Line 2, Col 8, Line 2, Col 14, "Optional arguments are only permitted on type members" ]

[<Fact>]
let ``Issue 20046 - struct annotation retains option semantics in language version 9`` () =
    FSharp
        """
module Program
type T() =
    static member M([<Struct>] ?index:int) = index
[<EntryPoint>]
let main _ =
    if T.M() <> None then failwith "Omitted option"
    if T.M(27) <> Some 27 then failwith "Ordinary option"
    if T.M(?index = Some 28) <> Some 28 then failwith "Forwarded option"
    if T.M(?index = None) <> None then failwith "Forwarded None"
    0
"""
    |> withLangVersion90
    |> asExe
    |> compileExeAndRun
    |> shouldSucceed
