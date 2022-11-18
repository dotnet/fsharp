module ParallelTypeCheckingTests.Code.TrieApproach.AutoOpenDetection

open FSharp.Compiler.Syntax

let private autoOpenShapes =
    set
        [|
            "FSharp.Core.AutoOpenAttribute"
            "Core.AutoOpenAttribute"
            "AutoOpenAttribute"
            "FSharp.Core.AutoOpen"
            "Core.AutoOpen"
            "AutoOpen"
        |]

/// This isn't bullet proof but I wonder who would really alias this very core attribute.
let isAutoOpenAttribute (attribute: SynAttribute) =
    match attribute.ArgExpr with
    | SynExpr.Const(constant = SynConst.Unit _)
    | SynExpr.Const(constant = SynConst.String _)
    | SynExpr.Paren(expr = SynExpr.Const(constant = SynConst.String _)) ->
        let attributeName =
            attribute.TypeName.LongIdent
            |> List.map (fun ident -> ident.idText)
            |> String.concat "."

        autoOpenShapes.Contains attributeName
    | _ -> false

let isAnyAttributeAutoOpen (attributes: SynAttributes) =
    List.exists (fun (atl: SynAttributeList) -> List.exists isAutoOpenAttribute atl.Attributes) attributes

let rec hasNestedModuleWithAutoOpenAttribute (decls: SynModuleDecl list) : bool =
    decls
    |> List.exists (function
        | SynModuleDecl.NestedModule (moduleInfo = SynComponentInfo (attributes = attributes); decls = decls) ->
            isAnyAttributeAutoOpen attributes || hasNestedModuleWithAutoOpenAttribute decls
        | _ -> false)

let rec hasNestedSigModuleWithAutoOpenAttribute (decls: SynModuleSigDecl list) : bool =
    decls
    |> List.exists (function
        | SynModuleSigDecl.NestedModule (moduleInfo = SynComponentInfo (attributes = attributes); moduleDecls = decls) ->
            isAnyAttributeAutoOpen attributes
            || hasNestedSigModuleWithAutoOpenAttribute decls
        | _ -> false)

let hasAutoOpenAttributeInFile (ast: ParsedInput) : bool =
    match ast with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = contents)) ->
        contents
        |> List.exists (fun (SynModuleOrNamespaceSig (attribs = attribs; decls = decls)) ->
            isAnyAttributeAutoOpen attribs || hasNestedSigModuleWithAutoOpenAttribute decls)
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = contents)) ->
        contents
        |> List.exists (fun (SynModuleOrNamespace (attribs = attribs; decls = decls)) ->
            isAnyAttributeAutoOpen attribs || hasNestedModuleWithAutoOpenAttribute decls)

// ==============================================================================================================================
// ==============================================================================================================================

open NUnit.Framework
open FSharp.Compiler.Service.Tests.Common

[<Test>]
let ``detect auto open`` () =
    let file =
        @"C:\Users\nojaf\Projects\safesparrow-fsharp\src\Compiler\Utilities\ImmutableArray.fsi"

    let ast = parseSourceCode (file, System.IO.File.ReadAllText(file))
    Assert.True(hasAutoOpenAttributeInFile ast)
