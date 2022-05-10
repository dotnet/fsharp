// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

/// Capture information about an interface in ASTs
[<RequireQualifiedAccess; NoEquality; NoComparison>]
type InterfaceData =
    | Interface of interfaceType: SynType * memberDefns: SynMemberDefns option
    | ObjExpr of objType: SynType * bindings: SynBinding list

    member Range: range

    member TypeParameters: string []

module InterfaceStubGenerator =

    /// Get members in the decreasing order of inheritance chain
    val GetInterfaceMembers:
        entity: FSharpEntity -> seq<FSharpMemberOrFunctionOrValue * seq<FSharpGenericParameter * FSharpType>>

    /// Check whether an interface is empty
    val HasNoInterfaceMember: entity: FSharpEntity -> bool

    /// Get associated member names and ranges.
    /// In case of properties, intrinsic ranges might not be correct for the purpose of getting
    /// positions of 'member', which indicate the indentation for generating new members
    val GetMemberNameAndRanges: interfaceData: InterfaceData -> (string * range) list

    /// Get interface member signatures
    val GetImplementedMemberSignatures:
        getMemberByLocation: (string * range -> FSharpSymbolUse option) ->
        FSharpDisplayContext ->
        InterfaceData ->
            Async<Set<string>>

    /// Check whether an entity is an interface or type abbreviation of an interface
    val IsInterface: entity: FSharpEntity -> bool

    /// Generate stub implementation of an interface at a start column
    val FormatInterface:
        startColumn: int ->
        indentation: int ->
        typeInstances: string [] ->
        objectIdent: string ->
        methodBody: string ->
        displayContext: FSharpDisplayContext ->
        excludedMemberSignatures: Set<string> ->
        FSharpEntity ->
        verboseMode: bool ->
            string

    /// Find corresponding interface declaration at a given position
    val TryFindInterfaceDeclaration: pos: pos -> parsedInput: ParsedInput -> InterfaceData option
