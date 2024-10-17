(**
---
title: Tutorial: AST APIs
category: FSharp.Compiler.Service
categoryindex: 300
index: 301
---
*)
(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: APIs for the untyped AST
=========================================

## The ParsedInput module

As established in [Tutorial: Expressions](./untypedtree.html#Walking-over-the-AST), the AST held in a [`ParsedInput`](../reference/fsharp-compiler-syntax-parsedinput.html) value
can be traversed by a set of recursive functions. It can be tedious and error-prone to write these functions from scratch every time, though,
so the [`ParsedInput` module](../reference/fsharp-compiler-syntax-parsedinputmodule.html)
exposes a number of functions to make common operations easier.

For example:

- [`ParsedInput.exists`](../reference/fsharp-compiler-syntax-parsedinputmodule.html#exists)
    - May be used by tooling to determine whether the user's cursor is in a certain context, e.g., to determine whether to offer a certain tooling action.
- [`ParsedInput.fold`](../reference/fsharp-compiler-syntax-parsedinputmodule.html#fold)
    - May be used when writing analyzers to collect diagnostic information for an entire source file.
- [`ParsedInput.foldWhile`](../reference/fsharp-compiler-syntax-parsedinputmodule.html#foldWhile)
    - Like `fold` but supports stopping traversal early.
- [`ParsedInput.tryNode`](../reference/fsharp-compiler-syntax-parsedinputmodule.html#tryNode)
    - May be used by tooling to get the last (deepest) node under the user's cursor.
- [`ParsedInput.tryPick`](../reference/fsharp-compiler-syntax-parsedinputmodule.html#tryPick)
    - May be used by tooling to find the first (shallowest) matching node near the user's cursor.
- [`ParsedInput.tryPickLast`](../reference/fsharp-compiler-syntax-parsedinputmodule.html#tryPickLast)
    - May be used by tooling to find the last (deepest) matching node near the user's cursor.

## SyntaxVisitorBase & SyntaxTraversal.Traverse

While the `ParsedInput` module functions are usually the simplest way to meet most needs,
there is also a [`SyntaxVisitorBase`](../reference/fsharp-compiler-syntax-syntaxvisitorbase-1.html)-based API that can
provide somewhat more fine-grained control over syntax traversal for a subset of use-cases at the expense of a bit more
ceremony and complexity.

## Examples

Let's start by introducing a helper function for constructing an AST from source code so we can run through some real examples:
*)

#r "FSharp.Compiler.Service.dll"
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Syntax

let checker = FSharpChecker.Create()

/// A helper for constructing a `ParsedInput` from a code snippet.
let mkTree codeSample =
    let parseFileResults =
        checker.ParseFile(
            "FileName.fs",
            SourceText.ofString codeSample,
            { FSharpParsingOptions.Default with SourceFiles = [| "FileName.fs" |] }
        )
        |> Async.RunSynchronously

    parseFileResults.ParseTree

(**
### ParsedInput.exists

Now consider the following code sample:
*)

let brokenTypeDefn = """
module Lib

// Whoops, we forgot the equals sign.
type T { A: int; B: int }  
"""

(**
Let's say we have a code fix for adding an equals sign to a type definition that's missing one—like the one above.
We want to offer the fix when the user's cursor is inside of—or just after—the broken type definition.

We can determine this by using `ParsedInput.exists` and passing in the position of the user's cursor:
*)

// type T { A: int; B: int } 
// ···········↑
let posInMiddleOfTypeDefn = Position.mkPos 5 12

(**
Given that cursor position, all we need to do is find a `SynTypeDefn` node:
*)

let isPosInTypeDefn = // true.
    (posInMiddleOfTypeDefn, mkTree brokenTypeDefn)
    ||> ParsedInput.exists (fun _path node ->
        match node with
        | SyntaxNode.SynTypeDefn _ -> true
        | _ -> false)

(**
If the position passed into `ParsedInput.exists` is not contained in any node in the given AST,
but rather is below or to the right of all nodes, `ParsedInput.exists` will fall back to exploring the nearest branch above
and/or to the left. This is useful because the user's cursor may lie beyond the range of all nodes.
*)

// type T { A: int; B: int }  
// ··························↑
let posAfterTypeDefn = Position.mkPos 5 28

(**
Our function still returns `true` if the cursor is past the end of the type definition node itself:
*)

let isPosInTypeDefn' = // Still true.
    (posAfterTypeDefn, mkTree brokenTypeDefn)
    ||> ParsedInput.exists (fun _path node ->
        match node with
        | SyntaxNode.SynTypeDefn _ -> true
        | _ -> false)

(**
### ParsedInput.fold

`ParsedInput.fold` can be useful when writing an analyzer to collect diagnostics from entire input files.
*)

(*** hide ***)
let getLineStr (line: int) : string = failwith "Nope."

(**
Take this code that has unnecessary parentheses in both patterns and expressions:
*)

let unnecessaryParentheses = """
let (x) = (id (3))
"""

(**
We can gather the ranges of all unnecessary parentheses like this:
*)

open System.Collections.Generic

module HashSet =
    let add item (set: HashSet<_>) =
        ignore (set.Add item)
        set

let unnecessaryParenthesesRanges =
   (HashSet Range.comparer, mkTree unnecessaryParentheses) ||> ParsedInput.fold (fun ranges path node ->
       match node with
       | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner; rightParenRange = Some _; range = range)) when
           not (SynExpr.shouldBeParenthesizedInContext getLineStr path inner)
           ->
           ranges |> HashSet.add range

       | SyntaxNode.SynPat(SynPat.Paren(inner, range)) when
           not (SynPat.shouldBeParenthesizedInContext path inner)
           ->
           ranges |> HashSet.add range

       | _ ->
           ranges)

(**
### ParsedInput.tryNode

Sometimes, we might just want to get whatever node is directly at a given position—for example, if the user's
cursor is on an argument of a function being applied, we can find the node representing the argument and use its path
to backtrack and find the function's name.
*)

let functionApplication = """
f x y  
"""

(**
If we have our cursor on `y`:
*)

// f x y
// ·····↑
let posOnY = Position.mkPos 2 5

(**
The syntax node representing the function `f` technically contains the cursor's position,
but `ParsedInput.tryNode` will keep diving until it finds the _deepest_ node containing the position.

We can thus get the node representing `y` and its ancestors (the `path`) like this:
*)

let yAndPath = // Some (SynExpr (Ident y), [SynExpr (App …); …])
    mkTree functionApplication
    |> ParsedInput.tryNode posOnY

(**
Note that, unlike `ParsedInput.exists`, `ParsedInput.tryPick`, and `ParsedInput.tryPickLast`,
`ParsedInput.tryNode` does _not_ fall back to the nearest branch above or to the left.
*)

// f x y
// ······↑
let posAfterY = Position.mkPos 2 8

(**
If we take the same code snippet but pass in a position after `y`,
we get no node:
*)

let nope = // None.
    mkTree functionApplication
    |> ParsedInput.tryNode posAfterY

(**
### ParsedInput.tryPick

Now imagine that we have a code fix for converting a record construction expression into an anonymous record construction
expression when there is no record type in scope whose fields match.
*)

let recordExpr = """
let r = { A = 1; B = 2 }
"""

(**
We can offer this fix when the user's cursor is inside of a record expression by
using `ParsedInput.tryPick` to return the surrounding record expression's range, if any.
*)

// let r = { A = 1; B = 2 }
// ······················↑
let posInRecordExpr = Position.mkPos 2 25

(**
Here, even though `ParsedInput.tryPick` will try to cleave to the given position by default,
we want to verify that the record expression node that we've come across actually contains the position,
since, like `ParsedInput.exists`, `ParsedInput.tryPick` will also fall back to the nearest branch above and/or
to the left if no node actually contains the position. In this case, we don't want to offer the code fix
if the user's cursor isn't actually inside of the record expression.
*)

let recordExprRange = // Some (2,8--2,24).
    (posInRecordExpr, mkTree recordExpr)
    ||> ParsedInput.tryPick (fun _path node ->
        match node with
        | SyntaxNode.SynExpr(SynExpr.Record(range = range)) when
            Range.rangeContainsPos range posInRecordExpr
            -> Some range
        | _ -> None)

(**
We might also sometimes want to make use of the `path` parameter. Take this simple function definition:
*)

let myFunction = """
module Lib

let myFunction paramOne paramTwo =
    ()
"""

(**
Imagine we want to grab the `myFunction` name from the `headPat` in the [`SynBinding`](../reference/fsharp-compiler-syntax-synbinding.html).

We can write a function to match the node we're looking for—and _not_ match anything we're _not_ looking for (like the argument patterns)—by taking its path into account:
*)

let myFunctionId = // Some "myFunction".
    (Position.pos0, mkTree myFunction)
    ||> ParsedInput.tryPick (fun path node ->
        // Match on the node and the path (the node's ancestors) to see whether:
        //   1. The node is a pattern.
        //   2. The pattern is a long identifier pattern.
        //   3. The pattern's parent node (the head of the path) is a binding.
        match node, path with
        | SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = [ ident ]))),
          SyntaxNode.SynBinding _ :: _ ->
            // We have found what we're looking for.
            Some ident.idText
        | _ ->
            // If the node or its context don't match,
            // we continue.
            None)

(**
Instead of traversing manually from `ParsedInput` to `SynModuleOrNamespace` to `SynModuleDecl.Let` to `SynBinding` to `SynPat`, we leverage the default navigation that happens in `ParsedInput.tryPick`.  
`ParsedInput.tryPick` will short-circuit once we have indicated that we have found what we're looking for by returning `Some value`.

Our code sample of course only had one let-binding and thus we didn't need to specify any further logic to differentiate between bindings.

Let's consider a second example involving multiple let-bindings:
*)

let multipleLetsInModule = """
module X

let a = 0
let b = 1
let c = 2
"""

(**
In this case, we know the user's cursor inside an IDE is placed after `c`, and we are interested in the body expression of the _last_ let-binding.
*)

// …
// let c = 2
// ·····↑
let posInLastLet = Position.mkPos 6 5

(**
Thanks to the cursor position we passed in, we do not need to write any code to exclude the expressions of the sibling let-bindings.
`ParsedInput.tryPick` will check whether the current position is inside any given syntax node before drilling deeper.
*)

let bodyOfLetContainingPos = // Some (Const (Int32 2, (6,8--6,9))).
    (posInLastLet, mkTree multipleLetsInModule)
    ||> ParsedInput.tryPick (fun _path node ->
        match node with
        | SyntaxNode.SynBinding(SynBinding(expr = e)) -> Some e
        | _ -> None)

(**
As noted above, `ParsedInput.tryPick` will short-circuit at the first matching node.
`ParsedInput.tryPickLast` can be used to get the _last_ matching node that contains a given position.

Take this example of multiple nested modules:
*)

let nestedModules = """
module M

module N =
    module O =
        module P = begin end
"""

(**
By using `ParsedInput.tryPick`, we'll get the name of the outermost nested module even if we pass in a position inside the innermost,
since the innermost is contained within the outermost.

This position is inside module `P`, which is nested inside of module `O`, which is nested inside of module `N`,
which is nested inside of top-level module `M`:
*)

// module M
//
// module N =
//     module O =
//         module P = begin end
// ···························↑
let posInsideOfInnermostNestedModule = Position.mkPos 6 28

(**
`ParsedInput.tryPick` short-circuits on the first match, and since module `N` is the first
nested module whose range contains position (6, 28), that's the result we get.
*)

let outermostNestedModule = // Some ["N"].
    (posInsideOfInnermostNestedModule, mkTree nestedModules)
    ||> ParsedInput.tryPick (fun _path node ->
        match node with
        | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longId))) ->
            Some [for ident in longId -> ident.idText]
        | _ -> None)

