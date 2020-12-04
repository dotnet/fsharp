module Tests.Parser

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.SyntaxTree
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
        | [ SynModuleDecl.Types ([ TypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members = [ _; _ ])) ], _)
            SynModuleDecl.Let _ ] -> ()
        | _ -> failwith "Unexpected tree"
