module FSharp.Compiler.Service.Tests.SyntaxTreeTests.StringTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework
open FsUnit

let private getBindingExpressionValue (parseResults: ParsedInput) =
    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = modules)) ->
        modules |> List.tryPick (fun (SynModuleOrNamespace (decls = decls)) ->
            decls |> List.tryPick (fun decl ->
                match decl with
                | SynModuleDecl.Let (bindings = bindings) ->
                    bindings |> List.tryPick (fun binding ->
                        match binding with
                        | SynBinding.SynBinding (headPat=(SynPat.Named _|SynPat.As(_,SynPat.Named _,_)); expr=e) -> Some e
                        | _ -> None)
                | _ -> None))
    | _ -> None

let private getBindingConstValue parseResults =
    match getBindingExpressionValue parseResults with
    | Some (SynExpr.Const(c,_)) -> Some c
    | _ -> None

[<Test>]
let ``SynConst.String with SynStringKind.Regular`` () =
    let parseResults =
        getParseResults
            """
let s = "yo"
"""

    match getBindingConstValue parseResults with
    | Some (SynConst.String (_,  kind, _)) -> kind |> should equal SynStringKind.Regular
    | _ -> Assert.Fail "Couldn't find const"

[<Test>]
let ``SynConst.String with SynStringKind.Verbatim`` () =
    let parseResults =
        getParseResults
            """
let s = @"yo"
"""

    match getBindingConstValue parseResults with
    | Some (SynConst.String (_,  kind, _)) -> kind |> should equal SynStringKind.Verbatim
    | _ -> Assert.Fail "Couldn't find const"

[<Test>]
let ``SynConst.String with SynStringKind.TripleQuote`` () =
    let parseResults =
        getParseResults
            "
let s = \"\"\"yo\"\"\"
"

    match getBindingConstValue parseResults with
    | Some (SynConst.String (_,  kind, _)) -> kind |> should equal SynStringKind.TripleQuote
    | _ -> Assert.Fail "Couldn't find const"

[<Test>]
let ``SynConst.Bytes with SynByteStringKind.Regular`` () =
    let parseResults =
        getParseResults
            """
let bytes = "yo"B
"""

    match getBindingConstValue parseResults with
    | Some (SynConst.Bytes (_,  kind, _)) -> kind |> should equal SynByteStringKind.Regular
    | _ -> Assert.Fail "Couldn't find const"

[<Test>]
let ``SynConst.Bytes with SynByteStringKind.Verbatim`` () =
    let parseResults =
        getParseResults
            """
let bytes = @"yo"B
"""

    match getBindingConstValue parseResults with
    | Some (SynConst.Bytes (_,  kind, _)) -> kind |> should equal SynByteStringKind.Verbatim
    | _ -> Assert.Fail "Couldn't find const"

[<Test>]
let ``SynExpr.InterpolatedString with SynStringKind.TripleQuote`` () =
    let parseResults =
        getParseResults
            "
let s = $\"\"\"yo {42}\"\"\"
"

    match getBindingExpressionValue parseResults with
    | Some (SynExpr.InterpolatedString(_,  kind, _)) -> kind |> should equal SynStringKind.TripleQuote
    | _ -> Assert.Fail "Couldn't find const"

[<Test>]
let ``SynExpr.InterpolatedString with SynStringKind.Regular`` () =
    let parseResults =
        getParseResults
            """
let s = $"yo {42}"
"""

    match getBindingExpressionValue parseResults with
    | Some (SynExpr.InterpolatedString(_,  kind, _)) -> kind |> should equal SynStringKind.Regular
    | _ -> Assert.Fail "Couldn't find const"

[<Test>]
let ``SynExpr.InterpolatedString with SynStringKind.Verbatim`` () =
    let parseResults =
        getParseResults
            """
let s = $@"Migrate notes of file ""{oldId}"" to new file ""{newId}""."
"""

    match getBindingExpressionValue parseResults with
    | Some (SynExpr.InterpolatedString(_,  kind, _)) -> kind |> should equal SynStringKind.Verbatim
    | _ -> Assert.Fail "Couldn't find const"
