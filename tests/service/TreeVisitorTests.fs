module Tests.Service.TreeVisitorTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text.Pos
open FSharp.Compiler.SourceCodeServices.AstTraversal
open NUnit.Framework

[<Test>]
let ``Visit type test`` () =
    let visitor =
        { new AstVisitorBase<_>() with
            member x.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr
            member x.VisitType(_, _) = Some () }

    let source = "123 :? int"
    let parseTree =
        match parseSourceCode("C:\\test.fs", source) with
        | None -> failwith "No parse tree"
        | Some parseTree -> parseTree

    Traverse(mkPos 1 11, parseTree, visitor)
    |> Option.defaultWith (fun _ -> failwith "Did not visit type")

    Traverse(mkPos 1 3, parseTree, visitor)
    |> Option.iter (fun _ -> failwith "Should not visit type")
