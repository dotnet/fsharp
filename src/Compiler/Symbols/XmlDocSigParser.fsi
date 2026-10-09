// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Symbols

/// Represents the kind of member element in a documentation comment ID (the `M:`/`P:`/`E:`
/// members carried by ParsedDocCommentId.Member). Types, fields and namespaces have their own
/// ParsedDocCommentId cases and so do not appear here.
[<RequireQualifiedAccess>]
type internal DocCommentIdKind =
    | Method
    | Property
    | Event
    | Unknown

/// Represents a parsed documentation comment ID (cref format)
[<RequireQualifiedAccess>]
type internal ParsedDocCommentId =
    /// Type reference (T:Namespace.Type)
    | Type of path: string list
    /// Member reference (M:, P:, E:) with type path, member name, generic arity, and kind
    | Member of typePath: string list * memberName: string * genericArity: int * kind: DocCommentIdKind
    /// Field reference (F:Namespace.Type.field)
    | Field of typePath: string list * fieldName: string
    /// Invalid or unparseable ID
    | None

module internal XmlDocSigParser =
    /// Parse a documentation comment ID string (e.g., "M:Namespace.Type.Method(System.String)")
    val parseDocCommentId: docCommentId: string -> ParsedDocCommentId
