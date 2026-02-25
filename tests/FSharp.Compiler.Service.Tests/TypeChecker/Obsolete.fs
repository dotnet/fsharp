module FSharp.Compiler.Service.Tests.TypeChecker.Obsolete

open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Symbols
open FSharp.Test.Assert
open Xunit

let checkObsoleteMessages message source =
    let symbolUse = Checker.getSymbolUse source
    symbolUse.Symbol.ObsoleteDiagnosticInfo.Value.Message.Value |> shouldEqual message

let checkNotObsolete source =
    let symbolUse = Checker.getSymbolUse source
    symbolUse.Symbol.ObsoleteDiagnosticInfo |> shouldEqual None

[<Fact>]
let ``Method 01`` () =
    checkObsoleteMessages "Message" """
type Class() =
    [<System.Obsolete "Message">]
    static member Method{caret}() = ()
"""

[<Fact>]
let ``Method 02`` () =
    checkObsoleteMessages "Message" """
type Class() =
    [<System.Obsolete "Message">]
    static member Method() = ()

Class.Method{caret}()
"""

[<Fact>]
let ``Method 03`` () =
    checkObsoleteMessages "Message" """
type Class() =
    static member Method(i: int) = ()

    [<System.Obsolete "Message">]
    static member Method(s: string) = ()

Class.Method{caret}("")
"""

[<Fact>]
let ``Method 04`` () =
    checkNotObsolete """
type Class() =
    static member Method(i: int) = ()

    [<System.Obsolete "Message">]
    static member Method(s: string) = ()

Class.Method{caret}(1)
"""

[<Fact>]
let ``Method 05`` () =
    let methodOverloads =
        Checker.getMethodOverloads ["Class"; "Method"] """
type Class() =
    static member Method(i: int) = ()

    [<System.Obsolete "Message">]
    static member Method(s: string) = ()

Class.Method({caret}1)
"""
    methodOverloads.Value
    |> List.sortBy _.Symbol.DeclarationLocation.Value.StartLine
    |> List.map (_.Symbol.ObsoleteDiagnosticInfo >> (Option.bind _.Message))
    |> shouldEqual [ None; Some "Message"]

[<Fact>]
let ``Type 01 - Union`` () =
    checkObsoleteMessages "Message" """
[<System.Obsolete "Message">]
type U{caret} = | A
"""

[<Fact>]
let ``Value 01`` () =
    checkObsoleteMessages "Message" """
[<System.Obsolete "Message">]
let s{caret} = ""
"""

[<Fact>]
let ``Value 02`` () =
    checkObsoleteMessages "Message" """
[<System.Obsolete "Message">]
let s = ""
let s1 = s{caret}
"""
