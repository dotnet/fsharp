(**
---
title: Tutorial: SyntaxVisitorBase
category: FSharp.Compiler.Service
categoryindex: 300
index: 301
---
*)
(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Using the SyntaxVisitorBase
=========================================

Syntax tree traversal is a common topic when interacting with the `FSharp.Compiler.Service`.  
As established in [Tutorial: Expressions](./untypedtree.html#Walking-over-the-AST), the [ParsedInput](../reference/fsharp-compiler-syntax-parsedinput.html) can be traversed by a set of recursive functions.  
It can be tedious to always construct these functions from scratch.

As an alternative, a [SyntaxVisitorBase](../reference/fsharp-compiler-syntax-syntaxvisitorbase-1.html) can be used to traverse the syntax tree.
Consider, the following code sample:
*)

let codeSample = """
module Lib

let myFunction paramOne paramTwo =
    ()
"""

(**
Imagine we wish to grab the `myFunction` name from the `headPat` in the [SynBinding](../reference/fsharp-compiler-syntax-synbinding.html).  
Let's introduce a helper function to construct the AST:
*)

#r "FSharp.Compiler.Service.dll"
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Syntax

let checker = FSharpChecker.Create()

/// Helper to construct an ParsedInput from a code snippet.
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
And create a visitor to traverse the tree:
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
Let's consider a second example where we know the user's cursor inside an IDE is placed after `c` and we are interested in the body expression of the let binding.
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
