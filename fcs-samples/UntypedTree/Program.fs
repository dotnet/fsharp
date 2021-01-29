// Open the namespace with InteractiveChecker type
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Syntax

// Create a checker instance (ignore notifications)
let checker = FSharpChecker.Create()

// ------------------------------------------------------------------

// Get untyped tree for a specified input
let getUntypedTree (file, input) = 
  let parsingOptions = { FSharpParsingOptions.Default with SourceFiles = [| file |] }
  let untypedRes = checker.ParseFile(file, FSharp.Compiler.Text.SourceText.ofString input, parsingOptions) |> Async.RunSynchronously
  match untypedRes.ParseTree with
  | Some tree -> tree
  | None -> failwith "Something went wrong during parsing!"

// ------------------------------------------------------------------

/// Walk over all module or namespace declarations 
/// (basically 'module Foo =' or 'namespace Foo.Bar')
/// Note that there is one implicitly, even if the file
/// does not explicitly define it..
let rec visitModulesAndNamespaces modulesOrNss =
  for moduleOrNs in modulesOrNss do
    let (SynModuleOrNamespace(lid, isRec, isModule, decls, xmlDoc, attribs, synAccess, m)) = moduleOrNs
    printfn "Namespace or module: %A" lid
    visitDeclarations decls

/// Walk over a pattern - this is for example used in 
/// let <pat> = <expr> or in the 'match' expression
and visitPattern = function
  | SynPat.Wild(_) -> 
      printfn "  .. underscore pattern"
  | SynPat.Named(pat, name, _, _, _) ->
      visitPattern pat
      printfn "  .. named as '%s'" name.idText
  | SynPat.LongIdent(LongIdentWithDots(ident, _), _, _, _, _, _) ->
      printfn "  identifier: %s" (String.concat "." [ for i in ident -> i.idText ])
  | pat -> printfn " - not supported pattern: %A" pat

/// Walk over an expression - the most interesting part :-)
and visitExpression = function
  | SynExpr.IfThenElse(cond, trueBranch, falseBranchOpt, _, _, _, _) ->
      printfn "Conditional:"
      visitExpression cond
      visitExpression trueBranch
      falseBranchOpt |> Option.iter visitExpression 

  | SynExpr.LetOrUse(_, _, bindings, body, _) ->
      printfn "LetOrUse with the following bindings:"
      for binding in bindings do
        let (Binding(access, kind, inlin, mutabl, attrs, xmlDoc, data, pat, retInfo, body, m, sp)) = binding
        visitPattern pat 
      printfn "And the following body:"
      visitExpression body
  | expr -> printfn " - not supported expression: %A" expr

/// Walk over a list of declarations in a module. This is anything
/// that you can write as a top-level inside module (let bindings,
/// nested modules, type declarations etc.)
and visitDeclarations decls = 
  for declaration in decls do
    match declaration with
    | SynModuleDecl.Let(isRec, bindings, range) ->
        for binding in bindings do
          let (Binding(access, kind, inlin, mutabl, attrs, xmlDoc, data, pat, retInfo, body, m, sp)) = binding
          visitPattern pat 
          visitExpression body         
    | _ -> printfn " - not supported declaration: %A" declaration


// ------------------------------------------------------------------

// Sample input for the compiler service
let input = """
  let foo() = 
    let msg = "Hello world"
    if true then 
      printfn "%s" msg """
let file = "/home/user/Test.fsx"

let tree = getUntypedTree(file, input) 

// Testing: Print the AST to see what it looks like
// tree |> printfn "%A"

match tree with
| ParsedInput.ImplFile(ParsedImplFileInput(file, isScript, qualName, pragmas, hashDirectives, modules, b)) ->
    visitModulesAndNamespaces modules
| _ -> failwith "F# Interface file (*.fsi) not supported."