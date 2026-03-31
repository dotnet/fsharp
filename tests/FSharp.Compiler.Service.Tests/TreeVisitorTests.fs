module FSharp.Compiler.Service.Tests.TreeVisitorTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Syntax
open Xunit

[<Fact>]
let ``Visit type test`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr
            member x.VisitType(_, _, _) = Some () }

    let source = "123 :? int"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    SyntaxTraversal.Traverse(mkPos 1 11, parseTree, visitor)
    |> Option.defaultWith (fun _ -> failwith "Did not visit type")

    SyntaxTraversal.Traverse(mkPos 1 3, parseTree, visitor)
    |> Option.iter (fun _ -> failwith "Should not visit type")

[<Fact>]
let ``Visit record definition test`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitRecordDefn(_, fields, _) = Some fields }

    let source = "type R = { A: int; B: string }"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some [ SynField (idOpt = Some id1); SynField (idOpt = Some id2) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit record definition"

[<Fact>]
let ``Visit union definition test`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitUnionDefn(_, cases, _) = Some cases }

    let source = "type U = A | B of string"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some [ SynUnionCase (ident = SynIdent(id1,_)); SynUnionCase (ident = SynIdent(id2,_)) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit union definition"

[<Fact>]
let ``Visit enum definition test`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitEnumDefn(_, cases, _) = Some cases }

    let source = "type E = A = 0 | B = 1"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some [ SynEnumCase (ident = SynIdent (id1, _)); SynEnumCase (ident = SynIdent (id2, _)) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit enum definition"

[<Fact>]
let ``Visit recursive let binding`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr
            member x.VisitLetOrUse(_, isRecursive, _, bindings, _) =
                if not isRecursive then failwith $"{nameof isRecursive} should be true"
                Some bindings }

    let source = "let rec fib n = if n < 2 then n else fib (n - 1) + fib (n - 2) in fib 10"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some [ SynBinding(valData = SynValData(valInfo = SynValInfo(curriedArgInfos = [ [ SynArgInfo(ident = Some id) ] ]))) ] when id.idText = "n" -> ()
    | _ -> failwith "Did not visit recursive let binding"

[<Fact>]
let ``Visit ValSig`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitValSig(path, defaultTraverse, SynValSig(ident = SynIdent(ident = valIdent))) =
                Some valIdent.idText
        }

    let source = """
module X

val y: int -> int
"""

    let parseTree = parseSourceCode("C:\\test.fsi", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some "y" -> ()
    | _ -> failwith "Did not visit SynValSig"

[<Fact>]
let ``Visit nested ValSig`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitValSig(path, defaultTraverse, SynValSig(ident = SynIdent(ident = valIdent))) =
                Some valIdent.idText
        }

    let source = """
module X

module Y =
    val z: int -> int
"""

    let parseTree = parseSourceCode("C:\\test.fsi", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some "z" -> ()
    | _ -> failwith "Did not visit SynValSig"

[<Fact>]
let ``Visit Record in SynTypeDefnSig`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitRecordDefn(path, fields, range) =
                fields
                |> List.choose (fun (SynField(idOpt = idOpt)) -> idOpt |> Option.map (fun ident -> ident.idText))
                |> String.concat ","
                |> Some
        }

    let source = """
module X

type Y =
    {
        A: int
        B: char
        C: string
    }
"""

    let parseTree = parseSourceCode("C:\\test.fsi", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some "A,B,C" -> ()
    | _ -> failwith "Did not visit SynTypeDefnSimpleRepr.Record in SynTypeDefnSig"

[<Fact>]
let ``Visit SynValSig in SynMemberSig`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitValSig(path, defaultTraverse, SynValSig(ident = SynIdent(ident = valIdent))) =
                Some valIdent.idText
        }

    let source = """
module Lib

type Meh =
    new: unit -> Meh
    member Foo: y: int -> int
"""

    let parseTree = parseSourceCode("C:\\test.fsi", source)
    let pos = mkPos 6 4

    match SyntaxTraversal.Traverse(pos, parseTree, visitor) with
    | Some "Foo" -> ()
    | _ -> failwith "Did not visit SynValSig in SynMemberSig.Member"

// https://github.com/dotnet/fsharp/issues/13114
[<Fact>]
let ``Issue 13114 - defaultTraverse walks into SynPat.Record`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr

            member x.VisitPat(_, defaultTraverse, pat) =
                match pat with
                | SynPat.Named _ -> Some pat
                | _ -> defaultTraverse pat }

    let source =
        """
type R = { A: int }
let f (r: R) =
    match r with
    | { A = a } -> a
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(mkPos 5 13, parseTree, visitor) with
    | Some(SynPat.Named(ident = SynIdent(ident = ident))) when ident.idText = "a" -> ()
    | other -> failwith $"defaultTraverse did not walk into SynPat.Record fields, got: %A{other}"

// https://github.com/dotnet/fsharp/issues/13114
[<Fact>]
let ``Issue 13114 - defaultTraverse walks into SynPat.QuoteExpr`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitExpr(_, _, defaultTraverse, expr) =
                match expr with
                | SynExpr.Const(SynConst.Int32 42, _) -> Some expr
                | _ -> defaultTraverse expr

            member x.VisitPat(_, defaultTraverse, pat) = defaultTraverse pat }

    let source =
        """
let f x =
    match x with
    | <@ 42 @> -> ()
    | _ -> ()
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(mkPos 4 8, parseTree, visitor) with
    | Some(SynExpr.Const(SynConst.Int32 42, _)) -> ()
    | other -> failwith $"defaultTraverse did not walk into SynPat.QuoteExpr, got: %A{other}"

// https://github.com/dotnet/fsharp/issues/13114
[<Fact>]
let ``Issue 13114 - defaultTraverse walks into SynPat.FromParseError`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr

            member x.VisitPat(_, defaultTraverse, pat) =
                match pat with
                | SynPat.Named _ -> Some pat
                | _ -> defaultTraverse pat }

    // SynPat.FromParseError wraps the inner pat when a parse error occurs;
    // defaultTraverse should descend into the wrapped pattern.
    let source =
        """
let f x =
    match x with
    | (a) -> a
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(mkPos 4 7, parseTree, visitor) with
    | Some(SynPat.Named _) -> ()
    | other -> failwith $"defaultTraverse did not walk into nested SynPat, got: %A{other}"

// https://github.com/dotnet/fsharp/issues/13114
[<Fact>]
let ``Issue 13114 - defaultTraverse walks into SynPat.IsInst`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr

            member x.VisitPat(_, defaultTraverse, pat) = defaultTraverse pat

            member x.VisitType(_, _, ty) =
                match ty with
                | SynType.LongIdent _ -> Some ty
                | _ -> None }

    let source =
        """
let f (x: obj) =
    match x with
    | :? string -> ()
    | _ -> ()
"""

    let parseTree = parseSourceCode ("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(mkPos 4 11, parseTree, visitor) with
    | Some(SynType.LongIdent _) -> ()
    | other -> failwith $"defaultTraverse did not walk into SynPat.IsInst, got: %A{other}"

