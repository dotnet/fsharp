// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Syntax

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

/// Represents a major syntax node in the untyped abstract syntax tree.
[<RequireQualifiedAccess>]
type SyntaxNode =
    | SynPat of SynPat
    | SynType of SynType
    | SynExpr of SynExpr
    | SynModule of SynModuleDecl
    | SynModuleOrNamespace of SynModuleOrNamespace
    | SynTypeDefn of SynTypeDefn
    | SynMemberDefn of SynMemberDefn
    | SynMatchClause of SynMatchClause
    | SynBinding of SynBinding
    | SynModuleOrNamespaceSig of SynModuleOrNamespaceSig
    | SynModuleSigDecl of SynModuleSigDecl
    | SynValSig of SynValSig
    | SynTypeDefnSig of SynTypeDefnSig
    | SynMemberSig of SynMemberSig

    /// The range of the syntax node, inclusive of its contents.
    member Range: range

/// Represents the set of ancestor nodes traversed before reaching
/// the current node in a traversal of the untyped abstract syntax tree.
type SyntaxVisitorPath = SyntaxNode list

[<AbstractClass>]
type SyntaxVisitorBase<'T> =
    new: unit -> SyntaxVisitorBase<'T>

    default VisitBinding:
        path: SyntaxVisitorPath * defaultTraverse: (SynBinding -> 'T option) * synBinding: SynBinding -> 'T option

    abstract VisitBinding:
        path: SyntaxVisitorPath * defaultTraverse: (SynBinding -> 'T option) * synBinding: SynBinding -> 'T option

    default VisitComponentInfo: path: SyntaxVisitorPath * synComponentInfo: SynComponentInfo -> 'T option
    abstract VisitComponentInfo: path: SyntaxVisitorPath * synComponentInfo: SynComponentInfo -> 'T option

    /// Controls the behavior when a SynExpr is reached; it can just do
    ///          defaultTraverse(expr)      if you have no special logic for this node, and want the default processing to pick which sub-node to dive deeper into
    /// or can inject non-default behavior, which might incorporate:
    ///          traverseSynExpr(subExpr)   to recurse deeper on some particular sub-expression based on your own logic
    /// path helps to track AST nodes that were passed during traversal
    abstract VisitExpr:
        path: SyntaxVisitorPath *
        traverseSynExpr: (SynExpr -> 'T option) *
        defaultTraverse: (SynExpr -> 'T option) *
        synExpr: SynExpr ->
            'T option

    default VisitExpr:
        path: SyntaxVisitorPath *
        traverseSynExpr: (SynExpr -> 'T option) *
        defaultTraverse: (SynExpr -> 'T option) *
        synExpr: SynExpr ->
            'T option

    abstract VisitHashDirective:
        path: SyntaxVisitorPath * hashDirective: ParsedHashDirective * range: range -> 'T option

    default VisitHashDirective: path: SyntaxVisitorPath * hashDirective: ParsedHashDirective * range: range -> 'T option

    abstract VisitImplicitInherit:
        path: SyntaxVisitorPath *
        defaultTraverse: (SynExpr -> 'T option) *
        inheritedType: SynType *
        synArgs: SynExpr *
        range: range ->
            'T option

    default VisitImplicitInherit:
        path: SyntaxVisitorPath *
        defaultTraverse: (SynExpr -> 'T option) *
        inheritedType: SynType *
        synArgs: SynExpr *
        range: range ->
            'T option

    abstract VisitInheritSynMemberDefn:
        path: SyntaxVisitorPath *
        componentInfo: SynComponentInfo *
        typeDefnKind: SynTypeDefnKind *
        synType: SynType *
        members: SynMemberDefns *
        range: range ->
            'T option

    default VisitInheritSynMemberDefn:
        path: SyntaxVisitorPath *
        componentInfo: SynComponentInfo *
        typeDefnKind: SynTypeDefnKind *
        synType: SynType *
        members: SynMemberDefns *
        range: range ->
            'T option

    abstract VisitRecordDefn: path: SyntaxVisitorPath * fields: SynField list * range -> 'T option
    default VisitRecordDefn: path: SyntaxVisitorPath * fields: SynField list * range -> 'T option

    abstract VisitUnionDefn: path: SyntaxVisitorPath * cases: SynUnionCase list * range -> 'T option
    default VisitUnionDefn: path: SyntaxVisitorPath * cases: SynUnionCase list * range -> 'T option

    abstract VisitEnumDefn: path: SyntaxVisitorPath * cases: SynEnumCase list * range -> 'T option
    default VisitEnumDefn: path: SyntaxVisitorPath * cases: SynEnumCase list * range -> 'T option

    abstract VisitInterfaceSynMemberDefnType: path: SyntaxVisitorPath * synType: SynType -> 'T option
    default VisitInterfaceSynMemberDefnType: path: SyntaxVisitorPath * synType: SynType -> 'T option

    abstract VisitLetOrUse:
        path: SyntaxVisitorPath *
        isRecursive: bool *
        defaultTraverse: (SynBinding -> 'T option) *
        bindings: SynBinding list *
        range: range ->
            'T option

    default VisitLetOrUse:
        path: SyntaxVisitorPath *
        isRecursive: bool *
        defaultTraverse: (SynBinding -> 'T option) *
        bindings: SynBinding list *
        range: range ->
            'T option

    abstract VisitMatchClause:
        path: SyntaxVisitorPath * defaultTraverse: (SynMatchClause -> 'T option) * matchClause: SynMatchClause ->
            'T option

    default VisitMatchClause:
        path: SyntaxVisitorPath * defaultTraverse: (SynMatchClause -> 'T option) * matchClause: SynMatchClause ->
            'T option

    abstract VisitModuleDecl:
        path: SyntaxVisitorPath * defaultTraverse: (SynModuleDecl -> 'T option) * synModuleDecl: SynModuleDecl ->
            'T option

    default VisitModuleDecl:
        path: SyntaxVisitorPath * defaultTraverse: (SynModuleDecl -> 'T option) * synModuleDecl: SynModuleDecl ->
            'T option

    abstract VisitModuleOrNamespace: path: SyntaxVisitorPath * synModuleOrNamespace: SynModuleOrNamespace -> 'T option
    default VisitModuleOrNamespace: path: SyntaxVisitorPath * synModuleOrNamespace: SynModuleOrNamespace -> 'T option

    abstract VisitPat: path: SyntaxVisitorPath * defaultTraverse: (SynPat -> 'T option) * synPat: SynPat -> 'T option
    default VisitPat: path: SyntaxVisitorPath * defaultTraverse: (SynPat -> 'T option) * synPat: SynPat -> 'T option

    abstract VisitRecordField:
        path: SyntaxVisitorPath * copyOpt: SynExpr option * recordField: SynLongIdent option -> 'T option

    default VisitRecordField:
        path: SyntaxVisitorPath * copyOpt: SynExpr option * recordField: SynLongIdent option -> 'T option

    abstract VisitSimplePats: path: SyntaxVisitorPath * pat: SynPat -> 'T option
    default VisitSimplePats: path: SyntaxVisitorPath * pat: SynPat -> 'T option

    abstract VisitType:
        path: SyntaxVisitorPath * defaultTraverse: (SynType -> 'T option) * synType: SynType -> 'T option

    default VisitType: path: SyntaxVisitorPath * defaultTraverse: (SynType -> 'T option) * synType: SynType -> 'T option

    abstract VisitTypeAbbrev: path: SyntaxVisitorPath * synType: SynType * range: range -> 'T option
    default VisitTypeAbbrev: path: SyntaxVisitorPath * synType: SynType * range: range -> 'T option

    abstract VisitAttributeApplication: path: SyntaxVisitorPath * attributes: SynAttributeList -> 'T option
    default VisitAttributeApplication: path: SyntaxVisitorPath * attributes: SynAttributeList -> 'T option

    abstract VisitModuleOrNamespaceSig:
        path: SyntaxVisitorPath * synModuleOrNamespaceSig: SynModuleOrNamespaceSig -> 'T option

    default VisitModuleOrNamespaceSig:
        path: SyntaxVisitorPath * synModuleOrNamespaceSig: SynModuleOrNamespaceSig -> 'T option

    abstract VisitModuleSigDecl:
        path: SyntaxVisitorPath * defaultTraverse: (SynModuleSigDecl -> 'T option) * synModuleSigDecl: SynModuleSigDecl ->
            'T option

    default VisitModuleSigDecl:
        path: SyntaxVisitorPath * defaultTraverse: (SynModuleSigDecl -> 'T option) * synModuleSigDecl: SynModuleSigDecl ->
            'T option

    abstract VisitValSig:
        path: SyntaxVisitorPath * defaultTraverse: (SynValSig -> 'T option) * valSig: SynValSig -> 'T option

    default VisitValSig:
        path: SyntaxVisitorPath * defaultTraverse: (SynValSig -> 'T option) * valSig: SynValSig -> 'T option

module public SyntaxTraversal =

    val internal rangeContainsPosLeftEdgeInclusive: m1: range -> p: pos -> bool

    val internal rangeContainsPosEdgesExclusive: m1: range -> p: pos -> bool

    val internal rangeContainsPosLeftEdgeExclusiveAndRightEdgeInclusive: m1: range -> p: pos -> bool

    val internal dive: node: 'a -> range: 'b -> project: ('a -> 'c) -> 'b * (unit -> 'c)

    val internal pick:
        pos: pos -> outerRange: range -> debugObj: obj -> diveResults: (range * (unit -> 'a option)) list -> 'a option

    val Traverse: pos: pos * parseTree: ParsedInput * visitor: SyntaxVisitorBase<'T> -> 'T option

/// <summary>
/// Holds operations for working with <see cref="T:FSharp.Compiler.Syntax.SyntaxNode"/>s
/// in the untyped abstract syntax tree (AST).
/// </summary>
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SyntaxNode =
    /// <summary>
    /// Extracts the <see cref="T:FSharp.Compiler.Syntax.SynAttributes"/>, if any,
    /// from the given <see cref="T:FSharp.Compiler.Syntax.SyntaxNode"/>.
    /// </summary>
    val (|Attributes|): node: SyntaxNode -> SynAttributes

/// <summary>
/// Holds operations for working with the untyped abstract syntax tree.
/// </summary>
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal SyntaxNodes =
    /// <summary>
    /// Applies the given predicate to each node of the AST and its context (path)
    /// down to a given position, returning true if a matching node is found, otherwise false.
    /// Traversal is short-circuited if no matching node is found through the given position.
    /// </summary>
    /// <param name="predicate">The predicate to match each node against.</param>
    /// <param name="position">The position in the input file down to which to apply the function.</param>
    /// <param name="ast">The AST to search.</param>
    /// <returns>True if a matching node is found, or false if no matching node is found.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// let isInTypeDefn =
    ///     (pos, ast)
    ///     ||> SyntaxNodes.exists (fun _path node ->
    ///         match node with
    ///         | SyntaxNode.SynTypeDefn _ -> true
    ///         | _ -> false)
    /// </code>
    /// </example>
    val exists: predicate: (SyntaxVisitorPath -> SyntaxNode -> bool) -> position: pos -> ast: SyntaxNode list -> bool

    /// <summary>
    /// Applies a function to each node of the AST and its context (path),
    /// threading an accumulator through the computation.
    /// </summary>
    /// <param name="folder">The function to use to update the state given each node and its context.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="ast">The AST to fold over.</param>
    /// <returns>The final state.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// let unnecessaryParentheses =
    ///    (HashSet Range.comparer, ast) ||> SyntaxNodes.fold (fun acc path node ->
    ///        match node with
    ///        | SyntaxNode.SynExpr (SynExpr.Paren (expr = inner; rightParenRange = Some _; range = range)) when
    ///            not (SynExpr.shouldBeParenthesizedInContext getLineString path inner)
    ///            ->
    ///            ignore (acc.Add range)
    ///            acc
    ///
    ///        | SyntaxNode.SynPat (SynPat.Paren (inner, range)) when
    ///            not (SynPat.shouldBeParenthesizedInContext path inner)
    ///            ->
    ///            ignore (acc.Add range)
    ///            acc
    ///
    ///        | _ -> acc)
    /// </code>
    /// </example>
    val fold:
        folder: ('State -> SyntaxVisitorPath -> SyntaxNode -> 'State) -> state: 'State -> ast: SyntaxNode list -> 'State

    /// <summary>
    /// Applies a function to each node of the AST and its context (path)
    /// until the folder returns <c>None</c>, threading an accumulator through the computation.
    /// </summary>
    /// <param name="folder">The function to use to update the state given each node and its context, or to stop traversal by returning <c>None</c>.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="ast">The AST to fold over.</param>
    /// <returns>The final state.</returns>
    val foldWhile:
        folder: ('State -> SyntaxVisitorPath -> SyntaxNode -> 'State option) ->
        state: 'State ->
        ast: SyntaxNode list ->
            'State

    /// <summary>
    /// Dives to the deepest node that contains the given position,
    /// returning the node and its path if found, or <c>None</c> if no
    /// node contains the position.
    /// </summary>
    /// <param name="position">The position in the input file down to which to dive.</param>
    /// <param name="ast">The AST to search.</param>
    /// <returns>The deepest node containing the given position, along with the path taken through the node's ancestors to find it.</returns>
    val tryNode: position: pos -> ast: SyntaxNode list -> (SyntaxNode * SyntaxVisitorPath) option

    /// <summary>
    /// Applies the given function to each node of the AST and its context (path)
    /// down to a given position, returning <c>Some x</c> for the first node
    /// for which the function returns <c>Some x</c> for some value <c>x</c>, otherwise <c>None</c>.
    /// Traversal is short-circuited if no matching node is found through the given position.
    /// </summary>
    /// <param name="chooser">The function to apply to each node and its context to derive an optional value.</param>
    /// <param name="position">The position in the input file down to which to apply the function.</param>
    /// <param name="ast">The AST to search.</param>
    /// <returns>The first value for which the function returns <c>Some</c>, or <c>None</c> if no matching node is found.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// let range =
    ///     (pos, ast) ||> SyntaxNodes.tryPick (fun _path node ->
    ///       match node with
    ///       | SyntaxNode.SynExpr (SynExpr.InterpolatedString (range = range)) when
    ///           rangeContainsPos range pos
    ///           -> Some range
    ///       | _ -> None)
    /// </code>
    /// </example>
    val tryPick:
        chooser: (SyntaxVisitorPath -> SyntaxNode -> 'T option) -> position: pos -> ast: SyntaxNode list -> 'T option

    /// <summary>
    /// Applies the given function to each node of the AST and its context (path)
    /// down to a given position, returning <c>Some x</c> for the last (deepest) node
    /// for which the function returns <c>Some x</c> for some value <c>x</c>, otherwise <c>None</c>.
    /// Traversal is short-circuited if no matching node is found through the given position.
    /// </summary>
    /// <param name="chooser">The function to apply to each node and its context to derive an optional value.</param>
    /// <param name="position">The position in the input file down to which to apply the function.</param>
    /// <param name="ast">The AST to search.</param>
    /// <returns>The last (deepest) value for which the function returns <c>Some</c>, or <c>None</c> if no matching node is found.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// let range =
    ///     (pos, ast)
    ///     ||> SyntaxNodes.tryPickLast (fun path node ->
    ///         match node, path with
    ///         | FuncIdent range -> Some range
    ///         | _ -> None)
    /// </code>
    /// </example>
    val tryPickLast:
        chooser: (SyntaxVisitorPath -> SyntaxNode -> 'T option) -> position: pos -> ast: SyntaxNode list -> 'T option

/// <summary>
/// Holds operations for working with the
/// untyped abstract syntax tree (<see cref="T:FSharp.Compiler.Syntax.ParsedInput"/>).
/// </summary>
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ParsedInput =
    /// <summary>
    /// Applies the given predicate to each node of the AST and its context (path)
    /// down to a given position, returning true if a matching node is found, otherwise false.
    /// Traversal is short-circuited if no matching node is found through the given position.
    /// </summary>
    /// <param name="predicate">The predicate to match each node against.</param>
    /// <param name="position">The position in the input file down to which to apply the function.</param>
    /// <param name="parsedInput">The AST to search.</param>
    /// <returns>True if a matching node is found, or false if no matching node is found.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// let isInTypeDefn =
    ///     (pos, parsedInput)
    ///     ||> ParsedInput.exists (fun _path node ->
    ///         match node with
    ///         | SyntaxNode.SynTypeDefn _ -> true
    ///         | _ -> false)
    /// </code>
    /// </example>
    val exists:
        predicate: (SyntaxVisitorPath -> SyntaxNode -> bool) -> position: pos -> parsedInput: ParsedInput -> bool

    /// <summary>
    /// Applies a function to each node of the AST and its context (path),
    /// threading an accumulator through the computation.
    /// </summary>
    /// <param name="folder">The function to use to update the state given each node and its context.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="parsedInput">The AST to fold over.</param>
    /// <returns>The final state.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// let unnecessaryParentheses =
    ///    (HashSet Range.comparer, parsedInput) ||> ParsedInput.fold (fun acc path node ->
    ///        match node with
    ///        | SyntaxNode.SynExpr (SynExpr.Paren (expr = inner; rightParenRange = Some _; range = range)) when
    ///            not (SynExpr.shouldBeParenthesizedInContext getLineString path inner)
    ///            ->
    ///            ignore (acc.Add range)
    ///            acc
    ///
    ///        | SyntaxNode.SynPat (SynPat.Paren (inner, range)) when
    ///            not (SynPat.shouldBeParenthesizedInContext path inner)
    ///            ->
    ///            ignore (acc.Add range)
    ///            acc
    ///
    ///        | _ -> acc)
    /// </code>
    /// </example>
    val fold:
        folder: ('State -> SyntaxVisitorPath -> SyntaxNode -> 'State) ->
        state: 'State ->
        parsedInput: ParsedInput ->
            'State

    /// <summary>
    /// Applies a function to each node of the AST and its context (path)
    /// until the folder returns <c>None</c>, threading an accumulator through the computation.
    /// </summary>
    /// <param name="folder">The function to use to update the state given each node and its context, or to stop traversal by returning <c>None</c>.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="parsedInput">The AST to fold over.</param>
    /// <returns>The final state.</returns>
    val foldWhile:
        folder: ('State -> SyntaxVisitorPath -> SyntaxNode -> 'State option) ->
        state: 'State ->
        parsedInput: ParsedInput ->
            'State

    /// <summary>
    /// Dives to the deepest node that contains the given position,
    /// returning the node and its path if found, or <c>None</c> if no
    /// node contains the position.
    /// </summary>
    /// <param name="position">The position in the input file down to which to dive.</param>
    /// <param name="parsedInput">The AST to search.</param>
    /// <returns>The deepest node containing the given position, along with the path taken through the node's ancestors to find it.</returns>
    val tryNode: position: pos -> parsedInput: ParsedInput -> (SyntaxNode * SyntaxVisitorPath) option

    /// <summary>
    /// Applies the given function to each node of the AST and its context (path)
    /// down to a given position, returning <c>Some x</c> for the first node
    /// for which the function returns <c>Some x</c> for some value <c>x</c>, otherwise <c>None</c>.
    /// Traversal is short-circuited if no matching node is found through the given position.
    /// </summary>
    /// <param name="chooser">The function to apply to each node and its context to derive an optional value.</param>
    /// <param name="position">The position in the input file down to which to apply the function.</param>
    /// <param name="parsedInput">The AST to search.</param>
    /// <returns>The first value for which the function returns <c>Some</c>, or <c>None</c> if no matching node is found.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// let range =
    ///     (pos, parsedInput) ||> ParsedInput.tryPick (fun _path node ->
    ///       match node with
    ///       | SyntaxNode.SynExpr (SynExpr.InterpolatedString (range = range)) when
    ///           rangeContainsPos range pos
    ///           -> Some range
    ///       | _ -> None)
    /// </code>
    /// </example>
    val tryPick:
        chooser: (SyntaxVisitorPath -> SyntaxNode -> 'T option) ->
        position: pos ->
        parsedInput: ParsedInput ->
            'T option

    /// <summary>
    /// Applies the given function to each node of the AST and its context (path)
    /// down to a given position, returning <c>Some x</c> for the last (deepest) node
    /// for which the function returns <c>Some x</c> for some value <c>x</c>, otherwise <c>None</c>.
    /// Traversal is short-circuited if no matching node is found through the given position.
    /// </summary>
    /// <param name="chooser">The function to apply to each node and its context to derive an optional value.</param>
    /// <param name="position">The position in the input file down to which to apply the function.</param>
    /// <param name="parsedInput">The AST to search.</param>
    /// <returns>The last (deepest) value for which the function returns <c>Some</c>, or <c>None</c> if no matching node is found.</returns>
    /// <example>
    /// <code lang="fsharp">
    /// let range =
    ///     (pos, parsedInput)
    ///     ||> ParsedInput.tryPickLast (fun path node ->
    ///         match node, path with
    ///         | FuncIdent range -> Some range
    ///         | _ -> None)
    /// </code>
    /// </example>
    val tryPickLast:
        chooser: (SyntaxVisitorPath -> SyntaxNode -> 'T option) ->
        position: pos ->
        parsedInput: ParsedInput ->
            'T option
