// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler.Ast
open System.Collections.Generic
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range

#if COMPILER_PUBLIC_API
module Structure =
#else
module internal Structure =
#endif
    /// Collapse indicates the way a range/snapshot should be collapsed. `Same` is for a scope inside
    /// some kind of scope delimiter, e.g. `[| ... |]`, `[ ... ]`, `{ ... }`, etc.  `Below` is for expressions
    /// following a binding or the right hand side of a pattern, e.g. `let x = ...`
    [<RequireQualifiedAccess>]
    type Collapse =
        | Below
        | Same

    /// Tag to identify the constuct that can be stored alongside its associated ranges
    [<RequireQualifiedAccess>]
    type Scope =
        | Open
        | Namespace
        | Module
        | Type
        | Member
        | LetOrUse
        | Val
        | CompExpr
        | IfThenElse
        | ThenInIfThenElse
        | ElseInIfThenElse
        | TryWith
        | TryInTryWith
        | WithInTryWith
        | TryFinally
        | TryInTryFinally
        | FinallyInTryFinally
        | ArrayOrList
        | ObjExpr
        | For
        | While
        | Match
        | MatchLambda
        | MatchClause
        | Lambda
        | CompExprInternal
        | Quote
        | Record
        | SpecialFunc
        | Do
        | New
        | Attribute
        | Interface
        | HashDirective
        | LetOrUseBang
        | TypeExtension
        | YieldOrReturn
        | YieldOrReturnBang
        | Tuple
        | UnionCase
        | EnumCase
        | RecordField
        | RecordDefn
        | UnionDefn
        | Comment
        | XmlDocComment

    /// Stores the range for a construct, the sub-range that should be collapsed for outlinging,
    /// a tag for the construct type, and a tag for the collapse style
    [<NoComparison>]
    type ScopeRange = {
        Scope: Scope
        Collapse: Collapse
        /// HintSpan in BlockSpan
        Range: range
        /// TextSpan in BlockSpan
        CollapseRange:range
    }

    /// Returns outlining ranges for given parsed input.
    val getOutliningRanges : sourceLines: string [] -> parsedInput: ParsedInput -> seq<ScopeRange>