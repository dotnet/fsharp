// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Symbols

open System.Text.RegularExpressions

/// Represents the kind of element in a documentation comment ID
[<RequireQualifiedAccess>]
type DocCommentIdKind =
    | Type
    | Method
    | Property
    | Field
    | Event
    | Namespace
    | Unknown

/// Represents a parsed documentation comment ID (cref format)
[<RequireQualifiedAccess>]
type ParsedDocCommentId =
    /// Type reference (T:Namespace.Type)
    | Type of path: string list
    /// Member reference (M:, P:, E:) with type path, member name, generic arity, and kind
    | Member of typePath: string list * memberName: string * genericArity: int * kind: DocCommentIdKind
    /// Field reference (F:Namespace.Type.field)
    | Field of typePath: string list * fieldName: string
    /// Invalid or unparseable ID
    | None

module XmlDocSigParser =
    /// Parse a documentation comment ID string (e.g., "M:Namespace.Type.Method(System.String)")
    let parseDocCommentId (docCommentId: string) =
        // Regex to match documentation comment IDs
        // Groups: kind (T/M/P/F/E/N), entity (dotted path), optional args, optional return type
        let docCommentIdRx =
            Regex(@"^(?<kind>\w):(?<entity>[\w\d#`.]+)(?<args>\(.+\))?(?:~([\w\d.]+))?$", RegexOptions.Compiled)

        // Parse generic args count from function name (e.g., MethodName``1)
        let fnGenericArgsRx =
            Regex(@"^(?<entity>.+)``(?<typars>\d+)$", RegexOptions.Compiled)

        let m = docCommentIdRx.Match(docCommentId)
        let kindStr = m.Groups["kind"].Value

        match m.Success, kindStr with
        | true, ("M" | "P" | "E") ->
            let parts = m.Groups["entity"].Value.Split('.')
            if parts.Length < 2 then
                ParsedDocCommentId.None
            else
                let entityPath = parts[.. (parts.Length - 2)] |> List.ofArray
                let memberOrVal = parts[parts.Length - 1]

                // Try and parse generic params count from the name
                let genericM = fnGenericArgsRx.Match(memberOrVal)

                let (memberOrVal, genericParametersCount) =
                    if genericM.Success then
                        (genericM.Groups["entity"].Value, int genericM.Groups["typars"].Value)
                    else
                        memberOrVal, 0

                let kind =
                    match kindStr with
                    | "M" -> DocCommentIdKind.Method
                    | "P" -> DocCommentIdKind.Property
                    | "E" -> DocCommentIdKind.Event
                    | _ -> DocCommentIdKind.Unknown

                // Handle constructor name conversion (#ctor in doc comments, .ctor in F#)
                let finalMemberName =
                    if memberOrVal = "#ctor" then ".ctor" else memberOrVal

                ParsedDocCommentId.Member(entityPath, finalMemberName, genericParametersCount, kind)

        | true, "T" ->
            let entityPath = m.Groups["entity"].Value.Split('.') |> List.ofArray
            ParsedDocCommentId.Type entityPath

        | true, "F" ->
            let parts = m.Groups["entity"].Value.Split('.')
            if parts.Length < 2 then
                ParsedDocCommentId.None
            else
                let entityPath = parts[.. (parts.Length - 2)] |> List.ofArray
                let memberOrVal = parts[parts.Length - 1]
                ParsedDocCommentId.Field(entityPath, memberOrVal)

        | _ -> ParsedDocCommentId.None
