(**
---
title: Expressions
category: Compiler Service
categoryindex: 2
index: 3
---
*)
(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Processing SyntaxTree
=================================================

This tutorial demonstrates how to get the SyntaxTree (AST)
for F# code and how to walk over the tree. This can be used for creating tools
such as code formatter, basic refactoring or code navigation tools. The untyped
syntax tree contains information about the code structure, but does not contain
types and there are some ambiguities that are resolved only later by the type
checker. You can also combine the SyntaxTree information with the API available
from [editor services](editor.html). 

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published


Getting the SyntaxTree
-----------------------

To access the untyped AST, you need to create an instance of `FSharpChecker`.
This type represents a context for type checking and parsing and corresponds either
to a stand-alone F# script file (e.g. opened in Visual Studio) or to a loaded project
file with multiple files. Once you have an instance of `FSharpChecker`, you can
use it to perform "untyped parse" which is the first step of type-checking. The
second phase is "typed parse" and is used by [editor services](editor.html).

To use the interactive checker, reference `FSharp.Compiler.Service.dll` and open the
`SourceCodeServices` namespace:
*)
#r "FSharp.Compiler.Service.dll"
open System
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
(**

### Performing untyped parse

The untyped parse operation is very fast (compared to type checking, which can 
take notable amount of time) and so we can perform it synchronously. First, we
need to create `FSharpChecker` - the constructor takes an argument that
can be used to notify the checker about file changes (which we ignore).

*)
// Create an interactive checker instance 
let checker = FSharpChecker.Create()
(**

To get the AST, we define a function that takes file name and the source code
(the file is only used for location information and does not have to exist).
We first need to get "interactive checker options" which represents the context.
For simple tasks, you can use `GetProjectOptionsFromScriptRoot` which infers
the context for a script file. Then we use the `ParseFile` method and
return the `ParseTree` property:

*)
/// Get untyped tree for a specified input
let getUntypedTree (file, input) = 
  // Get compiler options for the 'project' implied by a single script file
  let projOptions, errors = 
      checker.GetProjectOptionsFromScript(file, input, assumeDotNetFramework=false)
      |> Async.RunSynchronously

  let parsingOptions, _errors = checker.GetParsingOptionsFromProjectOptions(projOptions)

  // Run the first phase (untyped parsing) of the compiler
  let parseFileResults = 
      checker.ParseFile(file, input, parsingOptions) 
      |> Async.RunSynchronously

  parseFileResults.ParseTree
  
