module FSharp.Compiler.Service.Tests.ParameterInfoPatternMatchingTests

open Xunit

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Single.InMatchClause`` () =
    assertParameterInfoOverloads
        [ ["format"; "arg0"]
          ["format"; "args"]
          ["provider"; "format"; "args"]
          ["format"; "arg0"; "arg1"]
          ["format"; "arg0"; "arg1"; "arg2"]
          ["provider"; "format"; "arg0"]
          ["provider"; "format"; "arg0"; "arg1"]
          ["provider"; "format"; "arg0"; "arg1"; "arg2"] ] """
let rec f l =
    match l with
    | [] -> System.String.Format({caret}
    | x :: xs -> f xs"""

[<Fact>]
let ``LocationOfParams.MatchGuard`` () =
    assertHasParameterInfo """match [1] with | [x] when box({caret}x) <> null -> ()"""

[<Fact>]
let ``LocationOfParams.ThisOnceAsserted`` () =
    assertNoParameterInfo """
module CSVTypeProvider

f(fun x ->
    match args with
    | [| y |] ->
        for name, kind in (headerNames,
        rowType.AddMember(new ProvidedProperty({caret}
        null
    | _ -> failwith "unexpected generic params" )"""

[<Fact>]
let ``Multi.MethodInMatchCause`` () =
    assertParameterInfoContains ["format"; "arg0"] """
let rec f l =
    match l with
    | [] -> System.String.For{caret}mat("{0:X2}",
    | x :: xs -> f xs"""

[<Fact(Skip = "93945 - No param info shown on the Indexer Property")>]
let ``Regression.Multi.IndexerProperty.Bug93945`` () =
    assertParameterInfoOverloads [["int"; "int"]] """
type Year2(year : int) =
    member this.Item (month : int, day : int) = month + day

let O'seven = new Year2(2007)
let randomDay = O'seven.[12,{caret}"""