(**
### ParsedInput.tryPickLast

If however we use the same code snippet and pass the same position into `ParsedInput.tryPickLast`,
we can get the name of the _last_ (deepest or innermost) matching node:
*)

let innermostNestedModule = // Some ["P"].
    (posInsideOfInnermostNestedModule, mkTree nestedModules)
    ||> ParsedInput.tryPickLast (fun _path node ->
        match node with
        | SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longId))) ->
            Some [for ident in longId -> ident.idText]
        | _ -> None)

(**
If we want the next-to-innermost nested module, we can do likewise but make use of the `path` parameter:
*)

let nextToInnermostNestedModule = // Some ["O"].
    (posInsideOfInnermostNestedModule, mkTree nestedModules)
    ||> ParsedInput.tryPickLast (fun path node ->
        match node, path with
        | SyntaxNode.SynModule(SynModuleDecl.NestedModule _),
          SyntaxNode.SynModule(SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = longId))) :: _ ->
            Some [for ident in longId -> ident.idText]
        | _ -> None)

(**
### SyntaxTraversal.Traverse

Consider again the following code sample:
*)

let codeSample = """
module Lib

let myFunction paramOne paramTwo =
    ()
"""

(**
Imagine we wish to grab the `myFunction` name from the `headPat` in the [SynBinding](../reference/fsharp-compiler-syntax-synbinding.html).  

We can create a visitor to traverse the tree and find the function name:
*)