(**

Walking over the AST
--------------------

The abstract syntax tree is defined as a number of discriminated unions that represent
different syntactical elements (such as expressions, patterns, declarations etc.). The best
way to understand the AST is to look at the definitions in [`ast.fs` in the source 
code](https://github.com/fsharp/fsharp/blob/master/src/fsharp/ast.fs#L464).

The relevant parts are in the following namespace:
*)
open FSharp.Compiler.Syntax
(**

When processing the AST, you will typically write a number of mutually recursive functions
that pattern match on the different syntactical elements. There is a number of elements
that need to be supported - the top-level element is module or namespace declaration, 
containing declarations inside a module (let bindings, types etc.). A let declaration inside
a module then contains expression, which can contain patterns.

### Walking over patterns and expressions

We start by looking at functions that walk over expressions and patterns - as we walk,
we print information about the visited elements. For patterns, the input is of type
`SynPat` and has a number of cases including `Wild` (for `_` pattern), `Named` (for
`<pat> as name`) and `LongIdent` (for a `Foo.Bar` name). Note that the parsed pattern
is occasionally more complex than what is in the source code (in particular, `Named` is
used more often):
*)
/// Walk over a pattern - this is for example used in 
/// let <pat> = <expr> or in the 'match' expression
let rec visitPattern = function
  | SynPat.Wild(_) -> 
      printfn "  .. underscore pattern"
  | SynPat.Named(name, _, _, _) ->
      printfn "  .. named as '%s'" name.idText
  | SynPat.LongIdent(LongIdentWithDots(ident, _), _, _, _, _, _) ->
      let names = String.concat "." [ for i in ident -> i.idText ]
      printfn "  .. identifier: %s" names
  | pat -> printfn "  .. other pattern: %A" pat
(**
The function is recursive (for nested patterns such as `(foo, _) as bar`), but it does not
call any of the functions defined later (because patterns cannot contain other syntactical 
elements).

The next function iterates over expressions - this is where most of the work would be and
there are around 20 cases to cover (type `SynExpr.` and you'll get completion with other 
options). In the following, we only show how to handle `if .. then ..` and `let .. = ...`:
*)
/// Walk over an expression - if expression contains two or three 
/// sub-expressions (two if the 'else' branch is missing), let expression
/// contains pattern and two sub-expressions
let rec visitExpression e = 
  match e with
  | SynExpr.IfThenElse(ifExpr=cond; thenExpr=trueBranch; elseExpr=falseBranchOpt) ->
      // Visit all sub-expressions
      printfn "Conditional:"
      visitExpression cond
      visitExpression trueBranch
      falseBranchOpt |> Option.iter visitExpression 

  | SynExpr.LetOrUse(_, _, bindings, body, _) ->
      // Visit bindings (there may be multiple 
      // for 'let .. = .. and .. = .. in ...'
      printfn "LetOrUse with the following bindings:"
      for binding in bindings do
        let (SynBinding(access, kind, isInline, isMutable, attrs, xmlDoc, data, headPat, retInfo, equalsRange, init, m, debugPoint)) = binding
        visitPattern headPat
        visitExpression init
      // Visit the body expression
      printfn "And the following body:"
      visitExpression body
  | expr -> printfn " - not supported expression: %A" expr
(**
The `visitExpression` function will be called from a function that visits all top-level
declarations inside a module. In this tutorial, we ignore types and members, but that would
be another source of calls to `visitExpression`.

### Walking over declarations

As mentioned earlier, the AST of a file contains a number of module or namespace declarations
(top-level node) that contain declarations inside a module (let bindings or types) or inside
a namespace (just types). The following functions walks over declarations - we ignore types,
nested modules and all other elements and look only at top-level `let` bindings (values and 
functions):
*)
/// Walk over a list of declarations in a module. This is anything
/// that you can write as a top-level inside module (let bindings,
/// nested modules, type declarations etc.)
let visitDeclarations decls = 
  for declaration in decls do
    match declaration with
    | SynModuleDecl.Let(isRec, bindings, range) ->
        // Let binding as a declaration is similar to let binding
        // as an expression (in visitExpression), but has no body
        for binding in bindings do
          let (SynBinding(access, kind, isInline, isMutable, attrs, xmlDoc, 
                          valData, pat, retInfo, equalsRange, body, m, sp)) = binding
          visitPattern pat 
          visitExpression body         
    | _ -> printfn " - not supported declaration: %A" declaration
(**
The `visitDeclarations` function will be called from a function that walks over a 
sequence of module or namespace declarations. This corresponds, for example, to a file 
with multiple `namespace Foo` declarations:
*)
/// Walk over all module or namespace declarations 
/// (basically 'module Foo =' or 'namespace Foo.Bar')
/// Note that there is one implicitly, even if the file
/// does not explicitly define it..
let visitModulesAndNamespaces modulesOrNss =
  for moduleOrNs in modulesOrNss do
    let (SynModuleOrNamespace(lid, isRec, isMod, decls, xml, attrs, _, m)) = moduleOrNs
    printfn "Namespace or module: %A" lid
    visitDeclarations decls
(**
Now that we have functions that walk over the elements of the AST (starting from declaration,
down to expressions and patterns), we can get AST of a sample input and run the above function.

Putting things together
-----------------------

As already discussed, the `getUntypedTree` function uses `FSharpChecker` to run the first
phase (parsing) on the AST and get back the tree. The function requires F# source code together
with location of the file. The location does not have to exist (it is used only for location 
information) and it can be in both Unix and Windows formats:
*)
// Sample input for the compiler service
let input =
  """
  let foo() = 
    let msg = "Hello world"
    if true then 
      printfn "%s" msg
  """

// File name in Unix format
let file = "/home/user/Test.fsx"

// Get the AST of sample F# code
let tree = getUntypedTree(file, SourceText.ofString input)
(**
When you run the code in F# interactive, you can enter `tree;;` in the interactive console and
see pretty printed representation of the data structure - the tree contains a lot of information,
so this is not particularly readable, but it gives you good idea about how the tree looks.

The returned `tree` value is again a discriminated union that can be two different cases - one case
is `ParsedInput.SigFile` which represents F# signature file (`*.fsi`) and the other one is 
`ParsedInput.ImplFile` representing regular source code (`*.fsx` or `*.fs`). The implementation
file contains a sequence of modules or namespaces that we can pass to the function implemented
in the previous step:
*)
// Extract implementation file details
match tree with
| ParsedInput.ImplFile(implFile) ->
    // Extract declarations and walk over them
    let (ParsedImplFileInput(fn, script, name, _, _, modules, _)) = implFile
    visitModulesAndNamespaces modules
| _ -> failwith "F# Interface file (*.fsi) not supported."
(**
Summary
-------
In this tutorial, we looked at basic of working with the untyped abstract syntax tree. This is a 
comprehensive topic, so it is not possible to explain everything in a single article. The 
[Fantomas project](https://github.com/dungpa/fantomas) is a good example of tool based on the untyped
AST that can help you understand more. In practice, it is also useful to combine the information here
with some information you can obtain from the [editor services](editor.html) discussed in the next 
tutorial.
*)
