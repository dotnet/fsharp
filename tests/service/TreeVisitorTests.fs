module Tests.Service.TreeVisitorTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
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

[<Test>]
let ``Visit record definition test`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitRecordDefn(_, fields, _) = Some fields }

    let source = "type R = { A: int; B: string }"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some [ SynField (_, _, Some id1, _, _, _, _, _); SynField (_, _, Some id2, _, _, _, _, _) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit record definition"

[<Test>]
let ``Visit union definition test`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitUnionDefn(_, cases, _) = Some cases }

    let source = "type U = A | B of string"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some [ SynUnionCase (_, id1, _, _, _, _); SynUnionCase (_, id2, _, _, _, _) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit union definition"

[<Test>]
let ``Visit enum definition test`` () =
    let visitor =
        { new SyntaxVisitorBase<_>() with
            member x.VisitEnumDefn(_, cases, _) = Some cases }

    let source = "type E = A = 0 | B = 1"
    let parseTree = parseSourceCode("C:\\test.fs", source)

    match SyntaxTraversal.Traverse(pos0, parseTree, visitor) with
    | Some [ SynEnumCase (_, id1, _, _, _, _, _); SynEnumCase (_, id2, _, _, _, _, _) ] when id1.idText = "A" && id2.idText = "B" -> ()
    | _ -> failwith "Did not visit enum definition"