let visitor =
    { new SyntaxVisitorBase<string>() with
        override this.VisitPat(path, defaultTraverse, synPat) =
            // First check if the pattern is what we are looking for.
            match synPat with
            | SynPat.LongIdent(longDotId = SynLongIdent(id = [ ident ])) ->
                // Next we can check if the current path of visited nodes, matches our expectations.
                // The path will contain all the ancestors of the current node.
                match path with
                // The parent node of `synPat` should be a `SynBinding`.
                | SyntaxNode.SynBinding _ :: _ ->
                    // We return a `Some` option to indicate we found what we are looking for.
                    Some ident.idText
                // If the parent is something else, we can skip it here.
                | _ -> None
            | _ -> None }

let result = SyntaxTraversal.Traverse(Position.pos0, mkTree codeSample, visitor) // Some "myFunction"

(**
Instead of traversing manually from `ParsedInput` to `SynModuleOrNamespace` to `SynModuleDecl.Let` to `SynBinding` to `SynPat`, we leverage the default navigation that happens in `SyntaxTraversal.Traverse`.  
A `SyntaxVisitorBase` will shortcut all other code paths once a single `VisitXYZ` override has found anything.

Our code sample of course only had one let binding and thus we didn't need to specify any further logic whether to differentiate between multiple bindings.

### SyntaxTraversal.Traverse: using position

Let's now consider a second example where we know the user's cursor inside an IDE is placed after `c` and we are interested in the body expression of the let binding.
*)

