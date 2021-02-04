module Tests.Parser

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

module Recovery =
    [<Test>]
    let ``Unfinished interface member`` () =
        let parseResults = getParseResults """
type T =
    interface I with
    member x.P2 = ()

let x = ()
"""
        let (SynModuleOrNamespace (decls = decls)) = getSingleModuleLikeDecl parseResults
        match decls with
        | [ SynModuleDecl.Types ([ SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members = [ _; _ ])) ], _)
            SynModuleDecl.Let _ ] -> ()
        | _ -> failwith "Unexpected tree"

    [<Test>]
    let ``Union case 01 - of`` () =
        let parseResults = getParseResults """
type U1 =
    | A of

type U2 =
    | B of
    | C

let x = ()
    """
        let (|UnionWithCases|_|) typeDefn =
            match typeDefn with
            | SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (SynTypeDefnSimpleRepr.Union (unionCases = cases), _)) ->
                cases |> List.map (fun (SynUnionCase (ident = ident)) -> ident.idText) |> Some
            | _ -> None

        let (SynModuleOrNamespace (decls = decls)) = getSingleModuleLikeDecl parseResults
        match decls with
        | [ SynModuleDecl.Types ([ UnionWithCases ["A"]], _)
            SynModuleDecl.Types ([ UnionWithCases ["B"; "C"] ], _)
            SynModuleDecl.Let _ ] -> ()
        | _ -> failwith "Unexpected tree"
