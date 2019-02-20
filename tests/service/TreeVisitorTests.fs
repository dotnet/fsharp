module Tests.Service.TreeVisitorTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices.AstTraversal
open NUnit.Framework

[<Test>]
let ``Visit type test`` () =
    let visitor =
        { new AstVisitorBase<_>() with
            member x.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr
            member x.VisitType(_, _) = Some () }

    let source = "123 :? int"
    let parseTree = parseSource source

    Traverse(mkPos 1 11, parseTree, visitor)
    |> Option.defaultWith (fun _ -> failwith "Did not visit type")

    Traverse(mkPos 1 3, parseTree, visitor)
    |> Option.iter (fun _ -> failwith "Should not visit type")