let secondCodeSample = """
module X

let a = 0
let b = 1
let c = 2
"""

let secondVisitor =
    { new SyntaxVisitorBase<SynExpr>() with
        override this.VisitBinding(path, defaultTraverse, binding) =
            match binding with
            | SynBinding(expr = e) -> Some e }

let cursorPos = Position.mkPos 6 5

let secondResult =
    SyntaxTraversal.Traverse(cursorPos, mkTree secondCodeSample, secondVisitor) // Some (Const (Int32 2, (6,8--6,9)))

(**
Due to our passed cursor position, we did not need to write any code to exclude the expressions of the other let bindings.
`SyntaxTraversal.Traverse` will check whether the current position is inside any syntax node before drilling deeper.

### SyntaxTraversal.Traverse: using defaultTraverse

Lastly, some `VisitXYZ` overrides can contain a defaultTraverse. This helper allows you to continue the default traversal when you currently hit a node that is not of interest.
Consider `1 + 2 + 3 + 4`, this will be reflected in a nested infix application expression.
If the cursor is at the end of the entire expression, we can grab the value of `4` using the following visitor:
*)

let thirdCodeSample = "let sum = 1 + 2 + 3 + 4"

(*
AST will look like:

Let
 (false,
  [SynBinding
     (None, Normal, false, false, [],
      PreXmlDoc ((1,0), Fantomas.FCS.Xml.XmlDocCollector),
      SynValData
        (None, SynValInfo ([], SynArgInfo ([], false, None)), None,
         None),
      Named (SynIdent (sum, None), false, None, (1,4--1,7)), None,
      App
        (NonAtomic, false,
         App
           (NonAtomic, true,
            LongIdent
              (false,
               SynLongIdent
                 ([op_Addition], [], [Some (OriginalNotation "+")]),
               None, (1,20--1,21)),
            App
              (NonAtomic, false,
               App
                 (NonAtomic, true,
                  LongIdent
                    (false,
                     SynLongIdent
                       ([op_Addition], [],
                        [Some (OriginalNotation "+")]), None,
                     (1,16--1,17)),
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [],
                              [Some (OriginalNotation "+")]), None,
                           (1,12--1,13)),
                        Const (Int32 1, (1,10--1,11)), (1,10--1,13)),
                     Const (Int32 2, (1,14--1,15)), (1,10--1,15)),
                  (1,10--1,17)), Const (Int32 3, (1,18--1,19)),
               (1,10--1,19)), (1,10--1,21)),
         Const (Int32 4, (1,22--1,23)), (1,10--1,23)), (1,4--1,7),
      Yes (1,0--1,23), { LeadingKeyword = Let (1,0--1,3)
                         InlineKeyword = None
                         EqualsRange = Some (1,8--1,9) })
*)

let thirdCursorPos = Position.mkPos 1 22

let thirdVisitor =
    { new SyntaxVisitorBase<int>() with
        override this.VisitExpr(path, traverseSynExpr, defaultTraverse, synExpr) =
            match synExpr with
            | SynExpr.Const (constant = SynConst.Int32 v) -> Some v
            // We do want to continue to traverse when nodes like `SynExpr.App` are found.
            | otherExpr -> defaultTraverse otherExpr }

let thirdResult =
    SyntaxTraversal.Traverse(cursorPos, mkTree thirdCodeSample, thirdVisitor) // Some 4

(**
`defaultTraverse` is especially useful when you do not know upfront what syntax tree you will be walking.  
This is a common case when dealing with IDE tooling. You won't know what actual code the end-user is currently processing.

**Note: SyntaxVisitorBase is designed to find a single value inside a tree!**  
This is not an ideal solution when you are interested in all nodes of certain shape.  
It will always verify if the given cursor position is still matching the range of the node.    
As a fallback the first branch will be explored when you pass `Position.pos0`.  
By design, it is meant to find a single result.

*)
